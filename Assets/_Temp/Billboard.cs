using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    [SerializeField]
    private Camera mTarget;

    [SerializeField]
    private Vector3 mFix = new Vector3(0, 180, 0);

    [SerializeField]
    private bool mIsOnlyY = false;

    private Vector3 mStable;

    private void Start () {
        if (mIsOnlyY == false) { return; }
        mStable = transform.rotation.eulerAngles;
    }

    private void OnWillRenderObject () {
        if (mTarget == null) { mTarget = Camera.main; return; }
        transform.LookAt(mTarget.transform);
        transform.Rotate(mFix);
        if (mIsOnlyY == false) { return; }
        mStable.y = transform.rotation.eulerAngles.y;
        transform.eulerAngles = mStable;
    }
}
