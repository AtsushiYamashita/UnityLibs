namespace BasicExtends {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.Events;

    /// <summary>
    /// Triggerに関するイベントをコールします。
    /// 通常のOnTriggerEnterなどを使った判定よりは確実にイベントコールしますが、
    /// 代わりに少し負荷が大きいです。
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class TriggerEventer: MonoBehaviour {

        public class HitEvent: UnityEvent<GameObject[]> { }
        enum EventType { In, Stay, Out }

        [SerializeField]
        private List<GameObject> mTargetObject = new List<GameObject>();

        [SerializeField]
        private List<string> mTargetPath = new List<string>();

        [SerializeField]
        private List<string> mTargetTag = new List<string>();

        /// <summary>
        /// トリガーにターゲットのいずれかが入った瞬間に呼ばれる
        /// </summary>
        [SerializeField]
        private HitEvent mIn = new HitEvent();

        /// <summary>
        /// トリガーにターゲットのいずれかが入っている間に呼ばれる
        /// </summary>
        [SerializeField]
        private HitEvent mStay = new HitEvent();

        /// <summary>
        /// トリガーからターゲットのいずれかが出た瞬間に呼ばれる
        /// </summary>
        [SerializeField]
        private HitEvent mOut = new HitEvent();

        private List<Collider> mPrev = new List<Collider>();
        private List<Collider> mNow = new List<Collider>();

        void Start () {
            var target_len = mTargetObject.Count + mTargetPath.Count + mTargetTag.Count;
            Assert.IsTrue(target_len > 0);
        }

        private bool IsTarget ( Collider col ) {
            if (mTargetPath.Contains(col.name)) { return true; }
            if (mTargetTag.Contains(col.tag)) { return true; }
            if (mTargetObject.Contains(col.gameObject)) { return true; }
            return false;
        }

        private void OnTriggerStay ( Collider col ) {
            if (IsTarget(col) == false) { return; }
            mNow.Add(col);
        }

        private static int ComparisonID ( Object x, Object y ) {
            return x.GetInstanceID() - y.GetInstanceID();
        }

        private static void Invoke (HitEvent eve, params GameObject[] obj ) {
            eve.Invoke(obj);
        }

        private void CheckAndEventcall (
            Pair<int, List<Collider>.Enumerator> pre,
            Pair<int, List<Collider>.Enumerator> now ) {

            // 空集合処理
            if (pre.Key == -1) {
                Invoke(mIn, now.Value.Current.gameObject);
                now.Key = now.Value.MoveNext() ? now.Value.Current.GetInstanceID() : -1;
                return;
            }
            if (now.Key == -1) {
                Invoke(mOut, pre.Value.Current.gameObject);
                pre.Key = pre.Value.MoveNext() ? pre.Value.Current.GetInstanceID() : -1;
                return;
            }

            // 集合一つの処理
            if(pre.Key < now.Key) {
                Invoke(mOut, pre.Value.Current.gameObject);
                pre.Key = pre.Value.MoveNext() ? pre.Value.Current.GetInstanceID() : -1;
                return;
            }
            if(pre.Key > now.Key) {
                Invoke(mIn, now.Value.Current.gameObject);
                now.Key = now.Value.MoveNext() ? now.Value.Current.GetInstanceID() : -1;
                return;
            }

            // 積集合の処理
            Invoke(mStay, pre.Value.Current.gameObject, now.Value.Current.gameObject);
            pre.Key = pre.Value.MoveNext() ? pre.Value.Current.GetInstanceID() : -1;
            now.Key = now.Value.MoveNext() ? now.Value.Current.GetInstanceID() : -1;
        }

        private void Update () {
            if (mNow.Count != 0) { mNow.Sort(ComparisonID); }

            // 片方が空集合だった場合の処理
            if (mNow.Count == 0) { foreach (var e in mPrev) { Invoke(mOut,e.gameObject); } return; }
            if (mPrev.Count == 0) { foreach (var e in mNow) { Invoke(mIn,e.gameObject); } return; }

            // 処理用Iteratorの準備
            var p = mPrev.GetEnumerator();
            var n = mPrev.GetEnumerator();
            var pre = Pair<int, List<Collider>.Enumerator>.Gen(p.Current.GetInstanceID(), p);
            var now = Pair<int, List<Collider>.Enumerator>.Gen(n.Current.GetInstanceID(), n);

            // IDの若い順に処理
            while (pre.Key > 0 || now.Key > 0) { CheckAndEventcall(pre, now); }

            // 集合更新
            mPrev = mNow;
            mNow = new List<Collider>();
        }
    }
}
