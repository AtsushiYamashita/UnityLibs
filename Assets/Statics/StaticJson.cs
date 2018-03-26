using System;
using UnityEngine.Assertions;
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

    private static Pair<bool,string> ObjectString(string json) {
        // オブジェクト要素でなければ受け入れない。
        var s = json.IndexOf('{');
        var e = json.LastIndexOf('}');
        Assert.IsTrue(s == 0);
        Assert.IsTrue(s > -1 && e > -1, "Parse Error, this message is not json style object");
        var inside = json.Substring(1, e - 1);
        return StPair.Gen(true, inside);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static StringDict FromJSON ( this string json ) {
        var dic = new StringDict();

        var inside = ObjectString(json);

        // 要素の操作
        var splited = inside.GetValue().Split(',');
        Array.ForEach(splited, ( data ) =>
        {
            // 重複したキーが入っていないかチェックしてからdicに。
            var sets = data.Split(':');
            Assert.IsTrue(dic.KeyNotFound(sets [0]));
            dic.Add(sets [0], sets [1]);
        });
        return dic;
    }
}
