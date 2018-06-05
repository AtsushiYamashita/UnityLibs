using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BasicExtends;
using System.Linq;

internal enum TouchPhaseEx: int {

    /// <summary>
    /// A finger touched the screen.
    /// </summary>
    Began,

    /// <summary>
    ///  A finger moved on the screen.
    /// </summary>
    Moved,

    /// <summary>
    ///  A finger is touching the screen but hasn't moved.
    /// </summary>
    Stationary,

    /// <summary>
    ///   A finger was lifted from the screen. This is the final phase of a touch.
    /// </summary>
    Ended,

    /// <summary>
    ///    The system cancelled tracking for the touch.
    /// </summary>
    Canceled,

    /// <summary>
    /// A finger is long touch  
    /// </summary>
    LongTouchStart,

    /// <summary>
    /// A finger is long touch  
    /// </summary>
    LongTouching,

    /// <summary>
    /// this enum's length
    /// </summary>
    Length,
}

internal static class TouchPhaseConvert {
    internal static TouchPhase [] _TouchPhaseArr = (TouchPhase []) System.Enum.GetValues(typeof(TouchPhase));
    internal static TouchPhaseEx [] _TouchPhaseExArr = (TouchPhaseEx []) System.Enum.GetValues(typeof(TouchPhaseEx));
    internal static TouchPhaseEx Convert ( this TouchPhase touchPhase ) {
        return _TouchPhaseExArr [(int) touchPhase];
    }
}

[System.Serializable]
public class TouchEventCaller {
    [SerializeField]
    internal TouchPhaseEx mState = TouchPhaseEx.Began;

    [SerializeField]
    internal UnityEvent mEvent = new UnityEvent();
}

/// <summary>
/// タッチイベントが発生したときに
/// </summary>
public class TouchEvent: MonoBehaviour {

    [SerializeField]
    List<TouchEventCaller> mTouchEvent = new List<TouchEventCaller>();

    [SerializeField]
    private float mLongTouch = 2;

    Dictionary<TouchPhaseEx, bool> mIsTrigger = new Dictionary<TouchPhaseEx, bool>();
    private const int MULTI_TOUCH_MAX = 11;
    float [] mIsTouchStart = new float [MULTI_TOUCH_MAX];

    private bool mExistLongTouch = false;

    private void Start () {
        for (int i = 0; i < mIsTouchStart.Length; i++) {
            mIsTouchStart [i] = -1;
        }

        var arr = (TouchPhaseEx []) System.Enum.GetValues(typeof(TouchPhaseEx));
        for (int i = 0; i < (int) TouchPhaseEx.Length; i++) {
            mIsTrigger.TrySet(arr [i], false);
        }
        mTouchEvent.Sort(( a, b ) => { return (int) a.mState - (int) b.mState; });
    }

    private List<TouchPhaseEx> TouchCheck () {
        int long_touch = 0;
        foreach (var touch in Input.touches) {
            var phase = touch.phase;
            mIsTrigger [phase.Convert()] = true;
            if (phase == TouchPhase.Began) { mIsTouchStart [touch.fingerId] = Time.time; }
            if (phase == TouchPhase.Ended) { mIsTouchStart [touch.fingerId] = -1; }
            if (mIsTouchStart [touch.fingerId] < 0) { continue; }
            long_touch += Time.time - mIsTouchStart [touch.fingerId] > mLongTouch ? 1 : 0;
        }
        var prev = mExistLongTouch;
        mExistLongTouch = long_touch > 0;
        if (mExistLongTouch) {
            mIsTrigger [TouchPhaseEx.LongTouching] = true;
            if (prev == false) { mIsTrigger [TouchPhaseEx.LongTouchStart] = true; }
        }
        return mIsTrigger.Where(e => e.Value).Select(e => e.Key).ToList();
    }

    private void EventInvoke ( List<TouchPhaseEx> touched ) {
        for (int i = 0; i < touched.Count; i++) {
            var state = touched [i];
            var eve = mTouchEvent.Where(e => e.mState == state);
            foreach (var e in eve) { e.mEvent.Invoke(); }
            mIsTrigger [state] = false;
        }
    }

    private void Update () {
        var touched = TouchCheck();
        EventInvoke(touched);
    }
}
