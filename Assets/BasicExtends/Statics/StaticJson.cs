using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using BasicExtends;

/// <summary>
/// FromJsonNodeから呼ばれることでJsonからオブジェクトを生成するのと同じように使える
/// </summary>
public interface IFromJsonNode {
    void FromJson ( JsonNode node );
}

public static class FromJsonNode {
    public static T Parse<T>(this JsonNode node) where T: class, IFromJsonNode {
        T ret = (T) Activator.CreateInstance(typeof(T), true);
        if (ret.IsNotNull()) { ret.FromJson(node); }
        return ret;
    }
}

public interface IJsonable {
    string ToJson ();
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
            pair.Value.Append(StaticJson.ToJson(obj));
            return true;
        });

        AddMethod(( pair ) =>
        {
            var jsonable = pair.Key as IJsonable;
            if (jsonable.IsNull()) { return false; }
            pair.Value.Append(jsonable.ToJson());
            return true;
        });

        AddMethod(( pair ) =>
        {
            var jsonable = pair.Key as Transform;
            if (jsonable.IsNull()) { return false; }
            pair.Value.Append(Trfm.Convert(jsonable).ToJson());
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
        var inside = json.Substring(1, e - 1);
        return StPair.Gen(true, inside);
    }
}
