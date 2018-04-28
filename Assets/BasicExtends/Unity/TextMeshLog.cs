using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class TextMeshLog : MonoBehaviour {

	void Start () {
        var tmesh = GetComponent<TextMesh>();
        Application.logMessageReceived +=
            (msg,stk,type) => { tmesh.text = msg + "\n" + tmesh.text; };
    }
}
