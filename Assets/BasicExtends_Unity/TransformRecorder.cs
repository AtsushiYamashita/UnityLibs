namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public static class ListExtends_4_TransformRecorder {
        public static List<TransformRecorder.TimedData> Add (
            this List<TransformRecorder.TimedData> list,
            Pair<DateTime, TransformRecorder.Trfm> data ) {
            list.Add(data as TransformRecorder.TimedData);
            return list;
        }
    }

    /*
    class TransformRecorder{
      + static TForm Convert ( Transform tf );
      + TransformRecorder SetRecLength (int length);
      + TransformRecorder Push ();
      + List<TimedData> Get ( );
      + TForm Delta(int a,int b );
      + Trfm DeltaAvarage ( int a, int b )
    }
    */
    public class TransformRecorder: MonoBehaviour {
        public class TimedData: Pair<DateTime, Trfm> { }

        /*
        class TForm {
           public Vector3 [] mVal;
           public Vector3 POS { set ; get ; }
           public Vector3 ROT { set ; get ; }
           public Vector3 SCA{ set ; get ; } 
           public static TForm Delta ( TForm a, TForm b);
           public void Multiple(float num ) ;
           public string ToJson () ;
        }
        */
        public class Trfm {
            private int mPi = 0, mRi = 1, mSi = 2;
            public Vector3 [] mVal = new Vector3 [3];
            public Vector3 POS
            {
                set { mVal [mPi] = value; }
                get { return mVal [mPi]; }
            }
            public Vector3 ROT
            {
                set { mVal [mRi] = value; }
                get { return mVal [mRi]; }
            }
            public Vector3 SCA
            {
                set { mVal [mSi] = value; }
                get { return mVal [mSi]; }
            }
            public Trfm Add ( Trfm a ) {
                POS += a.POS;
                ROT += a.ROT;
                SCA += a.SCA;
                return this;
            }
            public Trfm Multiple ( float num ) {
                POS *= num;
                ROT *= num;
                SCA *= num;
                return this;
            }
            public string ToJson () {
                var dic = new Dictionary<string, string> {
                    { "pos",POS.ToJson() },
                    { "rot",ROT.ToJson() },
                    { "sca",SCA.ToJson() }
                };
                return dic.ToJson();
            }
        }

        public static Trfm Convert ( Transform tf ) {
            var data = new Trfm();
            var pos = tf.localPosition;
            var rot = tf.localRotation.eulerAngles;
            var sca = tf.localScale;
            data.POS.Set(pos.x, pos.y, pos.z);
            data.ROT.Set(rot.x, rot.y, rot.z);
            data.SCA.Set(sca.x, sca.y, sca.z);
            return data;
        }

        private List<TimedData> mRecords = new List<TimedData>();
        private int mLength = 5;

        public TransformRecorder SetRecLength ( int length ) {
            mLength = length;
            return this;
        }

        public TransformRecorder Push () {
            if (mRecords.Count >= mLength) {
                mRecords.RemoveAt(0);
            }
            mRecords.Add(new TimedData().Set(
                DateTime.Now, Convert(transform)));
            return this;
        }

        public List<TimedData> Get () { return mRecords; }

        private void Switch ( ref int a, ref int b ) {
            var t = a;
            a = b;
            b = a;
        }

        public Pair<int, Trfm> Delta ( int a, int b ) {
            var size = a - b;
            var ret = new Pair<int, Trfm>();
            if (size == 0) {
                return ret.Set(0, new Trfm());
            }
            if (size < 0) {
                Switch(ref a, ref b);
                size = a - b;
            }
            var _a = mRecords [a];
            var _b = mRecords [b];
            var delta = _a.GetKey() - _b.GetKey();
            var d = _a.GetValue().Add(_b.GetValue().Multiple(-1));
            return ret.Set(size, d);
        }

        public Trfm DeltaAvarage ( int a, int b ) {
            var d = Delta(a, b);
            var dv = d.GetValue();
            dv.Multiple(1 / d.GetKey());
            return dv;
        }
    }
}

