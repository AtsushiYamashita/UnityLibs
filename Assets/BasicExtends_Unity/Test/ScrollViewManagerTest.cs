using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;
using UnityEngine.Assertions;

public class ScrollViewManagerTest : TestComponent {

    [SerializeField]
    private ScrollViewElement mElement1;

    [SerializeField]
    private ScrollViewManager mManager;

    protected override void Init()
    {
        mManager = GetComponent<ScrollViewManager>();
        Assert.IsNotNull(mElement1);
        Assert.IsNotNull(mManager);
    }

    // 正常系
    public string TryGetIndexOwnTest_1()
    {
        var index = mManager.TryGetIndexOwn(mElement1);
        if(index < 0)
        {
            return Fail("Not found");
        }
        if(index != 0)
        {
            return Fail(""+index);
        }
        return Pass();
    }

    // 異常系：発見できない
    public string TryGetIndexOwnTest_2()
    {
        var index = mManager.TryGetIndexOwn(new ScrollViewElement());
        if (index != -1)
        {
            return Fail(""+index);
        }
        return Pass();
    }

    public string AddDataTest()
    {
        mManager.AddData(mElement1,"a", "b", "c");
        return Fail();
    }


}
