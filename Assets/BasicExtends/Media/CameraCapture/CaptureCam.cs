#pragma warning disable 0429

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.WSA.WebCam;
//using Windows.Media.Devices;
//using System.Runtime.InteropServices;
using OpenCVForUnity;

namespace BasicExtends {

    public interface IXrCamera {
        void Capture ( Action start = null, Action end = null );
        void CameraWakeUp ( Action action );
        void PhotoMode ( Action start );
        void CameraRelease ( Action action );
        void SetHoloOpacity ( float opacity );
    }

    /// <summary>
    /// Unityのオブジェクトと一緒にWinXRのWebCam画像を取得し、
    /// </summary>
    public class CaptureCam: IXrCamera {
        private static CameraParameters mCameraParam;
        private static HardwareCameraWrapper mResolution = new HardwareCameraWrapper();
        private PhotoCapture mCameraInstance = null;
        private bool mRecMode = false;
        private bool mPhotoWakeupping = false;
        private float mOpacity = 0.9f;
        private string mTo = "";
        private string mAs = "";
        private const bool mCompress = false;
        private const int PACKET_SIZE = 600;


        public CaptureCam () {
            Assert.IsNotNull(mResolution);
        }

        public CaptureCam Set ( string to, string as_ ) {
            mTo = to;
            mAs = as_;
            return this;
        }

        /// <summary>
        /// WebCameraとしてカメラ画像をキャプチャする
        /// </summary>
        public void Capture ( Action start = null, Action end = null ) {
            Debug.LogFormat("Capture => {0}", "call 1");
            if (mCameraInstance == null || mRecMode == false) {
                throw new Exception();
            }

            Debug.LogFormat("Capture => {0}", "call 2");
            if (start != null) { start.Invoke(); }


            while (cam.didUpdateThisFrame == false) { System.Threading.Thread.Sleep(1); }

            Debug.LogFormat("{0},{1}", cam.width, cam.height);
            var rgba_mat = new Mat(new Size(cam.width, cam.height), CvType.CV_8UC4);
            Debug.Log(rgba_mat.size());
            var colors = cam.GetPixels32();
            OpenCVForUnity.Utils.webCamTextureToMat(cam, rgba_mat, colors);
            var size = rgba_mat.size();
            var bgra_mat = new Mat(new Size(cam.width, cam.height), CvType.CV_8UC4);
            Imgproc.cvtColor(rgba_mat, bgra_mat, Imgproc.COLOR_BGRA2RGBA);

            var tick = DateTime.Now.Ticks;
            Msg.Gen().To(mTo)
                .As(mAs)
                .Act("Print2")
                .Set("w", "" + size.width)
                .Set("h", "" + size.height)
                .Set("tick", tick)
                .SetObjectData(rgba_mat)
                .UnJsonable().Pool();

            if (end != null) { end.Invoke(); }
        }

        WebCamTexture cam;
        public void CameraWakeUp ( Action action ) {
            // カメラ操作用のインスタンスの作成
            if (mCameraInstance != null) { return; }
            if (mPhotoWakeupping == true) { return; }
            mPhotoWakeupping = true;
            Debug.LogFormat("wake up...");
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
            var device = WebCamTexture.devices [0].name;
            Debug.Log("device : " + device);
            cam = new WebCamTexture(device);
            cam.Play();
            Debug.Log("cam" + cam);
            Debug.Log("cam.dimension" + cam.dimension);

            PhotoCapture.CreateAsync(true, ( PhotoCapture cameraInstance ) =>
            {
                mCameraInstance = cameraInstance;
                mPhotoWakeupping = false;

                //GCHandle campointer = (GCHandle) cameraInstance.GetUnsafePointerToVideoDeviceController();
                //VideoDeviceController vdc = campointer.Target as VideoDeviceController;
                //Debug.Log(vdc.GetType());

                Debug.LogFormat("wake up OK");
                action();
            });
        }

        public void PhotoMode ( Action start ) {
            if (mCameraInstance == null) { return; }
            if (mRecMode == true) { return; }
            Debug.LogFormat("photo mode...");
            var res = mResolution.GetResolution();
            CameraParameters c_param =
                InitCameraParams(res, mOpacity);
            mCameraInstance.StartPhotoModeAsync(c_param, ( result ) =>
            {
                mRecMode = result.success;
                Debug.LogFormat("Start photo mode");
                if (result.success == false) {
                    Debug.LogFormat("Start photo mode => faild");
                    return;
                }

                // テクスチャセットアップのための解像度通知
                Msg.Gen().To(mTo).As(mAs)
                    .Act("Setup")
                    .Set("w", cam.width)
                    .Set("h", cam.height)
                    .Push();
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

        ~CaptureCam () {
            CameraRelease(() => { });
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
