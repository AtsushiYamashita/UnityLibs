namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    interface ITrfm {
        Vec3 POS { set; get; }
        Vec3 ROT { set; get; }
        Vec3 SCA { set; get; }
        void Convert ( ref Transform tr, Trfm.Type type );
        Trfm Set ( Transform tf, Trfm.Type type );
    }

    [Serializable]
    public class Trfm: IJsonable, ITrfm, IFromJsonNode {

        public enum Type { World, Local }
        public Type DataType { set; get; }
        private const int cPi = 0, cRi = 1, cSi = 2;
        [SerializeField]
        public Vec3 [] mVal = new Vec3 [3];

        public Trfm () {
            Serializer.SetDatatype(Serializer.SerialType.Binary);
            Serializer.AssignSerializer(GetType().Name, Serial);
            Serializer.AssignDeserializer(GetType().Name, Deserial);
        }

        public static ByteList Serial ( object obj ) {
            var self = obj as Trfm;
            var bytes = ByteList.Zero;
            for (int i = 0; i < 3; i++) {
                var _bytes = Serializer.Serialize(self.mVal [i]);
                bytes.Add(_bytes.Count).Add(_bytes);
            }
            bytes.WriteCountToHead();

            return bytes;
        }

        public static CheckedRet<object> Deserial ( ByteList bytes ) {
            var size = bytes.DropInt32();
            if (bytes.Count != size) {
                return CheckedRet<object>.Fail();
            }
            var ret = new Trfm();
            for (int i = 0; i < 3; i++) {
                var _size = bytes.DropInt32();
                var _bytes = bytes.DropRange(0, _size);
                var _obj = Serializer.Deserialize<Vec3>(_bytes);
                if (_obj.Key == false) { return CheckedRet<object>.Fail(); }
                ret.mVal [i] = _obj.Value;
            }

            // 小数点に関わるため、ハッシュチェックはしない（できない）
            return CheckedRet<object>.Gen(true, ret);
        }

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

        public static Trfm Convert ( Transform tr, Type type ) {
            return new Trfm().Set(tr, type);
        }
        public static Trfm ConvertWorld ( Transform tr ) {
            return new Trfm().SetWorld(tr);
        }

        public void Convert ( ref Transform tr, Type type ) {
            if (type == Type.Local) {
                tr.localPosition = POS.Convert();
                tr.localEulerAngles = ROT.Convert();
                tr.localScale = SCA.Convert();

            } else {
                tr.position = POS.Convert();
                tr.eulerAngles = ROT.Convert();
                tr.localScale = SCA.Convert();
            }
        }

        public Trfm Set ( Transform tf, Type type ) {
            if (type == Type.Local) {
                POS = Vec3.Convert(tf.localPosition);
                ROT = Vec3.Convert(tf.localEulerAngles);
                SCA = Vec3.Convert(tf.localScale);
            } else {
                POS = Vec3.Convert(tf.position);
                ROT = Vec3.Convert(tf.eulerAngles);
                SCA = Vec3.Convert(tf.localScale);
            }
            return this;
        }

        public Trfm SetWorld ( Transform tf ) {
            POS = Vec3.Convert(tf.position);
            ROT = Vec3.Convert(tf.eulerAngles);
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

        public override bool Equals ( object obj ) {
            var trfm = obj as Trfm;
            if (trfm.IsNull()) { return false; }
            return this.GetHashCode() == trfm.GetHashCode();
        }

        public override int GetHashCode () {
            var hashCode = 1368377165;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vec3>.Default.GetHashCode(POS);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vec3>.Default.GetHashCode(ROT);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vec3>.Default.GetHashCode(SCA);
            return hashCode;
        }

        public static bool operator == ( Trfm trfm1, Trfm trfm2 ) {
            return EqualityComparer<Trfm>.Default.Equals(trfm1, trfm2);
        }
        public static bool operator != ( Trfm trfm1, Trfm trfm2 ) {
            return !(trfm1 == trfm2);
        }
    }
}