using System;
using System.Text;
using System.Collections.Generic;
using BasicExtends;

public static class StaticJson {
    public static string ToJson ( this Array arr ) {
        return arr.Stringify("[", ", ", "]");
    }

    public static string ToJson<K, V> ( this Dictionary<K, V> dic ) {
        return dic.Stringify();
    }
    public static string ToJson (  ) {
        return "''";
    }

    public static string ToJson ( this object obj ) {
        if (obj.IsNull()) { return "''"; }
        if (obj.GetType().IsArray) {
            Array arr = (Array) obj;
            return arr.ToJson();
        }
        if (obj.GetType().IsPrimitive) {
            return "'" + obj.ToString() + "'";
        }
        var dic = obj as Dictionary<object, object>;
        return dic.ToJson();
    }

    public static string ToJson<K, V> ( this object obj, 
        params string[] keys ) {
        var type = obj.GetType();
        var dic = new Dictionary<string, string>() {
            {"type",type.Name }
        };
        foreach(var key in keys) {
            var mems = type.GetMember(key);
            var val = string.Empty;
            var isNotManaged = mems.IsNull() || mems.Length < 1;
            if (isNotManaged) {
                dic.Add(key, ToJson());
                continue;
            }
            if(mems.Length == 1) {
            }
            dic.Add(key, val);
        }
        return dic.ToJson();
    }
}
