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
            private string [] mTags = new string [0];
            public void SetTags ( params string [] args ) {
                mTags = args;
            }
            public string [] Tags
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

            private string GetObjectPath () {
                string ret = Object.name;
                Transform parent = Object.transform.parent;
                while (parent != null) {
                    ret = string.Format("{0}/{1}", parent.name, ret);
                }
                ret = string.Format("{0}/{1}", Object.scene.name, ret);
                return ret;
            }

            public override string ToString () {
                string ret;
                ret = string.Format("GameObject:{0}\n", GetObjectPath());
                for (int i = 0; i < mTags.Length; i++) {
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

            private void AddByKey ( Data data ) {
                var key = data.SarchKey;
                if (key != string.Empty) {
                    var keyContained = mDatas.ContainsKey(key);
                    if (!keyContained) { mDatas.Add(key, null); }
                    if (keyContained) {
                        var old = mDatas [key].Object;
                        var isKeyUsed = old != data.Object;
                        string str = string.Format("The key:{0} is used by {1},{2}", key, old, data.Object);
                        Assert.IsTrue(!isKeyUsed, str);
                    }
                    mDatas [key] = data;
                }
            }

            private void AddByTag ( Data data ) {
                foreach (var tag in data.Tags) {
                    if (!mTagedDatas.ContainsKey(tag)) { mTagedDatas.Add(tag, new List<Data>()); }
                    var insided = mTagedDatas [tag].Where(e => { return e.Object == data.Object; }).ToArray();
                    if (insided.Length < 1) {
                        mTagedDatas [tag].Add(data);
                    }
                }
            }

            /// <summary>
            /// Add the object. Almost object use in awake.
            /// </summary>
            /// <param name="data"></param>
            public void Add ( Data data ) {
                if (data.Object == null) { return; }
                AddByKey(data);
                AddByTag(data);
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

        void Awake () {
            var count = GetComponents<FindableObject>();
            Assert.IsTrue(count.Length <= 1, "Over hold :: GameObject cannnot hold this component more then 2.");
            mData.Object = gameObject;
            Store.Instance.Add(mData);
        }

    }

}
