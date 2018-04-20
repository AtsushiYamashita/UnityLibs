using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace BasicExtends {

    /// <summary>
    /// 対象として指定したmToにむけて、ターゲットを移動させる
    /// 対象位置が移動しても追随するが、時間は固定。
    /// </summary>
    public class TargetTransporterTime: MonoBehaviour {
        [SerializeField]
        private Transform mTarget = null;
        [SerializeField]
        private Transform mTo = null;
        [SerializeField]
        private float mTime = -1;

        public void Move ( float time ) {
            mTime = time == 0 ? 0.001f : time;
        }

        public void Cancel () {
            mTime = -1;
        }

        public void Start () {
            Assert.IsNotNull(mTarget);
            Assert.IsNotNull(mTo);
        }

        public void Update () {
            if (mTime <= 0) { return; }

            var from = mTarget.position;
            var to = mTo.position;
            var per = Mathf.Clamp(Time.deltaTime / mTime, 0, 1);
            var dist = to - from;

            mTarget.position = from + dist * per;
        }
    }
}
