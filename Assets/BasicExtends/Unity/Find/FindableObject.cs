namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// This component express findable object.
    /// </summary>
    public class FindableObject: MonoBehaviour {

        [SerializeField]
        private FindableData mData = new FindableData();
        public FindableData Data { get { return mData; } }

        private void Reset () {
            var count = GetComponents<FindableObject>();
            string assert_msg = "Over hold :: GameObject cannnot hold this component more then 2.";
            Assert.IsTrue(count.Length <= 1, assert_msg);
            mData.Object = gameObject;
            var path = gameObject.GetObjectPath();
            mData.AddTags(path);
            mData.Index = FindStore.Instance.CountbyTag(path);
            FindStore.Instance.AddByKey(mData);
            FindStore.Instance.AddByTag(mData);
        }

        private void Start () {
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }

                if (msg.Match("act", "AddTag")) {
                    var tag = msg.TryGet("tag");
                    AddTag(tag); return;
                }

                if (msg.Match("act", "SetName")) {
                    var name = msg.TryGet("name");
                    SetName(name); return;
                }
            });
        }

        public void AddTag ( string str ) {
            mData.AddTags(str);
        }

        public void SetName ( string str ) {
            mData.AddTags(str);
        }
    }
}
