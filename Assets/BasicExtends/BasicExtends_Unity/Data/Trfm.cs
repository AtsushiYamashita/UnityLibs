namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    interface ITrfm {
        Vec3 POS { set; get; }
        Vec3 ROT { set; get; }
        Vec3 SCA { set; get; }
        void Convert ( ref Transform tr );
        Trfm Set ( Transform tf );
    }

    [Serializable]
    public class Trfm: IJsonable, ITrfm, IFromJsonNode {
        private const int cPi = 0, cRi = 1, cSi = 2;
        [SerializeField]
        public Vec3 [] mVal = new Vec3 [3];
        public Vec3 POS
        {
            set { mVal [cPi] = value; }
            get { return mVal [cPi]; }
        }
        public Vec3 ROT
        {
            set { mVal [cRi] = value; }
            get { return mVal [cRi]; }
        }
        public Vec3 SCA
        {
            set { mVal [cSi] = value; }
            get { return mVal [cSi]; }
        }
        public static Trfm Add ( Trfm a, Trfm b ) {
            var ret = new Trfm {
                POS = Vec3.Add(a.POS, b.POS),
                ROT = Vec3.Add(a.ROT, b.ROT),
                SCA = Vec3.Add(a.SCA, b.SCA),
            };
            return ret;
        }

        public static Trfm Multiple ( Trfm a, float num ) {
            var ret = new Trfm {
                POS = Vec3.Multiple(a.POS, num),
                ROT = Vec3.Multiple(a.ROT, num),
                SCA = Vec3.Multiple(a.SCA, num),
            };
            return ret;
        }
        public static Trfm Convert ( Transform tr ) {
            return new Trfm().Set(tr);
        }

        public void Convert ( ref Transform tr ) {
            tr.localPosition = POS.Convert();
            tr.localEulerAngles = ROT.Convert();
            tr.localScale = SCA.Convert();
        }

        public Trfm Set ( Transform tf ) {
            POS = Vec3.Convert(tf.localPosition);
            ROT = Vec3.Convert(tf.localEulerAngles);
            SCA = Vec3.Convert(tf.localScale);
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
        public override string ToString () {
            return ToJson();
        }

        public void FromJson ( JsonNode node ) {
            POS = node ["pos"].Parse<Vec3>();
            ROT = node ["rot"].Parse<Vec3>();
            SCA = node ["sca"].Parse<Vec3>();
        }
    }
}