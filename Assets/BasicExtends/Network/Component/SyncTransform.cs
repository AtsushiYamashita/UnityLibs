namespace BasicExtends
{

    using UnityEngine;

    public class SyncTransform : MonoBehaviour
    {
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

        Vector3 mPos = Vector3.zero;
        Vector3 mRot = Vector3.zero;
        Vector3 mSca = Vector3.zero;

        void Reset()
        {
            mSyncTo = name;
        }

        void Start()
        {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) { return; }
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Sync") && msg.ContainsKey("FROM")) {
                    var rec = msg.TryObjectGet<Trfm>();
                    var islocal = msg.Match("isLocal","True");
                    Sync(rec, islocal);
                    return;
                }
            });
        }

        private void Sync(Trfm rec,bool islocal)
        {
            var pos = rec.POS.Convert();
            var rot = rec.ROT.Convert();
            var sca = rec.SCA.Convert();
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

        private void Update()
        {
            if (mCount++ % mPase != 0) { return; }
            System.Func<Transform, Trfm> func = mIsLocal 
                ? (System.Func<Transform, Trfm>) Trfm.Convert 
                : Trfm.ConvertWorld;
            Msg.Gen()
                .To(mSyncTo)
                .As(GetType().Name)
                .Act("Sync")
                .Set("isLocal",mIsLocal? "True": "False")
                .Netwrok()
                .SetObjectData(func(transform)).Pool();
        }
    }

}