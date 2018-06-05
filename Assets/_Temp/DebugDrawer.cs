using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

/// <summary>
/// デバッグ時にログを確認するためのコンポネント
/// 
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class DebugDrawer: MonoBehaviour {

    private TextMesh mMesh = null;

    [SerializeField]
    private bool mPrintAssert = true;
    [SerializeField]
    private bool mPrintError = true;
    [SerializeField]
    private bool mPrintException = true;
    [SerializeField]
    private bool mPrintLog = true;
    [SerializeField]
    private bool mPrintWarning = true;

    private const int LENGTH = 1000;

    private void Reset () {
        mMesh = GetComponent<TextMesh>();
        mMesh.fontSize = 100;
        mMesh.tabSize = 2;
        mMesh.offsetZ = 1;
        mMesh.anchor = TextAnchor.UpperCenter;
        mMesh.alignment = TextAlignment.Center;
    }
    private void Start () {
        mMesh = GetComponent<TextMesh>();
    }

    private void OnEnable () {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable () {
        Application.logMessageReceived -= HandleLog;
    }

    private CheckedRet<string> GetTextPos ( string stackTrace, int line ) {
        var line_str = stackTrace.Split('\n');
        if (line_str.Length <= line) { return CheckedRet<string>.Fail(); }
        var index = line_str [line].LastIndexOf('/');
        if (index < 0) { return CheckedRet<string>.Fail(); }
        return new CheckedRet<string>().Set(true, line_str [line].Substring(index + 1));
    }

    private void HandleLog ( string logString, string stackTrace, LogType type ) {
        if (mMesh.IsNull()) { return; }
        if (type == LogType.Assert && mPrintAssert == false) { return; }
        if (type == LogType.Error && mPrintError == false) { return; }
        if (type == LogType.Exception && mPrintException == false) { return; }
        if (type == LogType.Log && mPrintLog == false) { return; }
        if (type == LogType.Warning && mPrintWarning == false) { return; }
        var pos1 = GetTextPos(stackTrace, 1);
        var pos2 = GetTextPos(stackTrace, 2);
        var pos = string.Format(" ({0}\n ({1}\n", (pos1.Key ? pos1.Value : "-)"), (pos2.Key ? pos2.Value : "-)"));
        mMesh.text = string.Format("{0}\n{1}\n{2}", logString, pos.Length > 10 ? pos : stackTrace, mMesh.text);
        if (mMesh.text.Length > LENGTH) { mMesh.text = mMesh.text.Substring(0, LENGTH); }
    }
}
