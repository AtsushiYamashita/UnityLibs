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
            public Node ( string str, UnityAction<ActionNode> act ) : this(str) { mActions.AddListener(act); }

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

            public void Invoke ( string state, ActionNode component ) {
                ForEach(( e ) =>
                {
                    if (e.mName != state.ToUpper()) { return; }
                    e.mActions.Invoke(component);
                });
            }
        }


        public class ActionNode: MonoBehaviour {

            [SerializeField]
            private string mState = ("start");

            [SerializeField]
            private NodeArray mNodes = new NodeArray();
                
            private void Reset () {
                mNodes.Add(new Node("start", ( ActionNode node ) =>
                {
                    node.mState = ("update");
                }))
                .Add(new Node("update"))
                .Add(new Node("end", ( ActionNode node ) =>
                {
                    node.mState = ("start");
                    node.gameObject.SetActive(false);
                }));
            }

            private void Update () {
                mNodes.Invoke(mState, this);
            }

            public void SetState ( string state ) {
                mState = state;
            }

            public void DebugPrint ( string str ) {
                DebugLog.Log.Print("{0}({1}){2}{3} "
                    ,gameObject.name ,mState
                    ,str.Length == 0 ? "":" : ", str);
            }
        }
    }

}