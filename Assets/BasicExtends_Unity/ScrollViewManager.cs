using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class ScrollViewManager : MonoBehaviour
{


    public int TryGetIndexOwn(ScrollViewElement elm)
    {
        if (elm == null) { return -1; }
        var vp = gameObject.FindChild("Viewport");
        var cnt = vp.FindChild("Content");
        var children = cnt.Children();

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i].GetComponent<ScrollViewElement>();
            if (elm != child) { continue; }
            return i;
        }

        return -1;
    }

    public void AddData(params object[] data)
    {

    }
}
