#pragma warning disable 0649 // never assigned. <= ignore

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;


/// <summary>
/// Androidでの実行時にはGyroに従い、
/// Editorでは矢印キーとマウスポジションに従うカメラ方向指定のコンポネントです。
/// Resetで強制的に正面を向かせます。
/// </summary>
[RequireComponent(typeof(Camera))]
public class GyroCamera: MonoBehaviour {

    [SerializeField]
    private Transform _pivot;

    [SerializeField]
    private Vector3 _reset_look_at_to;

    [SerializeField]
    private float __euler_speed_key = 1;

    [SerializeField]
    private float __euler_speed_mouse = 1;

    private Vector3 _prev_mouse_pos;
    private Quaternion _gyro;
    private Vector3 _euler;

    private Quaternion _fix;

    [SerializeField]
    private bool mResetOnlyY = true;

    private Quaternion GetGyroRot () {
        var isAndroid = Application.platform == RuntimePlatform.Android;
        if (Input.gyro.enabled && isAndroid) {
            var gyro = Input.gyro.attitude;
            var a = Quaternion.Euler(90, 0, 0);
            var b = new Quaternion(-gyro.x , -gyro.y, gyro.z, gyro.w);
            _gyro = a *  b ;
        } else {
            _EulerUpdate();
            _gyro = Quaternion.Euler(_euler);
        }
        return _gyro;
    }

    private void _EulerUpdate () {
        var dist = __euler_speed_key * Time.deltaTime * 100;
        _euler.x += Input.GetKey(KeyCode.UpArrow) ? +dist : 0;
        _euler.x += Input.GetKey(KeyCode.DownArrow) ? -dist : 0;
        _euler.y += Input.GetKey(KeyCode.LeftArrow) ? +dist : 0;
        _euler.y += Input.GetKey(KeyCode.RightArrow) ? -dist : 0;
        var mouse_pos = Input.mousePosition;
        var dif = mouse_pos - _prev_mouse_pos;
        _euler.y += dif.x * __euler_speed_mouse /10;
        _euler.x -= dif.y * __euler_speed_mouse / 10;
        _prev_mouse_pos = mouse_pos;
    }

    public void Reset () {
        //_pivot.rotation = transform.rotation *  Quaternion.Inverse(transform.rotation) ;
        if (mResetOnlyY) {
            var rot = Quaternion.Inverse(transform.localRotation).eulerAngles;
            _pivot.rotation = Quaternion.Euler(0, rot.y, 0);
            Debug.LogFormat("Gyro({0}) is Reset.{1}", name, _fix);
            return;
        }
        _pivot.rotation = Quaternion.Inverse(transform.localRotation);
        Debug.LogFormat("Gyro({0}) is Reset.{1}", name, _fix);
    }

    private void Start () {
        Input.gyro.enabled = true;
         Reset();
        _prev_mouse_pos = Input.mousePosition;
    }

    public void Test () {
        _pivot.Rotate(0, 10, 0);
    }

    private void OnPreRender () {
        this.transform.localRotation = GetGyroRot();
        //transform.localRotation += Quaternion.Euler(0, -_start.y, 0); 
    }
    private void Update () {
    }
}
