namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public enum KeyEventTiming {
        NonInput, KeyDown, Inputting, KeyUp
    }

    [Serializable]
    public class KEvent {
        [SerializeField]
        private KeyCode mTarget = KeyCode.None;
        [SerializeField]
        private KeyEventTiming mTargetEvent = KeyEventTiming.KeyDown;
        [SerializeField]
        private UnityEvent mEvent = new UnityEvent();

        private int mStatePrev = 0;
        private static readonly KeyEventTiming [] [] mTimingMap 
            = new KeyEventTiming [] [] {
            new KeyEventTiming [] { KeyEventTiming.NonInput, KeyEventTiming.KeyDown },
            new KeyEventTiming [] { KeyEventTiming.Inputting, KeyEventTiming.KeyUp }
        };

        public void Check () {
            var state = Input.GetKey(mTarget) ? 1 : 0;
            var call = mTimingMap [mStatePrev] [state];
            if (mTargetEvent != call) { return; }
            mEvent.Invoke();
        }
    }

    [Serializable]
    public class KeyEvent: MonoBehaviour {

        [SerializeField]
        private List<KEvent> mEventList = new List<KEvent>();

        void Update () {
            foreach(var e in mEventList) {
                e.Check();
            }
        }
    }
}


