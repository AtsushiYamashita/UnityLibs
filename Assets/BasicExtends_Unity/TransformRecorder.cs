namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

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
    [Serializable]
    public class Trfm: IJsonable {
        private readonly int mPi = 0, mRi = 1, mSi = 2;
        [SerializeField]
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
        public static Trfm Add ( Trfm a, Trfm b ) {
            var ret = new Trfm {
                POS = a.POS + b.POS,
                ROT = a.ROT + b.ROT,
                SCA = a.SCA + b.SCA
            };
            return ret;
        }

        public static Trfm Multiple ( Trfm a, float num ) {
            var ret = new Trfm {
                POS = a.POS * num,
                ROT = a.ROT * num,
                SCA = a.SCA * num
            };
            return ret;
        }
        public string ToJson () {
            var dic = new Dictionary<string, string> {
                    { "pos",POS.ToJson() },
                    { "rot",ROT.ToJson() },
                    { "sca",SCA.ToJson() }
                };
            return dic.ToJson();
        }
        public override string ToString () {
            return ToJson();
        }

        public static Trfm Convert ( Transform tr ) {
            return new Trfm().Set(tr);
        }

        public Trfm Set ( Transform tf ) {
            POS = tf.localPosition;
            ROT = tf.localRotation.eulerAngles;
            SCA = tf.localScale;
            return this;
        }

        public void Get ( Transform tf ) {
            tf.localPosition = POS;
            tf.localScale = SCA;
            tf.localRotation = Quaternion.Euler(ROT.x, ROT.y, ROT.z);
        }
    }


    [Serializable]
    public class TimedData: Pair<float, Trfm> {
        public float RelativeSecond () {
            var old = Key;
            return old - Time.time;
        }
        public new TimedData Set (float f, Trfm trfm) {
            base.Set(f, trfm);
            return this;
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
    [Serializable]
    public class TransformRecord {

        [SerializeField]
        private List<TimedData> mRecords = new List<TimedData>();
        private int mLength = 5;

        public TransformRecord SetRecLength ( int length ) {
            mLength = length;
            return this;
        }

        private TransformRecord Generate ( Transform transform ) {
            var data = (TimedData) new TimedData().Set(Time.time, Trfm.Convert(transform));
            mRecords.Add(data);
            return this;
        }

        private TransformRecord Reuse ( Transform transform ) {
            var t = mRecords [0];
            mRecords.RemoveAt(0);
            t.Set(Time.time, t.Value.Set(transform));
            mRecords.Add(t);
            return this;
        }


        public TransformRecord Push ( Transform transform ) {
            if (mRecords.Count < mLength) {
                return Generate(transform);
            }
            return Reuse(transform);
        }

        public List<TimedData> GetRecList () { return mRecords; }

        public int GetRecSize () { return mLength; }

        private void Switch ( ref int a, ref int b ) {
            var t = a; a = b; b = t;
        }

        public TimedData Delta ( int a, int b ) {
            var size = a - b;
            var ret = new TimedData();
            if (size == 0) {
                return ret.Set(0, new Trfm());
            }
            if (size < 0) {
                Switch(ref a, ref b);
                size = a - b;
            }
            var _a = mRecords [a];
            var _b = mRecords [b];
            var delta = _a.Key - _b.Key;
            var d = Trfm.Add(_a.Value, Trfm.Multiple(_b.Value, -1));
            return ret.Set(delta, d);
        }

        public Trfm DeltaAvarage ( int a, int b ) {
            var d = Delta(a, b);
            var dv = d.Value;
            return Trfm.Multiple(dv, 1 / d.Key);
        }
    }

    [Serializable]
    public class RecorderEvent: UnityEvent<TransformRecord> { }


    public class TransformRecorder: MonoBehaviour {
        [SerializeField]
        private int mSpan = 3;
        private int mCount = 0;
        private TransformRecord mRecord = new TransformRecord();

        [SerializeField]
        private RecorderEvent mRec = new RecorderEvent();

        private void Update () {
            if (mCount == 0) {
                mRecord.Push(transform);
                mRec.Invoke(mRecord);
            }
            mCount = (mCount + 1) % mSpan; // 0 - mSpan
        }

        public void AddRecEvent ( UnityAction<TransformRecord> action ) {
            mRec.AddListener(action);
        }

        public TransformRecord GetRecord () {
            return mRecord;
        }

        public int GetSpan () {
            return mSpan;
        }
    }
}

