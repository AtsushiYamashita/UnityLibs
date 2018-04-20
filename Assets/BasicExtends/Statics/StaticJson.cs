using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using BasicExtends;

public interface IJsonable {
    string ToJson ();
}


public class JsonParseData: Singleton<JsonParseData> {
    private StringBuilder mToken;
    private int mItrIndex = 0;
    private Dictionary<Type, Func<string, Func<object>, object>>
        mParsers = new Dictionary<Type, Func<string, Func<object>, object>>();

    private Dictionary<Type, Func<object>>
        mDefaults = new Dictionary<Type, Func<object>>();

    private ObjDict ObjectParse ( string json, Func<object> def ) {
        var inside = json.Substring(1, json.Length - 2);
        var dic = (ObjDict) def();
        if (inside.Length == 0) { return dic; }
        var sets = inside.Split(',');
        foreach (var set in sets) {

            if (set.Contains(":") == false) {
                throw new Exception("Syntax error (:)=> " + set);
            }
            var kv = set.Split(':');
            dic.Add(kv [0].Substring(1,kv[1].Length-2), kv [1]);
        }
        return dic;
    }

    public JsonParseData () {
        SetParser<string>(
            delegate () { return string.Empty; },
            (json,def)=> {
                if(json.Contains("\"") == false || json.ToCharArray()[0] != '"') {
                    throw new Exception("Syntax error (\")=> " + json);
                }
                return json.Substring(1, json.Length - 2);
            });

        // the Array sentence return string[]
        SetParser<Array>(
    delegate () { return StArr.To(); },
    (json,def)=> {
        if (json.Contains("[") == false || json.ToCharArray() [0] != '[') {
            throw new Exception("Syntax error ([)=> " + json);
        }
        return json.Substring(1, json.Length - 2).Split(',');
    });
        SetParser<ObjDict>(
            delegate () { return new ObjDict(); },
            ObjectParse);
    }

    public void SetParser<T> ( Func<object> deflt, Func<string, Func<object>, object> parser ) {
        mDefaults.TrySet(typeof(T), deflt);
        mParsers.TrySet(typeof(T), parser);
    }

    public static void Set<T> ( Func<object> deflt, Func<string, Func<object>, object> parser ) {
        Instance.SetParser<T>(deflt, parser);
    }

    internal static void Parse<T> ( string json, Action<T, bool> cb ) {
        var type = typeof(T);
        T obj;
        bool res = true;
        try {
            obj = (T) Instance.mParsers [type].Invoke(
                json, Instance.mDefaults [type]);
        } catch (Exception) {
            Debug.Log("test e");
            obj = (T) Instance.mDefaults [type]();
            res = false;
        }
        cb.Invoke(obj, res);
    }
}


public class JsonStringify: Singleton<JsonStringify> {
    public static string Stringify ( object obj ) {
        var pair = new Pair<object, StringBuilder>().Set(obj, new StringBuilder());
        Instance.mStringifyMethods.Invoke(pair);
        return pair.Value.ToString();
    }
    private MethodChain<Pair<object, StringBuilder>> mStringifyMethods = new MethodChain<Pair<object, StringBuilder>>();
    private JsonStringify () {
        AddMethod(( pair ) =>
        {
            var obj = pair.Key;
            if (obj.IsNotNull()) { return false; }
            pair.Value.Append("{}");
            return true;
        });

        AddMethod(( pair ) =>
        {
            var obj = pair.Key as string;
            if (obj == null) { return false; }
            pair.Value.Append("\"").Append(obj).Append("\"");
            return true;
        });

        AddMethod(( pair ) =>
        {
            var obj = pair.Key as Array;
            if (obj == null) { return false; }
            pair.Value.Append(obj.Stringify("[", ",", "]"));
            return true;
        });

        AddMethod(( pair ) =>
        {
            var type = pair.Key.GetType();
            if (type.IsPrimitive == false) { return false; }
            pair.Value.Append("'").Append(pair.Key).Append("'");
            return true;
        });

        AddMethod(( pair ) =>
        {
            var obj = pair.Key as Dictionary<string, string>;
            if (obj.IsNull()) { return false; }
            pair.Value.Append(obj.ToJson<string, string>());
            return true;
        });

        AddMethod(( pair ) =>
        {
            var obj = pair.Key as ObjDict;
            if (obj.IsNull()) { return false; }
            var dic = new Dictionary<string, string>();
            foreach (var e in obj) {
                dic.Add(e.Key, JsonStringify.Stringify(e.Value));
            }
            pair.Value.Append(dic.ToJson("\"", ""));
            return true;
        });

        AddMethod(( pair ) =>
        {
            var type = pair.Key.GetType();
            if (type != typeof(Vector3)) { return false; }
            var vec = (Vector3) pair.Key;
            pair.Value.Append(new Dictionary<string, string> {
            {"x", ""+vec.x},
            {"y", ""+vec.y},
            {"z", ""+vec.z}
        }.ToJson());
            return true;
        });
    }

    public string ToString ( Pair<object, StringBuilder> obj ) {
        mStringifyMethods.Invoke(obj);
        return new Dictionary<string, string>() {
            { "class", this.GetType().Name }
        }.ToJson();
    }
    public void AddMethod ( Func<Pair<object, StringBuilder>, bool> action ) {
        if (mStringifyMethods.GetProcessHolder().Contains(action)) { return; }
        mStringifyMethods.GetProcessHolder().Add(action);
    }

}

/// <summary>
/// Jsonとして文字列化する
/// (TODO：メソッドチェーンを書いたらそちらに分岐処理を渡す)
/// </summary>
public static class StaticJson {
    public static string ToJson ( this object obj ) {

        if (obj.IsNull()) { return "''"; }
        var type = obj.GetType();
        if (type.IsArray) {
            return JsonStringify.Stringify(obj);
        }
        if (type.IsPrimitive) {
            return "'" + obj.ToString() + "'";
        }
        return JsonStringify.Stringify(obj);
    }

    private static Pair<bool, string> ObjectString ( string json ) {
        // オブジェクト要素でなければ受け入れない。
        var s = json.IndexOf('{');
        var e = json.LastIndexOf('}');
        Assert.IsTrue(s == 0);
        Assert.IsTrue(s > -1 && e > -1, "Parse Error, this message is not json style object");
        var inside = json.Substring(1, e - 1);
        return StPair.Gen(true, inside);
    }
}
