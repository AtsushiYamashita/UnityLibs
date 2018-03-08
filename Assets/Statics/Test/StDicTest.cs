
namespace BasicExtends {

    public class StDicTest: TestComponent<StDicTest> {
        public string StringifyTest1 () {
            var dict = new StringDict() { {"test","a" } };
            var str = dict.Stringify();
            if (str != "{test:a}") { return "文字列への変換に失敗しています　" + str; }
            return "";
        }
        public string StringifyTest2 () {
            var dict = new StringDict() { { "test", "a" },{ "t","b" } };
            var str = dict.Stringify();
            if (str != "{test:a,t:b}") { return "文字列への変換に失敗しています　" + str; }
            return "";
        }

        public string KeyNotFoundTest () {
            var dict = new StringDict() { { "test", "a" }, { "t", "b" } };
            var str = dict.Stringify();
            if (dict.KeyNotFound("test")) { return "発見できるはずの要素が見つかっていません"; }
            if (dict.KeyNotFound("a")) { return ""; }
            return "発見できないはずの要素が見つかっています";
        }
    }
}
