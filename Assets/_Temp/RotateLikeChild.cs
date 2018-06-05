using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLikeChild : MonoBehaviour {
    [SerializeField]
    private Transform mParent = null;

    private Transform mPlaceHolder = null;
    private Transform mStartFix = null;

    [SerializeField]
    private bool mMoveX = true;
    [SerializeField]
    private bool mMoveY = true;
    [SerializeField]
    private bool mMoveZ = true;

    private void Setup () {
        mPlaceHolder = Instantiate(
            new GameObject()
            , transform.position
            , transform.rotation
            , mParent).transform;
        mStartFix = Instantiate(
            new GameObject()
            , transform.position
            , transform.rotation).transform;
    }

    private void Update () {
        if (mPlaceHolder == null) { Setup(); }
        var rot = mStartFix.eulerAngles;
        if (mMoveX) { rot.x = mPlaceHolder.eulerAngles.x; }
        if (mMoveY) { rot.y = mPlaceHolder.eulerAngles.y; }
        if (mMoveZ) { rot.z = mPlaceHolder.eulerAngles.z; }
        transform.eulerAngles = rot;
        transform.position = mPlaceHolder.position;
    }
}
