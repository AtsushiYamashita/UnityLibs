namespace BasicExtends.Scenario {
    using UnityEngine;
    using UnityEngine.Events;

    public enum ProcessState {
        Waiting, Start, Update, End, Ended
    }

    /// <summary>
    /// プロセスをシナリオに見立てて、
    /// ステートマシンでシナリオを作る
    /// </summary>
    public class ScenarioProcess: MonoBehaviour, IFactoryReceiver {
        [SerializeField]
        private ProcessState mState = ProcessState.Waiting;

        public ProcessState State {
            set { mState = value; Debug.Log(value); }
            get { return mState; } }

        [SerializeField]
        private UnityEvent mSkip = new UnityEvent();

        [SerializeField]
        private UnityEvent mStart = new UnityEvent();

        [SerializeField]
        private UnityEvent mUpdate = new UnityEvent();

        [SerializeField]
        private UnityEvent mEnd = new UnityEvent();

        protected string TypeName { get; set; }
        protected IFactory mFactory = null;


        private string [] StateNames = System.Enum.GetNames(typeof(ProcessState));
        private ProcessState [] States =(ProcessState []) System.Enum.GetValues(typeof(ProcessState));

        public void SetStart () {
            SetState("start");
            gameObject.SetActive(true);
        }

        public void SetState(string state ) {
            for(int i = 0;i < States.Length; i++) {
                if (StateNames [i].ToUpper() != state.ToUpper()) { continue; }
                State = States [i];
                return;
            }
            throw new System.Exception("You had call state = " + state);
        }

        protected virtual void Init () {}

        private void Start () {
            mFactory = mFactory ?? new TokenbaseParser();
            mFactory.SetTarget(this);
            Messenger.Assign(msg =>
            {
                if (msg.Unmatch("To", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Skip")) {
                    Skip();
                    return;
                }
                if (msg.Match("act", "SetScript")) {
                    mFactory.SetScript(msg.TryGet("script"));
                    return;
                }
            });
            Init();
        }

        private void Update () {
            switch (mState) {
                case ProcessState.Waiting: return;
                case ProcessState.Start: ProcessStart(); return;
                case ProcessState.Update: ProcessUpdate(); return;
                case ProcessState.End: ProcessEnd(); return;
                case ProcessState.Ended: return;
            }
        }

        protected virtual void ProcessStart () {
            mState = ProcessState.Update;
            mStart.Invoke();
        }

        protected void ProcessUpdate () {
            mUpdate.Invoke();
        }

        protected void ProcessEnd () {
            mState = ProcessState.Ended;
            gameObject.SetActive(false);
            mEnd.Invoke();
        }

        public virtual void Skip () {
            gameObject.SetActive(false);
            mState = ProcessState.Ended;
            Debug.Log("Process call skip " + name);
            mSkip.Invoke();
        }

        public void DebugLog(string str ) {
            var msg = (str.Length < 1 ? " " : " => " + str);
            Debug.LogFormat("({0}:{1}){2}", name, mState.ToString(), msg);
        }

        void IFactoryReceiver.InstanceSet<T> ( T t ) {
            throw new System.NotImplementedException();
        }
    }
}
