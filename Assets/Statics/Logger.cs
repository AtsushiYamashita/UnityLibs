#pragma warning disable 0414

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    public enum DebugLog { Log,Error } 
    public enum ReportLog { Log,Error } 

    public static class Logger{
        private static Dictionary<string, string> mPlaceHolder
            = new Dictionary<string, string>();

        private static string LogString ( string str ) {
            var stack = new System.Diagnostics.StackFrame(2);
            var stack_arr = StArr.To(
                stack.GetFileName(),
                stack.GetFileLineNumber(),
                stack.GetType().Name,
                stack.GetMethod().Name
                );
            var now = DateTime.Now;
            var date = StArr.To(
                //now.Minute + "m",
                now.Second + "s",
                now.Millisecond + "ms");
            var dic = new Dictionary<string, string> {
            { "msg", str },
            { "date", date.Stringify("(","/",")")},
            { "stack", stack_arr.Stringify("(","/",")")}
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
