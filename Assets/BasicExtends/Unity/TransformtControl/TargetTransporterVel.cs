using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BasicExtends {

    /// <summary>
    /// 対象として指定したmToにむけて、ターゲットを移動させる
    /// 対象位置が移動しても追随するが、時間は固定。
    /// </summary>
    public class TargetTransporterVel: MonoBehaviour {
        [SerializeField]
        private Transform mTarget = null;
        [SerializeField]
        private Transform mTo = null;
        [SerializeField]
        private float mVel = 0;

        public void Move ( float vel ) {
            mVel = vel;
        }

        public void Cancel () {
            mVel = -0;
        }

        public void Start () {
            Assert.IsNotNull(mTarget);
            Assert.IsNotNull(mTo);
        }

        public void Update () {
            if (mVel == 0) { return; }

            var from = mTarget.position;
            var to = mTo.position;
            var dist = to - from;
            mTarget.position = from + dist.normalized * mVel;
        }
    }
}
