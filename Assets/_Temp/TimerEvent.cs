using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvent : MonoBehaviour {

    [SerializeField]
    private UnityEvent mTimeup = new UnityEvent();
    [SerializeField]
    private float mCallTime = 0;

    private float mStart = -1;

    public void CountStart () {
        mStart = Time.time;
    }

    public void CountStart(float time ) {
        mCallTime = time;
        mStart = Time.time;
    }

    private void Update () {
        if (mStart < 0) { return; }
        if (Time.time - mStart < mCallTime) { return; }
        mStart = -1;
        mTimeup.Invoke();
	}
}
