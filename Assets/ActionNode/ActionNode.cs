using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System;
using BasicExtends;

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
            if (e.mName != state) { return; }
            e.mActions.Invoke(component);
        });
    }
}

[Serializable]
public class StateArray: BufferedArray<string> {

    public new StateArray Add ( string str ) {
        return (StateArray) base.Add(str.ToUpper());
    }

    public StateArray Remove ( string str ) {
        Map(( e ) => { return e == str.ToUpper() ? string.Empty : e; });
        return this;
    }
}

public class ActionNode: MonoBehaviour {

    [SerializeField]
    StateArray mStates = new StateArray().Add("start");

    [SerializeField]
    private NodeArray mNodes = new NodeArray()
        .Add(new Node("skiped"))
        .Add(new Node("start", ( ActionNode node ) =>
        {
            node.mStates.Add("update");
            node.mStates.Remove("start");
        }))
        .Add(new Node("update"))
        .Add(new Node("end", ( ActionNode node ) =>
        {
            node.mStates.Add("start");
            node.mStates.Remove("end");
            node.gameObject.SetActive(false);
        }));

    private void Update () {
        mStates.ForEach(( e ) => { mNodes.Invoke(e, this); });
    }

    /// <summary>
    /// When received "A to B",
    /// thne this switch own state A to B.
    /// </summary>
    /// <param name="str"></param>
    public void Switch(string str ) {
        var strs = str.Split(' ');
        Assert.IsTrue(strs.Length == 3);
        var a = strs [0];
        var b = strs [2];
        mStates.Remove(b).Add(a);
    }
}
