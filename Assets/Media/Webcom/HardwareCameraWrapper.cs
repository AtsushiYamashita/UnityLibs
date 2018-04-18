using System;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

namespace BasicExtends {

    public interface HarwareCamera {
        int Width { get; }
        int Heignt { get; }
        Resolution GetResolution ();
    }

    public class HardwareCameraWrapper {
        private Resolution mResolution;

        public int Width { get { return GetResolution().width; } }
        public int Heignt { get { return GetResolution().height; } }

        private Func<Resolution, int> mOrderRule = ( res ) =>
        {
            // Debug.LogFormat("res({0},{1})", res.width, res.height);
            return res.width * res.height;
        };

        private Func<Resolution, bool> mWhereRule = ( res ) =>
        {
            return res.height > 300.0f;
        };

        public Resolution GetResolution () {
            if (mResolution.height < 1) {
                mResolution = PhotoCapture.SupportedResolutions.
                            OrderByDescending(mOrderRule).Where(mWhereRule).Reverse().First();
            }
            Debug.LogFormat("Res({0},{1})", mResolution.width, mResolution.height);
            return mResolution;
        }
    }
}
