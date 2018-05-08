namespace BasicExtends {

    using UnityEngine;

    public class SyncTransform: MonoBehaviour {
        [SerializeField]
        private int mPlayerNo = 0;

        [SerializeField]
        private int mPase = 2;
        private int mCount = 0;

        [SerializeField]
        private string mSyncTo = "";

        [SerializeField]
        private float mSpeed = 0.8f;

        [SerializeField]
        private bool mIsLocal = false;

        private Trfm mPrev = null;
        private Trfm mTo = null;

        void Reset () {
            mSyncTo = name;
            mPrev = Trfm.Convert(transform);
        }

        void Start () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) { return; }
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Sync") && msg.ContainsKey("FROM")) {
                    mTo = msg.TryObjectGet<Trfm>();
                    mPrev = mTo;
                    var islocal = msg.Match("isLocal", "True");
                    Sync(islocal);
                    return;
                }
            });
        }

        private void Sync ( bool islocal ) {
            var pos = mTo.POS.Convert();
            var rot = mTo.ROT.Convert();
            var sca = mTo.SCA.Convert();
            if (islocal) {
                transform.localPosition = pos;
                transform.localEulerAngles = rot;
                transform.localScale = sca;
            } else {
                transform.position = pos;
                transform.eulerAngles = rot;
                transform.localScale = sca;
            }
        }

        private void Update () {
            if (mCount++ % mPase != 0) { return; }
            System.Func<Transform, Trfm> func = mIsLocal
                ? (System.Func<Transform, Trfm>) Trfm.Convert
                : Trfm.ConvertWorld;
            var data = func(transform);
            if (data == mPrev) { return; }

            Msg.Gen()
                .To(mSyncTo)
                .As(GetType().Name)
                .Act("Sync")
                .Set("isLocal", mIsLocal ? "True" : "False")
                .Netwrok()
                .SetObjectData(data).Pool();
        }
    }

}