namespace BasicExtends {
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// sarchKey = nameかtagを使って、
    /// 登録したり
    /// </summary>
    public class FindStore: Singleton<FindStore> {
        private Dictionary<string, FindableData> mDatas = new Dictionary<string, FindableData>();
        private Dictionary<string, List<FindableData>> mTagedDatas = new Dictionary<string, List<FindableData>>();

        private FindStore () { } // seal

        /// <summary>
        /// Clear all stored data.
        /// You will use game data reset.
        /// </summary>
        public void Clear () {
            mDatas = new Dictionary<string, FindableData>();
            mTagedDatas = new Dictionary<string, List<FindableData>>();
        }

        public int CountbyTag ( string str ) {
            if (mTagedDatas.ContainsKey(str) == false) { return 0; }
            return mTagedDatas [str].Count;
        }

        public void AddByKey ( FindableData data ) {
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

        public void AddByTag ( FindableData data ) {
            foreach (var tag in data.Tags) {
                if (!mTagedDatas.ContainsKey(tag)) { mTagedDatas.Add(tag, new List<FindableData>()); }
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
            FindableData data = null;
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
            List<FindableData> datas = null;
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
