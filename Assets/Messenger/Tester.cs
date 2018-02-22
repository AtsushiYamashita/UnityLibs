using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

    public string to;
    public bool say = false;

	void Start () {
        Messenger.Assign(e=> {
            Debug.Log(e.ToJson());
            if (e.Match("to", gameObject.name) == false) { return; }
            if (e.Match("Action", "say hello")) {
                Debug.LogFormat("{0} say 'Hello!'", gameObject.name);
                return;
            }
            if (e.Match("Action", "say bye")) {
                Debug.LogFormat("{0} say 'Bye!'", gameObject.name);
                return;
            }

        });
	}
	
	void Update () {
        if (say) {
            //new Msg().Set("to", to).Set("Action", "say hello").Push();
            "{to:aaa,Action:say bye}".FromJSON().Push();
            say = false;
        }
		
	}


}
