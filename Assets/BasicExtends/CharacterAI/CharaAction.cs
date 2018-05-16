using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaAction : MonoBehaviour {

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

    private void Reset () {
        SetTargetComponent();
    }

    public virtual float Effect () { return 0; }
    public virtual float GetCost () { return 0; }
}
