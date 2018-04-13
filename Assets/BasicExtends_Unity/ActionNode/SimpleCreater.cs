using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BasicExtends {
    public class SimpleCreater: MonoBehaviour {

        [SerializeField]
        private Transform mParent = null;

        [SerializeField]
        private Transform mPrefab = null;

        private void Start () {
            Assert.IsNotNull(mPrefab);
            MessengerSetup();
        }

        private void MessengerSetup () {
            //Msg.Gen().To(gameObject.name).As(GetType().Name).Push();
            Messenger.Assign(( Msg msg ) => {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Create")) { Create(); return; }
                if (msg.Match("act", "Create as")) { Create(msg.TryGet("name")); return; }
            });
        }

        public void Create (string name = "") {
            var obj = Instantiate(mPrefab, Vector3.zero, Quaternion.identity);
            if (name != "") { obj.name = name; }
            if (mParent == null) { return; }
            obj.transform.parent = mParent;
        }
    }
}