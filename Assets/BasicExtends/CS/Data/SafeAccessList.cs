using System;
using System.Collections.Generic;

namespace BasicExtends {
    [Serializable]
    public class SafeAccessList<T> where T:class {
        [NonSerialized]
        object mLock = new object();
        [NonSerialized]
        private List<T> mValue;

        public SafeAccessList () {
            mValue = new List<T>();
        }

        public SafeAccessList<T> Remove ( int i ) {
            lock (mLock) {
                mValue.RemoveAt(i);
            }
            return this;
        }

        public int Count () {
            return mValue.Count;
        }

        public T Pop (  ) {
            T t;
            if(mValue.Count < 1) { return null; }
            var last = mValue.Count - 1;
            lock (mLock) {
                t = mValue [ last];
                mValue.RemoveAt(last);
            }
            return t;
        }

        public T Get (int i) {
            T t;
            lock (mLock) {
                t = mValue [i];
            }
            return t;
        }

        public SafeAccessList<T> Add ( T t ) {
            lock (mLock) {
                mValue.Add(t);
            }
            return this;
        }
    }
}