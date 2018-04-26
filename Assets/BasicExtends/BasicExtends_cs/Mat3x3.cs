namespace BasicExtends {
    using UnityEngine;
    using System;

    public class Mat3x3 {
        public float [] mVal = new float [] {
            0,0,0,
            0,0,0,
            1,1,1
        };
        public float V00 { set { mVal [0] = value; } get { return mVal [0]; } }
        public float V10 { set { mVal [1] = value; } get { return mVal [1]; } }
        public float V20 { set { mVal [2] = value; } get { return mVal [2]; } }
        public float V01 { set { mVal [3] = value; } get { return mVal [3]; } }
        public float V11 { set { mVal [4] = value; } get { return mVal [4]; } }
        public float V21 { set { mVal [5] = value; } get { return mVal [5]; } }
        public float V02 { set { mVal [6] = value; } get { return mVal [6]; } }
        public float V12 { set { mVal [7] = value; } get { return mVal [7]; } }
        public float V22 { set { mVal [8] = value; } get { return mVal [8]; } }
        public Mat3x3 SetLine ( int l, Vec3 vec ) {
            foreach (var i in System.Linq.Enumerable.Range(0, 3)) {
                mVal [3 * l + i] = vec.mVal [i];
            }
            return this;
        }
        public Vec3 Multiple ( Vec3 vec ) {
            var ret = new Vec3();
            ret.X = mVal [0] * vec.X + mVal [1] * vec.Y + mVal [2] * vec.Z;
            ret.Y = mVal [3] * vec.X + mVal [4] * vec.Y + mVal [5] * vec.Z;
            ret.Z = mVal [6] * vec.X + mVal [7] * vec.Y + mVal [8] * vec.Z;
            return ret;
        }
        public Mat3x3 SetMove ( Vector2 vec ) {
            V20 += vec.x;
            V21 += vec.y;
            return this;
        }
        public Mat3x3 SetRot ( float size, int x, int y ) {
            throw new NotImplementedException();
        }
        public Mat3x3 SetSca ( float size, int x, int y ) {
            throw new NotImplementedException();
        }
    }

}

