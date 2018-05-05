using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System;
using BasicExtends.StateEventImp;

namespace BasicExtends {
    namespace StateEventImp {
        [Serializable]
        public class NodeEvent: UnityEvent<StateEvent> { }

        [Serializable]
        public class EventNode {
            public string mName = string.Empty;
            public NodeEvent mActions = new NodeEvent();

            public EventNode () { }
            public EventNode ( string str ) { mName = str.ToUpper(); }
            public EventNode ( string str
                , UnityAction<StateEvent> act ) : this(str) {
                mActions.AddListener(act);
            }

        }

        [Serializable]
        public class NodeArray: BufferedArray<EventNode> {

            public new NodeArray Add ( EventNode node ) {
                return (NodeArray) base.Add(node);
            }

            public NodeArray Remove ( EventNode node ) {
                Map(( e ) => { return e.mName == node.mName ? null : e; });
                SetFront();
                return this;
            }

            private void AfterAction ( string state, StateEvent component ) {
                if (state.ToUpper() == "START") {
                    component.SetState("update");
                    return;
                }
                if (state.ToUpper() == "END") {
                    component.SetState("start");
                    component.gameObject.SetActive(false);
                    return;
                }
            }

            public void Invoke ( string state, StateEvent component ) {
                ForEach(( e ) =>
                {
                    Assert.IsNotNull(e);
                    if (e.mName.ToUpper() != state.ToUpper()) { return; }
                    e.mActions.Invoke(component);
                });
                AfterAction(state, component);
            }
        }


    }

    public class StateEvent: MonoBehaviour {

        [SerializeField]
        private string mState = ("start");

        [SerializeField]
        private NodeArray mNodes = new NodeArray();

        //メインスレッド以外から呼び出すときもあるため必要
        private string mObjectName = "";

        protected virtual void StartProcess () { }

        private void Start () {
            mObjectName = gameObject.name;
            StartProcess();
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", mObjectName)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("Network", "true")) { return; }
                if (msg.Match("act", "SetState")) {
                    var state = msg.TryGet("state");
                    SetState(state);
                    return;
                }
            });
        }
        private void Reset () {
            //if (mNodes.Length != 0) { return; }
            mNodes.Reset()
                .Add(new EventNode("start"))
                .Add(new EventNode("update"))
                .Add(new EventNode("end"));
        }

        private void Update () {
            mNodes.Invoke(mState, this);
        }

        public void SetState ( string state ) {
            mState = state;
        }

        public void SetUpdate () {
            SetState("update");
        }
        public void SetEnd () {
            SetState("end");
        }

        public void DebugPrint ( string str ) {
            DebugLog.Log.Print("{0}({1}){2}{3} "
                , gameObject.name, mState
                , str.Length == 0 ? "" : " : ", str);
        }
    }
}
