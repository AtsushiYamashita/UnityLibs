using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using BasicExtends;

public interface IJsonable {
    string ToJson ();
}

/// <summary>
/// Jsonとして文字列化する
/// (TODO：メソッドチェーンを書いたらそちらに分岐処理を渡す)
/// </summary>
public static class StaticJson {
    public static string ToJson ( this Array arr ) {
        Assert.IsNotNull(arr);
        return arr.Stringify("[", ", ", "]");
    }

    public static string ToJson<K, V> ( this Dictionary<K, V> dic ) {
        Assert.IsNotNull(dic);
        return dic.Stringify();
    }
    public static string ToJson (  ) {
        return "''";
    }

    public static string ToJson ( this Vector3 vec ) {
        return new Dictionary<string, string> {
            {"x", ""+vec.x},
            {"y", ""+vec.y},
            {"z", ""+vec.z}
        }.ToJson();
    }

    public static string ToJson ( this object obj ) {
        if (obj.IsNull()) { return "''"; }
        var type = obj.GetType();
        if (type.IsArray) {
            Array arr = (Array) obj;
            return arr.ToJson();
        }
        if (type.IsPrimitive) {
            return "'" + obj.ToString() + "'";
        }
        if (type == typeof(Vector3)) {
            return "'" + obj.ToJson() + "'";
        }
        var jsonable = obj as IJsonable;
        if (jsonable != null) {

        }
        var dic = obj as Dictionary<object, object>;
        return dic.ToJson();
    }

    /// <summary>
    /// ディクショナリに対して、Keyを指定してJSON処理を行う
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="obj"></param>
    /// <param name="keys"></param>
    /// <returns></returns>
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
        var splited = inside.Value.Split(',');
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
