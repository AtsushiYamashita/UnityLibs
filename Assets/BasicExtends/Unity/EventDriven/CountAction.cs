using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountAction: MonoBehaviour {

    [System.Serializable]
    public enum Type {
        SmallerThen, BiggerThen, Equal
    }

    [SerializeField]
    private Type mType = Type.Equal;

    [SerializeField]
    UnityEvent mEvent = new UnityEvent();

    [SerializeField]
    private int mTarget = 0;
    private int mCount = 0;
    System.Func<int, int, bool> [] mFunc = {
        (a,b)=> a<b,
        (a,b)=> a>b,
        (a,b)=> a==b,
    };

    private void Start () {
        CountReset();
    }

    public void CountReset () {
        mCount = 0;
    }

    public void CountSet ( int num ) {
        mCount = num;
    }

    public void TargetSet ( int num ) {
        mTarget = num;
    }

    public void TypeSet ( Type type ) {
        mType = type;
    }

    public void Increment ( int num ) {
        mCount += num;
        if (mFunc [(int) mType](mCount, mTarget)) { mEvent.Invoke(); }
    }
}
