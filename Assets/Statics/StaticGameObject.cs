using System.Collections.Generic;
using UnityEngine;
using System;

namespace BasicExtends {
    public static class StaticGameObject {

        /// <summary>
        /// UnityでGameObjectの子供として扱われているオブジェクトを、
        /// リスト形式で一度に取得できます
        /// 
        /// Usage
        /// var childrenList = gameObject.Children();
        /// foreach(var child in childrenList){ .... }
        /// </summary>
        /// <param name="obj">子供を取得したいオブジェクト</param>
        /// <returns>取得された子供</returns>
        public static List<GameObject>  Children ( this GameObject obj ) {
            var children = new List<GameObject>();
            var tr = obj.transform;
            for (int i = 0; i < tr.childCount; i++) {
                var ch = tr.GetChild(i).gameObject;
                if (ch.activeSelf == false) { continue; }
                children.Add(ch);
            }
            return children;
        }

        public static GameObject FindBrother ( this GameObject obj, string name ) {
            var tr = obj.transform.parent;
            for (int i = 0; i < tr.childCount; i++) {
                var ch = tr.GetChild(i).gameObject;
                if (ch.activeSelf == false) { continue; }
                if (ch.name == name) { return ch; }
            }
            throw new Exception(name + " is not found in brother");
        }

        public static GameObject FindChild ( this GameObject obj, string name ) {
            var tr = obj.transform;
            for (int i = 0; i < tr.childCount; i++) {
                var ch = tr.GetChild(i).gameObject;
                if (ch.activeSelf == false) { continue; }
                if (ch.name == name) { return ch; }
            }
            throw new Exception(name + " is not found in child");
        }

        public static int IID ( this GameObject obj ) {
            return obj.GetInstanceID();
        }

        public static int IID ( this Component obj ) {
            return obj.gameObject.GetInstanceID();
        }
    }
}
