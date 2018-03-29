using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using BasicExtends;


public class ScrollViewElement : MonoBehaviour
{
    public StringDict mData = null;
    public Action<GameObject, StringDict> mViewAction;

    /// <summary>
    /// こいつの親として一番近いScrollRectを返します。
    /// 取得に失敗したらnullが戻ります。
    /// </summary>
    /// <returns></returns>
    public ScrollRect TryGetScrollView () {
        ScrollRect ret = null;
        Transform temp = transform;
        while (ret == null) {
            var parent = temp.parent;
            if (parent == temp) { return null; ; }

            ret = parent.GetComponent<ScrollRect>();
            if (ret != null) { break; }
            temp = parent;
        }
        return ret;
    }

    public virtual void SetData ( 
        Action<GameObject, StringDict> action ) {
        mViewAction = action;
    }

    public virtual void SetData( StringDict obj,
        Action<GameObject,StringDict> action ) {
        mViewAction = action;
        DataUpdate(obj);
    }

    public virtual void DataUpdate ( StringDict obj ) {
        Assert.IsTrue(mViewAction != null);
        mViewAction.Invoke(gameObject, obj);
    }
}
