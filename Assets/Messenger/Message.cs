using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

public class Msg: Dictionary<string, string> {
    public Msg Set ( string key, string value ) {
        if (ContainsKey(key) == false) { Add(key, value); return this; }
        this [key] = value;
        return this;
    }

    /// <summary>
    /// when key-value is matched, then this function return true.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Match(string key,string value ) {
        if (ContainsKey(key) == false) { return false; }
        var insideValue = this [key];
        if (insideValue != value) { return false; }
        return true;
    }

    /// <summary>
    /// this function do not return null.
    /// If this cannnot the param, then return empty string.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string TryGet(string key ) {
        if (ContainsKey(key) == false) { return string.Empty; }
        var insideValue = this [key];
        if (insideValue == null) { return string.Empty; }
        return insideValue;
    }

    public string ToJson () {
        string data = string.Empty;
        foreach (var pair in this) {
            if (data.Length > 0) { data += ","; }
            data += string.Format("\"{0}\":\"{1}\"", pair.Key, pair.Value);
        }
        return "{" + data + "}";
    }

}

public static class Messenger {
    private static List<Action<Msg>> mCallBacks = new List<Action<Msg>>();

    public static void Push ( this Msg msg ) {
        if (msg.Match("To", "Messanger") && msg.Match("Action", "ClearAll")) {
            Debug.Log("Data clear <= " + msg.ToJson());
            msg.Clear();
        }
        foreach (var cb in mCallBacks) {
            cb.Invoke(msg);
        }
    }

    public static void Assign ( this Action<Msg> callback ) {
        mCallBacks.Add(callback);
    }

    public static Msg FromJSON ( this string json ) {
        var msg = new Msg();
        var s = json.IndexOf('{');
        var e = json.LastIndexOf('}');
        Assert.IsTrue(s > -1 && e > -1, "Parse Error, this message is not json style object");
        var inside = json.Substring(s+1, e - s-1);
        var splited = inside.Split(',');
        Array.ForEach(splited, ( data ) => {
            var sets = data.Split(':');
            Assert.IsTrue(msg.ContainsKey(sets [0]) == false);
            msg.Add(sets[0], sets[1]);
        });
        return msg;
    }
}
