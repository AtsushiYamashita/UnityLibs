#if false

using System;
using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity.InputModule;


public class HoloButton: MonoBehaviour, IInputHandler {

    [SerializeField]
    private UnityEvent mOnTargetIn = new UnityEvent();
    [SerializeField]
    private UnityEvent mOnTapDown = new UnityEvent();
    [SerializeField]
    private UnityEvent mOnTap = new UnityEvent();
    [SerializeField]
    private UnityEvent mOnTapUp = new UnityEvent();
    [SerializeField]
    private UnityEvent mOnTargetOut = new UnityEvent();

    private int mButtonDownCount = 0;

    private bool mWasGazed = false;

    private void NotGazingMe () {
        if (mWasGazed == false) { return; }
        mOnTargetOut.Invoke();
    }


    /// <summary>
    /// Press the button programatically.
    /// </summary>
    void IInputHandler.OnInputDown ( InputEventData eventData ) {
        mButtonDownCount++;
    }

    void IInputHandler.OnInputUp ( InputEventData eventData ) {
        eventData.Use();
        mButtonDownCount = -1;
    }


    private void Tap () {
        // Debug.Log("mButtonDownCount " + mButtonDownCount);
        var isDown = mButtonDownCount == 1 || Input.GetKeyDown(KeyCode.Space);
        //カウントアップの前に、Downの判定
        mButtonDownCount++;
        if (isDown) {
            // Debug.Log("mOnTapDown");
            mOnTapDown.Invoke();
            return;
        }
        // Debug.Log("mOnTap");
        mOnTap.Invoke();
    }

    private void GazingMe () {
        if (mWasGazed == false) {
            mOnTargetIn.Invoke();
        }
        if (mButtonDownCount > 0 || Input.GetKey(KeyCode.Space)) {
            Tap();
            return;
        }
        if(mButtonDownCount <0 || Input.GetKeyUp(KeyCode.Space)) {
            mOnTapUp.Invoke();
            mButtonDownCount = 0;
            return;
        }
    }

    private void Start () {
        //InputManager.Instance.AddGlobalListener(gameObject);
    }

    private bool GazedNow () {
        var hit = GazeManager.Instance.HitObject;
        if (hit == null) { return false; }
        return hit == gameObject;
    }

    void Update () {
        var now = GazedNow();
        Action ButtonAction = now ? (Action) GazingMe : NotGazingMe;
        ButtonAction();
        mWasGazed = now;
    }
}

#endif
