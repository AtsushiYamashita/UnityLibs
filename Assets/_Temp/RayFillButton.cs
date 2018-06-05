using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// 見つめることで起動するボタンの実装
/// </summary>
[RequireComponent(typeof(Collider))]
public class RayFillButton : RayEvent {

    [SerializeField]
    private float mFillTime = 1.5f;

    [SerializeField]
    private Image mImage = null;

    [SerializeField]
    private UnityEvent mFillMax = new UnityEvent();

    private float mLastInvoked = 0;
    private const float INVOKE_WAIT = 3;

    private void Start () {
        Assert.IsNotNull(mImage);
        Assert.IsTrue(mFillTime > 0);
	}
	
    public override void OnStay ( Camera c, float t ) {
        var per = t / mFillTime;
        mImage.fillAmount = per;
        if(per < 1) { return; }
        var time = Time.time;
        if (time - mLastInvoked < INVOKE_WAIT) { return; }
        mLastInvoked = time;
        mFillMax.Invoke();
    }

    public override void OnExit ( Camera c, float t ) {
        Debug.Log("RayFillButton(=OnExit");
        mImage.fillAmount = 0;
        base.OnExit(c, t);
    }
}
