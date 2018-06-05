using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;


[RequireComponent(typeof(Renderer))]
public class NetworkTest: MonoBehaviour {

    private Renderer mOwnRenderer = null;

    [SerializeField]
    private KeyCode mEvent = KeyCode.D;

    private void Start () {
        mOwnRenderer = GetComponent<Renderer>();

        Messenger.Assign(( msg ) =>
        {
            if (msg.Match("Network", "True")) { return; }
            if (msg.Unmatch(Msg.TO, name)) { return; }
            if (msg.Unmatch(Msg.AS, GetType().Name)) { return; }
            if (msg.Match(Msg.ACT, "Switch")) {
                Switch();
                return;
            }
        });
    }

    private void Update () {
        if (Input.GetKeyDown(mEvent) == false) { return; }
        Debug.Log("send");
        Msg.Gen().Set(Msg.TO, name)
            .Set(Msg.AS, GetType().Name)
            .Set(Msg.ACT, "Switch")
            .Netwrok()
            .Push();
    }

    private void Switch () {
        mOwnRenderer.enabled = !mOwnRenderer.enabled;
    }
}
