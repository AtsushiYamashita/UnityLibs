using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.WSA.WebCam;

namespace BasicExtends {

    public interface IXrCamera {
        Texture2D GetTexture ();
        void Capture ( Action start = null, Action end = null );
        void CameraWakeUp ( Action action );
        void PhotoMode ( Action start );
        void CameraRelease ( Action action );
        void SetHoloOpacity ( float opacity );
    }

    /// <summary>
    /// Unityのオブジェクトと一緒にWinXRのWebCam画像を取得し、
    /// </summary>
    public class WinXRCamera :IXrCamera{
        private static CameraParameters mCameraParam;
        private static HardwareCameraWrapper mResolution = new HardwareCameraWrapper();
        private PhotoCapture mCameraInstance = null;
        private Texture2D mTexture = null;
        private bool mRecMode = false;
        private bool mPhotoWakeupping = false;
        private float mOpacity = 0.9f;

        public WinXRCamera () {
            Assert.IsNotNull(mResolution);
            CreateTexture(mResolution.Width, mResolution.Heignt);
            Assert.IsNotNull(mTexture);
        }

        public Texture2D GetTexture () {
            return mTexture;
        }

        public void Capture ( Action start = null, Action end = null ) {
            Debug.LogFormat("Capture => {0}", "call 1");
            if (mCameraInstance == null || mRecMode == false) {
                Debug.LogFormat("Capture => {0}", "call -1");
                CameraWakeUp(() => { PhotoMode(() => { Capture(); }); });
                return;
            }

            Debug.LogFormat("Capture => {0}", "call 2");

            mCameraInstance.TakePhotoAsync(( result, captured ) =>
            {
                Debug.LogFormat("Capture => {0}", result.success ? "OK" : "filed");
                if (result.success == false) { return; }

                if (start != null) { start.Invoke(); }

                // ターゲットテクスチャに生画像データをコピーします
                captured.UploadImageDataToTexture(mTexture);
                if (end != null) { end.Invoke(); }
            });
        }

        public void CameraWakeUp ( Action action ) {
            // カメラ操作用のインスタンスの作成
            if (mCameraInstance != null) { return; }
            if (mPhotoWakeupping == true) { return; }
            mPhotoWakeupping = true;
            Debug.LogFormat("wake up...");
            PhotoCapture.CreateAsync(true, ( PhotoCapture cameraInstance ) =>
            {
                mCameraInstance = cameraInstance;
                mPhotoWakeupping = false;
                Debug.LogFormat("wake up OK");
                action();
            });
        }

        public void PhotoMode ( Action start ) {
            if (mCameraInstance == null) { return; }
            if (mRecMode == true) { return; }
            Debug.LogFormat("photo mode...");
            CameraParameters c_param = InitCameraParams(mResolution.GetResolution(), mOpacity);
            mCameraInstance.StartPhotoModeAsync(c_param, ( result ) =>
            {
                mRecMode = result.success;
                Debug.LogFormat("Start photo mode");
                if (result.success == false) {
                    Debug.LogFormat("Start photo mode => faild");
                    return;
                }
                start();
            });
        }

        public void CameraRelease ( Action action ) {
            // カメラを非アクティベート化します
            if (mCameraInstance == null) { return; }
            if (mRecMode == false) { return; }
            Debug.LogFormat("CameraRelease");
            mCameraInstance.StopPhotoModeAsync(( result ) =>
            {
                // photo capture のリソースをシャットダウンします
                mCameraInstance.Dispose();
                mCameraInstance = null;
                mRecMode = false;
                Debug.LogFormat("CameraRelease end");
                action();
            });
        }

        ~WinXRCamera () {
            CameraRelease(() => { });
        }

        private void CreateTexture ( int w, int h ) {
            if (mTexture != null) { return; }
            var tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            //var tex = new Texture2D(w, h);
            tex.filterMode = FilterMode.Bilinear;
            tex.Apply();
            mTexture = tex;
        }

        /// <summary>
        /// 画面合成における仮想オブジェクトの非透明度
        /// この値が大きいほど色が濃くなる。
        /// </summary>
        /// <param name="opacity"></param>
        public void SetHoloOpacity ( float opacity ) {
            mOpacity = opacity;
        }

        private static CameraParameters InitCameraParams ( Resolution res, float opacity ) {
            if (mCameraParam.cameraResolutionWidth == res.width) { return mCameraParam; }
            mCameraParam = new CameraParameters();
            mCameraParam.hologramOpacity = opacity; // 不透明度
            mCameraParam.cameraResolutionWidth = res.width;
            mCameraParam.cameraResolutionHeight = res.height;
            mCameraParam.pixelFormat = CapturePixelFormat.BGRA32;
            return mCameraParam;
        }
    }


}