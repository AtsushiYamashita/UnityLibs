namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.Events;

    public interface IWebCoBehaviourm {
        void Start ();
        void CameraWakeup_1 ();
        void CameraPhotoMode_2 ();
        void CameraShot_3 ();
        void CameraEnd_4 ();
    }

    public class CamCapOperator: MonoBehaviour, IWebCoBehaviourm {

        [SerializeField]
        private float mOpacity = 0.9f;

        [SerializeField]
        private string mTo = "";
        [SerializeField]
        private string mAs = "";

        private const string DATA_MSG_AS = "MessageTransporter";

        [SerializeField]
        private UnityEvent mWakeupOk = new UnityEvent();
        [SerializeField]
        private UnityEvent mStartRecModeOk = new UnityEvent();
        [SerializeField]
        private UnityEvent mShotStart = new UnityEvent();
        [SerializeField]
        private UnityEvent mShotEnd = new UnityEvent();
        [SerializeField]
        private UnityEvent mEndRecEvent = new UnityEvent();

        private CaptureCam mCamera = null;
        private bool mNessClose = false;

        public void Start () {
            mCamera = new CaptureCam().Set(mTo,mAs);
            mCamera.SetHoloOpacity(mOpacity);
            Assert.IsTrue(mTo.Length > 1);
        }

        public void Reset() {
            mTo = name;
            mAs = DATA_MSG_AS;
        }

        public void CameraWakeup_1 () {
            mCamera.CameraWakeUp(() =>
            {
                mWakeupOk.Invoke();
            });

        }
        public void CameraPhotoMode_2 () {
            mCamera.PhotoMode(
                () => {
                mStartRecModeOk.Invoke();
            });
        }

        public void CameraShot_3 () {
            mCamera.Capture(
                () =>
                {
                    Debug.Log("shot");
                    mShotStart.Invoke();
                },
                () =>
                {
                    mNessClose = true;
                    mShotEnd.Invoke();
                });
        }

        public void CameraEnd_4 () {
            mCamera.CameraRelease(() =>
            {
                mEndRecEvent.Invoke();
            });
        }

        ~CamCapOperator () {
            if (mNessClose) { return; }
            mCamera.CameraRelease(() => { mEndRecEvent.Invoke(); });
        }

    }

}