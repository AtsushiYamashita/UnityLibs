namespace BasicExtends {

    public class StaticGameObjectTest: TestComponent {

        public string ChildrenTest () {
            var children = gameObject.Children();
            var count = transform.childCount;
            for (int i = 0; i < count; i++) {
                var c1 = transform.GetChild(i).IID();
                var c2 = children [i].IID();
                if (c1 != c2) {
                    return "子供の配列ではないようです。"
                        + string.Format("{0},{1}", c1, c2);
                }
            }
            return "";
        }

        public string BrotherTest () {
            var parent = transform.parent.gameObject;
            var cren = parent.Children();
            var brt = gameObject.FindBrother("ccc");
            if (cren [1].IID() != brt.IID()) {
                return "正しい兄弟関係のオブジェクトではありません";
            }
            try {
                gameObject.FindBrother("bbb");
            } catch {
                return "";
            }
            return "取得すべきでないオブジェクトの取得が行われています";
        }

        public string ChildTest () {
            var c1 = gameObject.Children() [0].IID();
            var c2 = gameObject.FindChild("bbb").IID();
            if (c1 != c2) {
                return "正しい親子関係のオブジェクトではありません";
            }
            try {
                gameObject.FindChild("ccc");
            } catch {
                return "";
            }
            return "取得すべきでないオブジェクトの取得が行われています";
        }

    }
}
