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

    public class WinWebCameraCapture: MonoBehaviour, IWebCoBehaviourm {

        [SerializeField]
        private float mOpacity = 0.9f;

        [SerializeField]
        private MeshPrinter mMeshPrinter = null;

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

        private WinXRCamera mCamera = null;
        private bool mNessClose = false;


        public void Start () {
            mCamera = new WinXRCamera();
            mCamera.SetHoloOpacity(mOpacity);
            Assert.IsNotNull(mMeshPrinter);
        }


        public void CameraWakeup_1 () {
            mCamera.CameraWakeUp(() =>
            {
                mWakeupOk.Invoke();
            });

        }
        public void CameraPhotoMode_2 () {
            mCamera.PhotoMode(mMeshPrinter,
                () => {
                mStartRecModeOk.Invoke();
            });
        }

        public void CameraShot_3 () {
            mCamera.Capture(
                mMeshPrinter.Print,
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

        ~WinWebCameraCapture () {
            if (mNessClose) { return; }
            mCamera.CameraRelease(() => { mEndRecEvent.Invoke(); });
        }

    }

}