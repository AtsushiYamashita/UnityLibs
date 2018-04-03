using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class MethodChainTest : TestComponent {
    public string FailTest (string name) {
        return Fail();
    }

}
