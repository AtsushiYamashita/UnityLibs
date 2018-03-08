using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class SingletonTest : TestComponent<Singleton<SingletonTest>> {

    public class TestClass  : Singleton<TestClass> {
        public int i = 0;
        private TestClass () {
        }
    }

    public class TestClass2: Singleton<TestClass> {
        public TestClass2 () {
            Instance.i++;
        }
    }


    public string ActivateTest () {
        var t = TestClass.Instance;
        if (t.GetType().Name != "TestClass") { return "インスタンス失敗"; }
        return "";

    }

    public string ActivateTest2 () {
        var t = TestClass2.Instance;
        if (t.GetType().Name != "TestClass") { return "インスタンス失敗";  }
        return "";
    }
}
