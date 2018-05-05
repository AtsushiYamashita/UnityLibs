using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BasicExtends;

/// <summary>
/// 指定されたComponentを修飾子として、
/// 条件判定を行ってイベントを呼び出す
///
/// ValueStoreから値を取り出す場合は「＄名前」とする
/// </summary>
public class ValueCheckCall: MonoBehaviour {

    [SerializeField]
    UnityEvent mTrueEvent = new UnityEvent();
    [SerializeField]
    UnityEvent mFalseEvent = new UnityEvent();
    [SerializeField]
    private Component mTarget = null;

    public Component Target { get { return mTarget; } }

    /// <summary>
    /// 判定に何らかの主体がいる場合はターゲットとして設定する
    /// </summary>
    public void SetTargetComponent ( Component target ) {
        mTarget = target;
    }

    /// <summary>
    /// 判定に何らかの主体がいる場合はターゲットとして設定する
    /// </summary>
    public void SetTargetComponent () {
        if (transform == transform.root) {
            mTarget = transform;
            return;
        }
        var target = transform.parent;
        while (target != transform.root) {
            var v = target.GetComponent<ValueCheckCall>();
            if (v.IsNull()) {
                target = target.parent;
                continue;
            }
            mTarget = v.Target;
            return;
        }
        mTarget = target;
    }

    public bool ThenDo ( bool bl ) {
        var call = bl ? mTrueEvent : mFalseEvent;
        call.Invoke();
        return bl;
    }

    private void Reset () {
        SetTargetComponent();
    }

}
