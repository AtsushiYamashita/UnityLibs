namespace BasicExtends {
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public interface ITransformRecorder {
        /// <summary>
        /// 記録時間の変更
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        TimeWhileEventCaller SetRecLength ( int length );
        TimeWhileEventCaller Push ();
        List<TimedTrfm> Get ();
        Trfm Delta ( int a, int b );
        Trfm DeltaAvarage ( int a, int b );
    }

    [System.Serializable]
    public class TransformRecord {

        [SerializeField]
        private List<TimedTrfm> mRecords = new List<TimedTrfm>();

        [SerializeField]
        private int mLength = 6;

        public Trfm GetRecent () {
            return mRecords.Last().Value;
        }

        public TransformRecord SetRecLength ( int length ) {
            mLength = length;
            return this;
        }

        private Trfm Generate ( Transform transform ) {
            var data = new TimedTrfm().Set(Time.time, Trfm.Convert(transform));
            mRecords.Add(data);
            return data.Value;
        }

        private Trfm Reuse ( Transform transform ) {
            var t = mRecords [0];
            mRecords.RemoveAt(0);
            var pair = t.Set(Time.time, t.Value.Set(transform));
            mRecords.Add(t);
            return pair.Value;
        }


        public Trfm Push ( Transform transform ) {
            if (mRecords.Count < mLength) {
                return Generate(transform);
            }
            return Reuse(transform);
        }

        public List<TimedTrfm> GetRecList () { return mRecords; }

        public int GetRecSize () { return mLength; }

        private void Switch ( ref int a, ref int b ) {
            var t = a; a = b; b = t;
        }

        public TimedTrfm Delta ( int a, int b ) {
            var ret = new TimedTrfm();
            if (a == b) {
                return ret.Set(0, new Trfm());
            }
            if (a > b) {
                Switch(ref a, ref b);
            }
            var _a = mRecords [a];
            var _b = mRecords [b];
            var delta = _b.Key - _a.Key;
            var d = Trfm.Add(_b.Value, Trfm.Multiple(_a.Value, -1));
            return ret.Set(delta, d);
        }

        public Trfm DeltaAvarage ( int a, int b ) {
            var deltaTimedTrfm = Delta(a, b);
            var dv = deltaTimedTrfm.Value;
            return Trfm.Multiple(dv, 1 / deltaTimedTrfm.Key);
        }
    }

    /// <summary>
    /// 時間とTrfmをセットで扱う
    /// </summary>
    [System.Serializable]
    public class TimedTrfm: Pair<float, Trfm> {

        /// <summary>
        /// 記録した時間が現在から何秒前かを返す
        /// </summary>
        /// <returns></returns>
        public float RelativeSecond () {
            var memorized = Key;
            return memorized - Time.time;
        }

        public new TimedTrfm Set ( float f, Trfm trfm ) {
            base.Set(f, trfm);
            return this;
        }
    }
}