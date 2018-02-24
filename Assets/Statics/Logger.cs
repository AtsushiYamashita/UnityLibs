#pragma warning disable 0414

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    public class Logger {
        private static Logger mInstance = new Logger();
        private static Dictionary<string, string> mPlaceHolder
            = new Dictionary<string, string>();
        private Logger () { }

        private static Logger LogPrint ( string str ) {
            var stack = new System.Diagnostics.StackFrame(2);
            var stack_arr = StArr.To(
                stack.GetFileName(),
                stack.GetFileLineNumber(),
                stack.GetType().Name,
                stack.GetMethod().Name
                );
            var now = DateTime.Now;
            var date = StArr.To(
                now.Minute,
                now.Second,
                now.Millisecond);
            var dic = new Dictionary<string, string> {
            { "msg", str },
            { "date", date.Stringify("(","/",")")},
            { "stack", stack_arr.Stringify("(","/",")")}
        };
            Debug.Log(dic.ToJson());
            return mInstance;
        }

        public static Logger Log ( string str ) {
            LogPrint(str);
            return mInstance;
        }
        public static Logger Log ( string str, params object [] args ) {
            LogPrint(string.Format(str, args));
            return mInstance;
        }

        public Logger LogSend ( string to ) {
            if (to == null) { return mInstance; }
            if (to.Length < 1) { return mInstance; }

            return mInstance;
        }
    }
}
