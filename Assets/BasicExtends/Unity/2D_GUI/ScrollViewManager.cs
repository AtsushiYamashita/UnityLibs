using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class ScrollViewManager: MonoBehaviour {
    [SerializeField]
    private ScrollViewElement mViewPrefabs = null;

    public int TryGetIndexOwn ( ScrollViewElement target ) {
        if (target == null) { return -1; }
        var vp = gameObject.FindChild("Viewport");
        var cnt = vp.FindChild("Content");
        var elements = cnt.Children().Select(( e ) => { return e.GetComponent<ScrollViewElement>(); });
        int count = 0;
        foreach(var e in elements) {
            if(target == e) { return count; }
            ++count;
        }
        return -1;
    }

    public void AddData ( System.Action<GameObject, StringDict> viewAction,
        params StringDict [] data ) {
        foreach (var d in data) {
            var t = Instantiate(mViewPrefabs);
            t.SetData(d, viewAction);
        }
    }
}
