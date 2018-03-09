using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ScrollViewElement : MonoBehaviour
{
    /// <summary>
    /// こいつの親として一番近いScrollRectを返します。
    /// 取得に失敗したらnullが戻ります。
    /// </summary>
    /// <returns></returns>
    public ScrollRect TryGetScrollView()
    {
        ScrollRect ret = null;
        Transform temp = transform;
        while (ret == null)
        {
            var parent = temp.parent;
            if (parent == temp) { return null; ; }

            ret = parent.GetComponent<ScrollRect>();
            if (ret != null) { break; }
            temp = parent;
        }
        return ret;
    }

    public virtual void SetData(object obj) { }
}
