using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ApplicationUtil: MonoBehaviour {

    /// <summary>
    /// アプリの中断で呼び出される
    /// </summary>
    [SerializeField]
    UnityEvent mPause = new UnityEvent();

    /// <summary>
    /// アプリの中断したあと、再開示に呼び出される
    /// </summary>
    [SerializeField]
    UnityEvent mRestart = new UnityEvent();


    /// <summary>
    /// アプリの終了時に呼び出される
    /// mAppDirectExitがfalseだと最後にAppExitを呼ばないと終了しなくなる
    /// </summary>
    [SerializeField]
    UnityEvent mAppExitStart = new UnityEvent();

    /// <summary>
    /// mAppExitStartを呼ばずにアプリを終了する
    /// </summary>
    [SerializeField]
    private bool mAppDirectExit = false;

    /// <summary>
    /// mAppExitStartを呼ばずにアプリを終了する
    /// </summary>
    public void AppExit () {
        if (Application.platform == RuntimePlatform.WindowsEditor) { Debug.DebugBreak(); }
        mAppDirectExit = true;
        Application.Quit();
    }

    /// <summary>
    /// アプリの終了時に自動で呼び出される
    /// </summary>
    private void OnApplicationQuit () {
        if (mAppDirectExit) { Debug.Log("Application.Quit"); Application.Quit(); }
        mAppExitStart.Invoke();

        // 終了処理を中止させている
        Application.CancelQuit();
    }

    /// <summary>
    /// アプリが一時停止・再開したときに呼び出される
    /// </summary>
    private void OnApplicationPause ( bool pauseStatus ) {
        if (pauseStatus) { mPause.Invoke(); return; }
        mRestart.Invoke();
    }
}
