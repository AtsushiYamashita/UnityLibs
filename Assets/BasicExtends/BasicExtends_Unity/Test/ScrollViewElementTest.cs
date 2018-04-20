using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using BasicExtends;

public class ScrollViewElementTest: TestComponent {

    private ScrollViewElement mElement1;

    [SerializeField]
    private ScrollViewManager mManager;

    protected override void Init () {
        mElement1 = GetComponent<ScrollViewElement>();
        Assert.IsNotNull(mElement1);
        Assert.IsNotNull(mManager);
    }

    public string TryGetScrollViewTest () {
        var scr = mElement1.TryGetScrollView();
        if (scr == null) { return Fail(); }

        var name = scr.gameObject.name;
        if (name != mManager.name) { return Fail(name); }

        return Pass();
    }

}
