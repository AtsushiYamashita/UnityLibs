using System;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class TransformRecorderTest:  TestComponentMulti{

    public void FailTest(string name, Action<string,string> result ) {
        result.Invoke(name, Fail());
    }

}
