using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {
    public class ThreadsafeCounter {
        private int mCounter = 0;
        private System.Object mLock = new System.Object();
        public ThreadsafeCounter Increment () {
            lock (mLock) {
                mCounter++;
            }
            return this;
        }
        public int Get () {
            return mCounter;
        }
    }
}