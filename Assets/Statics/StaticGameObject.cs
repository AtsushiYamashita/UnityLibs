using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticGameObject  {

    public static GameObject [] Children ( this GameObject obj ) {
        var children = new List<GameObject>();
        var tr = obj.transform;
        for (int i = 0; i < tr.childCount; i++) {
            var ch = tr.GetChild(i).gameObject;
            if (ch.activeSelf == false) { continue; }
            children.Add(ch);
        }
        return children.ToArray();
    }

    public static GameObject FindBrother ( this GameObject obj, string name ) {
        var tr = obj.transform.parent;
        for (int i = 0; i < tr.childCount; i++) {
            var ch = tr.GetChild(i).gameObject;
            if (ch.activeSelf == false) { continue; }
            if (ch.name == name) { return ch; }
        }
        throw new Exception(name + " is not found in brother");
    }

    public static GameObject FindChild ( this GameObject obj, string name ) {
        var tr = obj.transform;
        for (int i = 0; i < tr.childCount; i++) {
            var ch = tr.GetChild(i).gameObject;
            if (ch.activeSelf == false) { continue; }
            if (ch.name == name) { return ch; }
        }
        throw new Exception(name + " is not found in child");
    }
}
