using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionSet : MonoBehaviour {

    [SerializeField]
    private UnityEvent mEvent = new UnityEvent();

    public void Action () {
        mEvent.Invoke();
    }
}
