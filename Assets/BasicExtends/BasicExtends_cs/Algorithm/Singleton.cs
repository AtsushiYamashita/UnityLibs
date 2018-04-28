using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {
    public class Singleton<T> where T : class {
        private static T mInstance = null;
        public static T Instance
        {
            get
            {
                if (mInstance.IsNull()) {
                    mInstance = (T) Activator.CreateInstance(typeof(T), true);
                }
                return mInstance;
            }
        }
    }
}