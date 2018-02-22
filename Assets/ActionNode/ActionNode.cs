using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

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

public class NodeList: List<Node> {
    public void Invoke ( string state, ActionNode component ) {
        foreach (var node in this) {
            if (node.mName != state) { continue; }
            node.mActions.Invoke(component);
        }
    }
}

[Serializable]
public class StateList {

    public static readonly uint SIZE = 5;

    [SerializeField]
    private string [] mStates = new string [SIZE];

    public StateList Add ( string str ) {
        ForEach(( e ) => { return e == string.Empty ? string.Empty : str; });
        return this;
    }
    public StateList Remove ( string str ) {
        ForEach(( e ) => { return e == str ? string.Empty : e;  });
        return this;
    }

    public StateList ForEach ( Func<string, string> func ) {
        for (int i = 0; i < SIZE; i++) {
            mStates [i] = func(mStates [i]);
        }
        return this;
    }
    public StateList ForEach ( Action<string> func ) {
        for (int i = 0; i < SIZE; i++) {
             func(mStates [i]);
        }
        return this;
    }
}

public class ActionNode: MonoBehaviour {

    [SerializeField]
    StateList mStates = new StateList();

    [SerializeField]
    private NodeList mNodes = new NodeList  {
        new Node("skiped"),
        new Node("start",( ActionNode node) =>{
            node.mStates.Add("update");
            node.mStates.Remove("start");
        } ),
        new Node("update"),
        new Node("end",( ActionNode node) =>{
            node.mStates.Add("start");
            node.mStates.Remove("end");
            node.gameObject.SetActive(false);
        } ),
    };

    private void Update () {
        mStates.ForEach(( e ) => { mNodes.Invoke(e, this); });
    }
}
