namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Events;

    public class TimeWhileEventCaller: MonoBehaviour {
        [SerializeField]
        private int mSpan = 6;

        [SerializeField]
        private int mCount = 0;

        [SerializeField]
        private UnityEvent mRec = new UnityEvent();

        private void Update () {
            if (mCount == 0) {
                mRec.Invoke();
            }
            mCount = (mCount + 1) % mSpan; // [0 - mSpan]
        }

        public void AddEvent ( UnityAction action ) {
            mRec.AddListener(action);
        }

        public int GetSpan () {
            return mSpan;
        }
    }
}

