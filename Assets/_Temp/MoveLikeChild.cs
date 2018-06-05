using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLikeChild: MonoBehaviour {

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
        var pos = mStartFix.position;
        if (mMoveX) { pos.x = mPlaceHolder.position.x; }
        if (mMoveY) { pos.y = mPlaceHolder.position.y; }
        if (mMoveZ) { pos.z = mPlaceHolder.position.z; }
        transform.position = pos;
        transform.rotation = mPlaceHolder.rotation;

    }
}
