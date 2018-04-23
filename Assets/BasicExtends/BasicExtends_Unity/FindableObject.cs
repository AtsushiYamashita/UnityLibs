using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using BasicExtends.Findable;

namespace BasicExtends {

    namespace Findable {

        /// <summary>
        /// This data include for you can find this object.
        /// </summary>
        [System.Serializable]
        public class Data {
            private GameObject mObject = null;
            public GameObject Object
            {
                get { return mObject; }
                set { this.mObject = value; }
            }

            [SerializeField]
            private string mSarchKey = string.Empty;
            public string SarchKey
            {
                get { return mSarchKey; }
                set { this.mSarchKey = value; }
            }

            [SerializeField]
            private List<string> mTags = new List<string>();
            public void SetTags ( params string [] args ) {
                mTags = new List<string>();
                AddTags(args);
            }
            public void AddTags ( params string [] args ) {
                mTags.AddRange(args);
                Store.Instance.AddByTag(this);

            }
            public List<string> Tags
            {
                get { return mTags; }
            }

            [SerializeField]
            private int mIndex = -1;
            public int Index
            {
                get { return mIndex; }
                set { this.mIndex = value; }
            }

            public override string ToString () {
                string ret;
                ret = string.Format("GameObject:{0}\n", Object.GetObjectPath());
                for (int i = 0; i < mTags.Count; i++) {
                    ret += string.Format("tag{0}:{1}\n", i, mTags [i]);
                }
                ret += string.Format("index:{0}\n", mIndex);
                return ret;
            }
        }

        /// <summary>
        /// sarchKey = nameかtagを使って、
        /// 登録したり
        /// </summary>
        public class Store: Singleton<Store> {
            private Dictionary<string, Data> mDatas = new Dictionary<string, Data>();
            private Dictionary<string, List<Data>> mTagedDatas = new Dictionary<string, List<Data>>();

            private Store () { } // seal

            /// <summary>
            /// Clear all stored data.
            /// You will use game data reset.
            /// </summary>
            public void Clear () {
                mDatas = new Dictionary<string, Data>();
                mTagedDatas = new Dictionary<string, List<Data>>();
            }

            public int CountbyTag ( string str ) {
                if (mTagedDatas.ContainsKey(str) == false) { return 0; }
                return mTagedDatas [str].Count;
            }

            public void AddByKey ( Data data ) {
                var key = data.SarchKey;
                if (key == string.Empty) { return; }

                var keyContained = mDatas.ContainsKey(key);
                if (keyContained) {
                    string str = string.Format("The key:{0} is used by {1},{2}"
                        , key, mDatas [key].Object, data.Object);
                    throw new System.Exception(str);
                }
                mDatas.TrySet(key, data);
            }

            public void AddByTag ( Data data ) {
                foreach (var tag in data.Tags) {
                    if (!mTagedDatas.ContainsKey(tag)) { mTagedDatas.Add(tag, new List<Data>()); }
                    var contain = mTagedDatas [tag].Where(e => { return e.Object == data.Object; });
                    if (contain != null) { return; }
                    mTagedDatas [tag].Add(data);
                }
            }

            /// <summary>
            /// Get named object.
            /// </summary>
            /// <param name="name"></param>
            /// <returns>When that named object is nothing then return null</returns>
            public GameObject GetNamed ( string name ) {
                Data data = null;
                var exist = mDatas.TryGetValue(name, out data);
                if (!exist) { return null; }
                if (data == null) { return null; }
                if (data.Object == null) {
                    mDatas.Remove(name);
                    return null;
                }
                return data.Object;
            }

            /// <summary>
            /// Get tagged object.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public List<GameObject> GetTagged ( string tag ) {
                List<Data> datas = null;
                var exist = mTagedDatas.TryGetValue(tag, out datas);
                if (!exist) { return null; }
                if (datas == null) { return null; }

                var dat2 = datas.Where(e =>
                {
                    if (e == null) { return false; }
                    if (e.Object == null) { return false; }
                    return true;
                }).ToList();
                mTagedDatas [tag] = dat2;
                return dat2.Select(e => { return e.Object; }).ToList();
            }
        }
    }

    /// <summary>
    /// This component express findable object.
    /// </summary>
    public class FindableObject: MonoBehaviour {

        [SerializeField]
        private Data mData = new Data();
        public Data Data { get { return mData; } }

        private void Reset () {
            var count = GetComponents<FindableObject>();
            Assert.IsTrue(count.Length <= 1, "Over hold :: GameObject cannnot hold this component more then 2.");
            mData.Object = gameObject;
            var path = gameObject.GetObjectPath();
            mData.AddTags(path);
            mData.Index = Store.Instance.CountbyTag(path);
            Store.Instance.AddByKey(mData);
            Store.Instance.AddByTag(mData);
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
