using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.XR.WSA.WebCam;


namespace BasicExtends {

    [RequireComponent(typeof(Renderer))]
    public class MeshPrinter: MonoBehaviour {

        [SerializeField]
        private float mScale = 1f;
        Renderer mRender = null;
        private Texture2D mTexture = null;
        private bool mSetupped = false;


        private void Start () {
            mRender = GetComponent<Renderer>();
            Assert.IsNotNull(mRender);
            mRender.material = new Material(Shader.Find("Unlit/Texture"));

            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("As", GetType().Name)) { return; }
                if (msg.Match("act", "Setup")) {
                    var w = int.Parse(msg.TryGet("w"));
                    var h = int.Parse(msg.TryGet("h"));
                    Setup(w, h);
                    return;
                }
            });
        }

        public void Setup ( int w, int h ) {
            ScaleFit(w, h);
            mTexture=CreateTexture(w, h);
            mRender.material.mainTexture = mTexture;
            Assert.IsNotNull(mTexture);
            mSetupped = true;
        }


        private Texture2D CreateTexture ( int w, int h ) {
            if (mTexture != null) { return mTexture; }
            var tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.Apply();
            return tex;
        }


        private static float Aspect ( float width, float height ) {
            return height / width;
        }


        private void ScaleFit ( int w, int h ) {
            float aspect = Aspect(w, h);
            mRender.transform.localScale
                = new Vector3(-mScale, aspect * mScale, 1f);
        }


        public void Print (PhotoCaptureFrame captured) {
            if (mSetupped == false) { throw new System.Exception("Not setuped yet.");  }

            // ターゲットテクスチャに生画像データをコピーします
            captured.UploadImageDataToTexture(mTexture);
            mRender.material.SetTexture("_MainTex", mTexture);
        }
    }

}