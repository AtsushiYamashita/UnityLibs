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
        private float mTextureScale = 1f;

        [SerializeField]
        private float mOpacity = 0.9f;

        [SerializeField]
        Renderer mRender = null;

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
            Assert.IsNotNull(mRender);
            mCamera = new WinXRCamera();
            mCamera.SetHoloOpacity(mOpacity);
            mRender.material = new Material(Shader.Find("Unlit/Texture"));
            mRender.material.mainTexture = mCamera.GetTexture();
            ScaleFitTexture();
        }

        private static float Aspect ( float width, float height ) {
            return height / width;
        }

        private void ScaleFitTexture () {
            var tex = mRender.material.mainTexture;
            float aspect = Aspect(tex.width, tex.height);
            mRender.transform.localScale
                = new Vector3(-mTextureScale, aspect * mTextureScale, 1f);
            Debug.Log("ScaleFitTexture end");
        }

        public void CameraWakeup_1 () {
            mCamera.CameraWakeUp(() =>
            {
                mWakeupOk.Invoke();
            });

        }
        public void CameraPhotoMode_2 () {
            mCamera.PhotoMode(() =>
            {
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
                    mRender.material.SetTexture("_MainTex", mCamera.GetTexture());
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