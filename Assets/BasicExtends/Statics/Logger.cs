﻿namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum DebugLog { Log, Error }

    public static class Logger {
        public static long mStart = 0;
        public static long mPrev = 0;
        private static Action<string> [] LogFunc
            = new Action<string> [] { Debug.Log, Debug.LogError };

        private static string LogString ( string str ) {
            int look = 3;
            System.Diagnostics.StackFrame stack = null;
            while (stack == null) { stack = new System.Diagnostics.StackFrame(look--); }
            if (mStart == 0) { mStart = DateTime.Now.Ticks; }
            var now = (DateTime.Now.Ticks - mStart) / 1000;
            var dt = now - mPrev;
            mPrev = now;
            var dic = new Dictionary<string, string> {
            { "msg", str },
            { "passed", dt +"/" + now},
            { "stack", stack.GetMethod() != null ? stack.GetMethod().Name : "(property?)" }
        };
            return dic.ToJson();
        }

        public static void Print ( this DebugLog type, string str ) {
            var log_str = LogString(str);
            LogFunc[(int)type](log_str);
        }

        public static void Print ( this DebugLog type, string str, params object [] args ) {
            Print(type, string.Format(str, args));
        }
    }
}
