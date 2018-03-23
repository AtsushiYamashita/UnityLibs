using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewElement<T> : ScrollViewElement where T:class
{
    public T mData = null;

    public override void SetData(object obj, System.Action<GameObject> action)
    {
        mData = obj as T;
        action.Invoke(gameObject);
    }


    public T GetData () {
        return mData;
    }
}
