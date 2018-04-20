#pragma warning disable 0414

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    public enum DebugLog { Log, Error }
    public enum ReportLog { Log, Error }

    public static class Logger {
        public static long mStart = 0;
        public static long mPrev = 0;
        private static Dictionary<string, string> mPlaceHolder
            = new Dictionary<string, string>();

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
            Action<string> log = Debug.Log;
            Action<string> err = Debug.LogError;
            var print = type == DebugLog.Log ? log : err;
            print(log_str);
        }

        public static void Print ( this DebugLog type, string str, params object [] args ) {
            Print(type, string.Format(str, args));
        }

        public static void Print ( this ReportLog type, string str ) {
            var log_str = LogString(str);
            Action<string> log = Debug.Log;
            Action<string> err = Debug.LogError;
            var print = type == ReportLog.Log ? log : err;
            print(log_str);
        }

        public static void Print ( this ReportLog type, string str, params object [] args ) {
            Print(type, string.Format(str, args));
        }
    }
}
