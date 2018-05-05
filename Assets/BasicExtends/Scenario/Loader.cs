namespace BasicExtends.Scenario {
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using BasicExtends;

    public interface ILoader {
        void Set ( string name, bool end );
        void ResetProcess ();
        void StartProcess ();
        void AssignLoadingAction ( UnityAction<ILoader> action );
        int GetTodo ();
        int GetDone ();
        float Done { get; }
    }

    public class LoadingEvent: UnityEvent<ILoader> { }

    /// <summary>
    /// 読込を管理するイベント
    /// </summary>
    public class Loader: MonoBehaviour,ILoader {

        [SerializeField]
        UnityEvent mLoadStart = new UnityEvent();
        [SerializeField]
        LoadingEvent mLoading = new LoadingEvent();
        [SerializeField]
        UnityEvent mLoadEnd = new UnityEvent();
        private Pair<float,int> mChecked = Pair<float,int>.Gen(0,0); // time,ocunt

        private Dictionary<string, bool> mStates = new Dictionary<string, bool>();

        public void Set ( string name, bool end ) {
            mStates.TrySet(name, end);
        }

        public void ResetProcess () {
            mChecked = Pair<float, int>.Gen(Time.time, 0);
        }

        public int GetTodo () {
            return mStates.Count;
        }

        public int GetDone () {
            var now = Time.time;
            if (mChecked.Key + 0.1f > now) { return mChecked.Value; }
            mChecked.Key = now;
            mChecked.Value = mStates.Count(e => !e.Value);
            return mChecked.Value;
        }

        public float Done { get { return GetDone() / GetTodo() + 0.001f; } }

        public void StartProcess () {
            ResetProcess();
            mLoadStart.Invoke();
        }

        private void Start () {
            StartProcess();
            Messenger.Assign(msg => {
                if (msg.Unmatch("To", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "set")) {
                    var isDone = msg.TryGet("state").ToUpper() == "True".ToUpper();
                    Set(msg.TryGet("name"),  isDone);
                    return;
                }
                if (msg.Match("act", "reset")) {
                    ResetProcess();
                    return;
                }
                if (msg.Match("act", "AssignLoading")) {
                    // action(done,todo)
                    var cb = msg.TryObjectGet<UnityAction<ILoader>>();
                    AssignLoadingAction(cb);
                    return;
                }
            });
        }

        public void AssignLoadingAction(UnityAction<ILoader> action ) {
            mLoading.AddListener(action);
        }

        private void Update () {
            var done = Done;
            if(done >= 1) {
                mLoadEnd.Invoke();
                enabled = false;
                return;
            }
            mLoading.Invoke(this);
        }
    }
}

