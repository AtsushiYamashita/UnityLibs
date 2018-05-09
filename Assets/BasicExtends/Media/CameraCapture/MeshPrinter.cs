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
                //Debug.Log(msg.ToJson());
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("As", GetType().Name)) { return; }
                if (msg.Match("act", "Setup")) {
                    var w = int.Parse(msg.TryGet("w"));
                    var h = int.Parse(msg.TryGet("h"));
                    Setup(w, h);
                    return;
                }
                if (msg.Match("act", "Print")) {
                    Debug.Log("msg.Match(act, Print))");
                    Print(msg.TryObjectGet<List<byte>>().ToArray());
                    return;
                }
                if (msg.Match("act", "Print2")) {
                    Debug.Log("msg.Match(act, Print))");
                    Print(int.Parse(msg.TryGet("id")),
                        int.Parse(msg.TryGet("splited")),
                        int.Parse(msg.TryGet("packet_size")), 
                        msg.TryObjectGet<List<byte>>().ToArray());
                    return;
                }
            });
        }

        public void Setup ( int w, int h ) {
            ScaleFit(w, h);
            mTexture=CreateTexture(w, h);
            mRender.material.mainTexture = mTexture;
            Assert.IsNotNull(mTexture);
            mColor = new Color [w * h];
            for (int i = 0;i < mColor.Length; i++) {
                mColor[i] = new Color(0,0,0);
            }
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
            if (mSetupped == false) {
                throw new System.Exception("Not setuped yet.");  }


            List<byte> buf = new List<byte>();
            captured.CopyRawImageDataIntoBuffer(buf);

            var b = Serializer.Serialize(
                new Pair<string, byte []>().Set("てすと", buf.ToArray()));
            Print(b.ToArray());
        }


        public void Print(byte[] argb_bytes ) {

            //var bd = BinarySerial.Deserialize<Pair<string, byte []>>(argb_bytes);
            //Debug.Log("bd" + bd.Key);
            //argb_bytes = bd.Value;

            int stride = 4;
            float denominator = 1.0f / 255.0f;
            int skip = 0;
            Color preC = new Color(0,0,0,0);

            List<Color> colorArray = new List<Color>();
            for (int i = argb_bytes.Length - 1; i >= 0; i -= stride) {
                if (skip++ % 4 != 0) {
                    colorArray.Add(preC);
                    continue;
                }

                // char added = (char) argb_bytes [i - 0];
                float a = 0;
                float r = (int) (argb_bytes [i - 1]) * denominator;
                float g = (int) (argb_bytes [i - 2]) * denominator;
                float b = (int) (argb_bytes [i - 3]) * denominator;
                preC = new Color(r, g, b, a);
                colorArray.Add(preC);
            }

            mTexture.SetPixels(colorArray.ToArray());
            mTexture.Apply();

            mRender.material.SetTexture("_MainTex", mTexture);
        }

        Color [] mColor ;

        public void Print (int id, int splited,int packet_size, byte [] argb_bytes ) {

            //var bd = BinarySerial.Deserialize<Pair<string, byte []>>(argb_bytes);
            //Debug.Log("bd" + bd.Key);
            //argb_bytes = bd.Value;

            int stride = 4;
            float denominator = 1.0f / 255.0f;

            for (int i = id * packet_size, p = id * packet_size/4; i < argb_bytes.Length; i += stride,p++) {
                // char added = (char) argb_bytes [i - 0];
                mColor [p].b = (int) (argb_bytes [i + 0]) * denominator;
                mColor [p].g = (int) (argb_bytes [i + 1]) * denominator;
                mColor [p].r = (int) (argb_bytes [i + 2]) * denominator;
                mColor [p].a = 0;
            }
            Msg.Gen().To("Debug").Act("log")
                .Set("id", id)
                .Set("splited", splited)
                .Set("packet_size", packet_size)
                .Set("mColor[id * packet_size/4].r", mColor [id * packet_size / 4].r)
                .Pool();

            mTexture.SetPixels(mColor);
            mTexture.Apply();

            mRender.material.SetTexture("_MainTex", mTexture);
        }
    }
}
