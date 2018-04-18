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
            Messenger.Assign(( Msg msg ) => {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Create")) { Create(); return; }
                if (msg.Match("act", "Create child")) { CreateChild(); return; }
                var name = msg.TryGet("name");
                if (msg.Match("act", "Create as")) { Create(name); return; }
                if (msg.Match("act", "Create child as")) { CreateChild(name); return; }
            });
        }

        public void Create (string name = "") {
            var obj = Instantiate(mPrefab, Vector3.zero, Quaternion.identity);
            if (name != "") { obj.name = name; }
        }

        public void CreateChild ( string name = "" ) {
            if (mParent == null) { Create(name); return; }
            var obj = Instantiate(mPrefab, Vector3.zero, Quaternion.identity);
            if (name != "") { obj.name = name; }
            obj.transform.parent = mParent;
        }

        public void SetParent (Transform parent) {
            mParent = parent;
        }
    }
}