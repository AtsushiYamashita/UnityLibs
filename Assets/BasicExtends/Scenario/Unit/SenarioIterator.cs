namespace BasicExtends.Scenario {
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.Events;
    using BasicExtends;

    public interface ICanSkip {
        void Skip ();
    }

    public class SenarioIterator<T>: MonoBehaviour, ICanSkip, IFactoryReceiver where T : MonoBehaviour {

        // 0番をActiveなレイヤーとした疑似スタック構造を保持する
        [SerializeField]
        protected StackLikeList<T> mStack = new StackLikeList<T>();

        [SerializeField]
        private UnityEvent mChangeStart = new UnityEvent();

        [SerializeField]
        private UnityEvent mChangeEnd = new UnityEvent();

        [SerializeField]
        private UnityEvent mUnitSkip = new UnityEvent();

        protected string TypeName { get; set; }
        protected IFactory mFactory = null;

        public void InstanceSet ( T t ) {
            PushHead(t);
        }

        private void ChangeMessage (string f, string t) {
            var str = string.Format("Change {0} -> {1}", f, t);
            Msg.Gen().To("Debug").Act("Log").Set("msg",str).Push();
        }

        public void PushHead ( T unit ) {
            mChangeStart.Invoke();
            Assert.IsNotNull(unit);
            var old = mStack.Head.IsNull() ? "---" : mStack.Head.name;
            ChangeMessage(old, unit.name);

            unit.gameObject.SetActive(true);
            unit.transform.parent = transform;

            mStack.ForEach(e => e.gameObject.SetActive(false));
            mStack.Push(unit);
        }

        public void Remove () {
            mChangeStart.Invoke();

            var t = mStack.Pop();
            Assert.IsTrue(t.Key,"取得に失敗しました");
            var obj = t.Value.gameObject;
            ChangeMessage(obj.name, mStack.Head.name);

            if (mStack.Head == null) {
                return;
            }
            mStack.Head.gameObject.SetActive(true);
            mChangeEnd.Invoke();
        }

        public void Skip () {
            while (mStack.Head != null && mStack.Count > 0) {
                var t = mStack.Pop();
                var itr = t as ICanSkip;
                if (itr.IsNotNull()) { itr.Skip(); }
                Assert.IsTrue(t.Key, "取得に失敗しました");
                var obj = t.Value.gameObject;
            }
            Msg.Gen().To("Debug").Act("Log").Set("msg",name + " is skiped.").Push();
            gameObject.SetActive(false);
            mUnitSkip.Invoke();
        }

        protected virtual void StartProcess () { }

        private void Start () {
            mFactory = mFactory ?? new TokenbaseParser<T>();
            mFactory.SetTarget(this);
            AssignChildren();
            StartProcess();
            Messenger.Assign(msg =>
            {
                if (msg.Unmatch("To", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "PushHead")) {
                    PushHead(msg.TryObjectGet<T>());
                    return;
                }
                if (msg.Match("act", "remove")) {
                    Remove();
                    return;
                }
                if (msg.Match("act", "Skip")) {
                    throw new System.NotImplementedException();
                }
                if (msg.Match("act", "SetScript")) {
                    mFactory.SetScript(msg.TryGet("script"));
                    return;
                }
            });
        }

        /// <summary>
        /// AssignChildrenの中で
        /// Activeな子が見つかったら
        /// そこまでスキップするという処理
        /// </summary>
        private void SkipOnStart (GameObject obj) {
            if (obj.gameObject.activeSelf) { return; }
            while ( mStack.Count > 0) {
                var target = mStack.Pop();
                if (target.Check == false) { continue; }
                var skip = target.Value as ICanSkip;
                if (skip.IsNotNull()) { skip.Skip(); }
            }
        }

        /// <summary>
        /// 自分の配下にあるGameObjectから、
        /// Tに該当するコンポネントを持ったものを抽出し、
        /// 上から順に実行する
        /// </summary>
        private void AssignChildren () {
            var children = gameObject.Children();
            children.Reverse();
            var active = 0;
            foreach(var c in children) {
                var t = c.GetComponent<T>();
                if(t == null) { continue; }
                SkipOnStart(t.gameObject);
                mStack.Push(t);
                active += t.gameObject.activeSelf ? 1 : 0;
            }

            if(mStack.Count <= 0) {
                throw new System.Exception("イテレーションの対象がありません");
            }

            // アクティブな進行対象がなければ、一番上のオブジェクトを有効化する。
            if (active == 0 ) {
                PushHead(mStack.Pop().Value);
                active++;
            }
            Assert.IsTrue(active == 1, "複数の対象が有効化されています");
        }

        /// <summary>
        /// seal
        /// </summary>
        void IFactoryReceiver.InstanceSet<T1> ( T1 t ) {
            throw new System.NotImplementedException();
        }
    }
}