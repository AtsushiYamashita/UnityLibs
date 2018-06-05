namespace BasicExtends {
    using UnityEngine;

    /// <summary>
    /// LifedThreadに対して、
    /// 生存時間のUpdateを発信する。
    /// 処理そのものはThreadManagerにほぼ依存している。
    /// </summary>
    public class LifedThreadManager: MonoBehaviour {

        private void Start () {
            ThreadManager.Instance.Enable = true;
            Debug.Log("LifelimitedThreadManager");
        }

        private void Update () {
            ThreadManager.DeadTimeUpdate();
            Messenger.Flash();
        }

        private void OnDestroy () {
            ThreadManager.Abort();
        }

        private void OnApplicationQuite () {
            ThreadManager.Abort();
        }
    }
}
