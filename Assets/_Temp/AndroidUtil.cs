using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Android用に開発する場合の汎用便利機能
/// </summary>
public class AndroidUtil: MonoBehaviour {

    /// <summary>
    /// スリープ状態から復帰したときに呼び出しされる
    /// </summary>
    [SerializeField]
    UnityEvent mSleeped = new UnityEvent();

    /// <summary>
    /// Escapeキーや、Androidのバックボタンを押されたときに実行
    /// </summary>
    [SerializeField]
    UnityEvent mBackbutton = new UnityEvent();

    /// <summary>
    /// Escapeキーや、Androidのバックボタンを長押しされたときに実行
    /// </summary>
    [SerializeField]
    UnityEvent mBackbuttonLong = new UnityEvent();

    [SerializeField]
    private float mEscapeLongTime = 2;

    private long mLastUpdate = 0;
    private const int SEC = 1000 * 1000;
    private float mEscapeStart = -1;

    /// <summary>
    /// スリープ状態を強制的に回避する
    /// </summary>
    public void DontSleepMode () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// スリープ状態を強制的に回避する
    /// </summary>
    public void SleepModeSetDefault () {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    private void Start () {
        mLastUpdate = System.DateTime.Now.Ticks;
    }

    private void Update () {
        var now = System.DateTime.Now.Ticks;
        if (mLastUpdate > 0 && now - mLastUpdate > 10 * SEC) { mSleeped.Invoke(); }
        mLastUpdate = now;
        EscapeEvent();
    }

    private void EscapeEvent () {
        if (Input.GetKey(KeyCode.Escape) == false) { mEscapeStart = -1; return; }
        if (mEscapeStart < 0) { mEscapeStart = Time.time; mBackbutton.Invoke(); return; }
        if (Time.time - mEscapeStart > mEscapeLongTime) { mBackbuttonLong.Invoke(); return; }
    }
}
