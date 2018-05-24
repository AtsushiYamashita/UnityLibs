namespace BasicExtends {
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class StEnum {
        public static CheckedRet<T> ParseEnum<T> ( this string str ) where T : struct {
            var arr = StEnum.ToArray<T>();
            foreach (var e in arr) {
                if (e.ToString() != str) { continue; }
                return new CheckedRet<T>().Set(true, e);
            }
            return CheckedRet<T>.Fail();
        }

        public static T [] ToArray<T> () where T : struct {
            return (T []) System.Enum.GetValues(typeof(T));
        }
    }
}
