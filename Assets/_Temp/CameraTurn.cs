#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurn : MonoBehaviour {
	void Start () {
        transform.Rotate(0, 0, 180);
	}
}
#endif