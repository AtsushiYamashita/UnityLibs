namespace BasicExtends {
    using UnityEngine;
    using System;

    /// <summary>
    /// Transformをネットワーク越しに同期する。
    /// </summary>
    public class SyncTransform: MonoBehaviour {
        /// <summary>
        /// 同期する対象の設定
        /// </summary>
        [SerializeField]
        private SyncTo mSyncTo = new SyncTo();
        private SyncData<Trfm> mSync = new SyncData<Trfm>();
        private string mOwnIpid = "";

        /// <summary>
        /// 移動の速度
        /// </summary>
        [SerializeField]
        private float mSpeed = 0.8f;

        /// <summary>
        /// ローカル座標系で計算するか
        /// </summary>
        [SerializeField]
        private bool mIsLocal = false;

        public void SetIpId ( string ipid ) {
            mSyncTo.mIpid = ipid;
        }

        private void Reset () {
            mSyncTo.mComponentName = GetType().Name;
            mSyncTo.mObjectName = name;
        }

        /// <summary>
        /// ローカルとワールドそれぞれに合わせて、
        /// 目標位置にイージングで近づく
        /// </summary>
        private void SyncUpdate ( Trfm e ) {
            var pos_from = mIsLocal ? transform.localPosition : transform.position;
            var pos_set = mIsLocal ?
                (Action<Vector3>) transform.MoveToLocal : transform.MoveTo;
            pos_set(Vector3.Lerp(pos_from, e.POS.Convert(), mSpeed));

            var rot_from = mIsLocal ? transform.localRotation : transform.rotation;
            var rot_set = mIsLocal ?
                (Action<Quaternion>) transform.RotateToLocal : transform.RotateTo;
            rot_set(Quaternion.Lerp(rot_from, Quaternion.Euler(e.ROT.Convert()), mSpeed));
        }

        private void Start () {
            Func<Msg, bool> receiveMatch = null;//msg => mOwnIpid == msg.TryGet("IPID");
            mSync.SyncTo = mSyncTo;
            mSync.WhenStart = e => { transform.localScale = e.SCA.Convert(); };
            mSync.Update = SyncUpdate;
            mSync.Setup(receiveMatch);

            var type = mIsLocal ? Trfm.Type.Local : Trfm.Type.World;

            Messenger.Assign(( msg ) =>
            {
                if (msg.Unmatch(Msg.TO, name)) { return; }
                if (msg.Unmatch(Msg.AS, GetType().Name)) { return; }
                if (msg.Match(Msg.ACT, "SetIPID")) {
                    mOwnIpid = msg.TryGet("IPID");
                    return;
                }
            });
        }

        private void Update () {
            mSync.Sync();

            var type = mIsLocal ? Trfm.Type.Local : Trfm.Type.World;
            var now = Trfm.Convert(transform, type);
            mSync.UpdateSend(() => { return mSync.Prev != now; }, now);
        }
    }
}
