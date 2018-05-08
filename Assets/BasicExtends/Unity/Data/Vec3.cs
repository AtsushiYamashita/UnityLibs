namespace BasicExtends {

    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    interface IVec3 {
        float X { set; get; }
        float Y { set; get; }
        float Z { set; get; }
        Vec3 Set ( params float [] fs );
        Vec3 Set ( Vec3 fs );
        Vector3 Convert ();
    }

    [System.Serializable]
    public class Vec3: IJsonable, IVec3,IFromJsonNode {
        private const int cX = 0, cY = 1, cZ = 2;
        [SerializeField]
        public float [] mVal = new float [3];
        internal int magnitude;

        public float X
        {
            set { mVal [cX] = value; }
            get { return mVal [cX]; }
        }
        public float Y
        {
            set { mVal [cY] = value; }
            get { return mVal [cY]; }
        }
        public float Z
        {
            set { mVal [cZ] = value; }
            get { return mVal [cZ]; }
        }

        public Vec3 Set ( params float [] fs ) {
            Assert.IsTrue(fs.Length == 3);
            for (int i = 0; i < fs.Length; i++) {
                mVal [i] = fs [i];
            }
            return this;
        }

        public Vec3 Set ( Vec3 fs ) {
            Assert.IsNotNull(fs);
            for (int i = 0; i < 3; i++) {
                mVal [i] = fs.mVal [i];
            }
            return this;
        }


        public Vector3 Convert () {
            Vector3 v = new Vector3();
            v.x = X; v.y = Y; v.z = Z;
            return v;
        }

        public static Vec3 Convert ( Vector3 vec ) {
            var ret = new Vec3().Set(vec.x, vec.y, vec.z);
            return ret;
        }


        public static Vec3 Add ( params Vec3 [] v ) {
            Assert.IsNotNull(v);
            Assert.IsTrue(v.Length > 0);
            var ret = new Vec3();
            for (int n = 0; n < v.Length; n++) {
                ret.X = v [n].X + ret.X;
                ret.Y = v [n].Y + ret.Y;
                ret.Z = v [n].Z + ret.Z;
            }
            return ret;
        }

        public static Vec3 Multiple ( Vec3 a, float times ) {
            var ret = new Vec3().Set(a);
            for (int i = 0; i < 3; i++) {
                ret.mVal [i] *= times;
            }
            return ret;
        }

        public void FromJson ( JsonNode json ) {
            X = json ["X"].Get<float>();
            Y = json ["Y"].Get<float>();
            Z = json ["Z"].Get<float>();
        }

        public string ToJson () {
            var dic = new Dictionary<string, string> {
                    { "X",X.ToJson() },
                    { "Y",Y.ToJson() },
                    { "Z",Z.ToJson() }
                };
            return dic.ToJson();
        }

        public override string ToString () {
            return ToJson();
        }

        public override bool Equals ( object obj ) {
            var vec = obj as Vec3;
            if (vec.IsNull()) { return false; }
            return this == vec;
        }

        public override int GetHashCode () {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public static bool operator == ( Vec3 vec1, Vec3 vec2 ) {
            return EqualityComparer<Vec3>.Default.Equals(vec1, vec2);
        }

        public static bool operator != ( Vec3 vec1, Vec3 vec2 ) {
            return !(vec1 == vec2);
        }
    }
}