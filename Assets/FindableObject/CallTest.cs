using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class CallTest: MonoBehaviour {

    private void Start () {
        var store = Findable.Store.Instance;
        TestA(store.GetNamed, "largeC", "CCC", "Basic test.");
        TestA(store.GetNamed, "SmallF", "fff", "Get from another scene.");
        TestB(store.GetTagged, "Scene1", 4, "Basic test.");
        TestB(store.GetTagged, "Scene2", 4, "Get from another scene.");
        TestB(store.GetTagged, "Child", 4, "Complex query");
        TestC(store.GetTagged, "Child", new string [] { "a", "b", "s", "x" }, "order query");
        TestB(store.GetTagged, "tag", 1, "s qumall size ery");
        TestB(store.GetTagged, "Empty", 6, "s Big size ery");

    }

    private void TestA ( Func<string, GameObject> func, string arg1, string arg2, string reason ) {
        var testName = func.Method.Name;
        var data = func(arg1);
        var d = data.GetComponent<FindableObject>().Data;
        Assert.IsTrue(d.SarchKey == arg1);
        Assert.IsTrue(data.name == arg2);
        Debug.LogFormat("Test[{0}:{1}] succeed for {2}", testName, arg1, reason);
    }

    private void TestB ( Func<string, List<GameObject>> func, string arg1, int arg2, string reason ) {
        var testName = func.Method.Name;
        var datas = func(arg1);
        Assert.IsTrue(datas.Count == arg2,string.Format("count:{0},arg2:{1}",datas.Count, arg2));
        foreach (var data in datas) {
            var d = data.GetComponent<FindableObject>().Data;
            var con = Array.IndexOf(d.Tags, arg1);
            Assert.IsTrue(con >= 0);

        }
        Debug.LogFormat("Test[{0}:{1}] succeed for {2}", testName, arg1, reason);
    }

    private void TestC ( Func<string, List<GameObject>> func, string arg1, string [] arg2, string reason ) {
        var testName = func.Method.Name;
        var datas = func(arg1).ToArray();
        for (int i = 0; i < arg2.Length; i++) {
            Assert.IsTrue(datas [i].name.IndexOf(arg2 [i]) > -1);
        }
        Debug.LogFormat("Test[{0}:{1}] succeed for {2}", testName, arg1, reason);
    }
}
