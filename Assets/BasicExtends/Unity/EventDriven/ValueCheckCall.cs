using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BasicExtends;

public class ValueCheckCall: MonoBehaviour {

    [SerializeField]
    UnityEvent mTrueEvent = new UnityEvent();
    [SerializeField]
    UnityEvent mFalseEvent = new UnityEvent();
    [SerializeField]
    private bool mResult = false;
    [SerializeField]
    private Component mTarget = null;

    /// <summary>
    /// 判定に何らかの主体がいる場合はターゲットとして設定する
    /// </summary>
    public void SetTargetComponent ( Component target) {
        mTarget = target;
    }

    public bool ThenDo ( bool bl ) {
        mResult = bl;
        var call = bl ? mTrueEvent : mFalseEvent;
        call.Invoke();
        return bl;
    }

    /// <summary>
    /// この判定における「型」はすべてfloatで扱う。
    /// </summary>
    private float ToValue ( string str ) {
        if (str.IndexOf("$") < 0) {
            float r = 0;
            float.TryParse(str, out r);
            return r;
        }
        var key1 = (mTarget.IsNotNull() ? "" + mTarget.name : "");
        var key2 = str.Substring(1);
        var v = FindableStore.TryGet<float>(key1+"."+key2);
        if (v.Check) {
            return v.Value;
        }
        var e = string.Format("({0}) is missing", str);
        throw new Exception(e);
    }

    private Func<float, float, bool> ToFunc ( string type ) {
        switch (type) {
            case "<": return ( a, b ) => a < b;
            case ">": return ( a, b ) => a > b;
            case "<=": return ( a, b ) => a <= b;
            case ">=": return ( a, b ) => a >= b;
            case "==": return ( a, b ) => a == b;
            case "!=": return ( a, b ) => a != b;
        }
        var e = string.Format("({0}) is missing", type);
        throw new Exception(e);
    }

    public bool Check ( string script ) {
        var queue = new Queue<string>(script.Split(" ".ToCharArray() [0]));
        var v1 = ToValue(queue.Dequeue());
        var f = ToFunc(queue.Dequeue());
        var v2 = ToValue(queue.Dequeue());
        var res = f(v1, v2);
        return ThenDo(res);
    }
}
