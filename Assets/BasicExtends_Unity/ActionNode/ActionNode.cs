using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System;
using BasicExtends;


namespace BasicExtends {

    namespace ActionNode {
        [Serializable]
        public class NodeEvent: UnityEvent<ActionNode> { }

        [Serializable]
        public class Node {
            public string mName = string.Empty;
            public NodeEvent mActions = new NodeEvent();

            public Node () { }
            public Node ( string str ) { mName = str.ToUpper(); }
            public Node ( string str
                , UnityAction<ActionNode> act ) : this(str) {
                mActions.AddListener(act);
            }

        }

        [Serializable]
        public class NodeArray: BufferedArray<Node> {

            public new NodeArray Add ( Node node ) {
                return (NodeArray) base.Add(node);
            }

            public NodeArray Remove ( Node node ) {
                Map(( e ) => { return e.mName == node.mName ? null : e; });
                SetFront();
                return this;
            }

            private void AfterAction ( string state, ActionNode component ) {
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

            public void Invoke ( string state, ActionNode component ) {
                ForEach(( e ) =>
                {
                    Assert.IsNotNull(e);
                    if (e.mName != state.ToUpper()) { return; }
                    e.mActions.Invoke(component);
                });
                AfterAction(state, component);
            }
        }


        public class ActionNode: MonoBehaviour {

            [SerializeField]
            private string mState = ("start");

            [SerializeField]
            private NodeArray mNodes = new NodeArray();

            //メインスレッド以外から呼び出すときもあるため必要
            private string mObjectName = "";

            private void Start () {
                MessengerSetup();
                mObjectName = gameObject.name;
            }

            private void MessengerSetup () {
                Messenger.Assign(( Msg msg ) =>
                {
                    if (msg.Unmatch("to", mObjectName)) { return; }
                    if (msg.Unmatch("as", GetType().Name)) { return; }
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
                    .Add(new Node("start"))
                .Add(new Node("update"))
                .Add(new Node("end"));
            }

            private void Update () {
                mNodes.Invoke(mState, this);
            }

            public void SetState ( string state ) {
                mState = state;
            }

            public void DebugPrint ( string str ) {
                DebugLog.Log.Print("{0}({1}){2}{3} "
                    , gameObject.name, mState
                    , str.Length == 0 ? "" : " : ", str);
            }
        }
    }
}
