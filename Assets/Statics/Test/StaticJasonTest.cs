using System;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class StaticJasonTest: TestComponentMulti {

    public void Stringify_OnlyObjectTest ( Result result ) {
        // {}
        var json = JsonStringify.Stringify(null);
        if (json != "{}") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_OnlyTArrayTest ( Result result ) {
        // []
        var json = JsonStringify.Stringify(StArr.To());
        if (json != "[]") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_OnlyTArrayTest_int_1 ( Result result ) {
        var json = JsonStringify.Stringify(StArr.To(1));
        if (json != "[1]") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_OnlyTArrayTest_int_2 ( Result result ) {
        var json = JsonStringify.Stringify(StArr.To(1, 2));
        if (json != "[1,2]") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_OnlyTArrayTest_float ( Result result ) {
        var json = JsonStringify.Stringify(StArr.To(1, 2, 3.14f));
        if (json != "[1,2,3.14]") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_OnlyTArrayTest_string ( Result result ) {
        var json = JsonStringify.Stringify(StArr.To(1, 2, "abs"));
        if (json != "[1,2,\"abs\"]") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_SimpleObjectTest_1 ( Result result ) {
        // { "a":"b" }
        var json = JsonStringify.Stringify(new Dictionary<string, string> {
            { "a","b" }
        });
        if (json != "{\"a\":\"b\"}") { result.Invoke(Fail("Parsed to " + json)); }
        result.Invoke(Pass());
    }

    public void Stringify_SimpleObjectTest_2 ( Result result ) {
        // { "a":"b" , "c":"d"}
        var json = JsonStringify.Stringify(new Dictionary<string, string> {
            { "a","b" }, { "c","d" }

        });
        if (json != "{\"a\":\"b\",\"c\":\"d\"}") {
            result.Invoke(Fail("Parsed to " + json));
        }
        result.Invoke(Pass());
    }

    public void Stringify_ArrayIncludeObjectTest_1 ( Result result ) {
        // {"a":[1,2,3]}
        var json = JsonStringify.Stringify(new ObjDict {
            { "a",StArr.To(1,2,3).ToObjArr() }
        });

        if (json != "{\"a\":[1,2,3]}") {
            result.Invoke(Fail("Parsed to " + json));
        }
        result.Invoke(Pass());
    }

    public void Stringify_ArrayIncludeObjectTest_2 ( Result result ) {
        // {"a":[1,2,3],"c":"d"}

        var json = JsonStringify.Stringify(new ObjDict {
            { "a",StArr.To(1,2,3) }, {"c","d" }
        });
        if (json != "{\"a\":[1,2,3],\"c\":\"d\"}") {
            result.Invoke(Fail("Parsed to " + json));
        }

        result.Invoke(Pass());
    }

    public void Stringify_ObjectIncludeObjectTest ( Result result ) {
        // { a:{e:f, g:h}, c:d }
        var json = JsonStringify.Stringify(new ObjDict {
            { "a", new Dictionary<string,string>{
                { "e","f" }, { "g", "h"} }
            }, {"c","d" }
        });
        if (json != "{\"a\":{\"e\":\"f\",\"g\":\"h\"},\"c\":\"d\"}") {
            result.Invoke(Fail("Parsed to " + json));
        }
        result.Invoke(Pass());
    }

    public void Parse_OnlyStringTest ( Result result ) {
        // "s"
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<string>("\"s\"", ( obj, ret ) =>
        {
            if (obj.GetType() != typeof(string)) {
                result.Invoke(Fail("Illefgurar type " + obj.GetType().Name));
            }
            if (obj.Length != 1) {
                result.Invoke(Fail("Illefgurar element " + obj));
            }
            if (obj != "s") {
                result.Invoke(Fail("Illefgurar element " + obj));
            }
            result.Invoke(Pass());
        });
    }

    public void Parse_OnlyArrayTest ( Result result ) {
        // [1,2,3]
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<Array>("[1,2,3]", ( obj, ret ) =>
        {
            if (obj.GetType().IsArray == false) {
                result.Invoke(Fail("Illefgurar type " + obj.GetType().Name));
            }
            if (obj.GetType() != typeof(String [])) {
                result.Invoke(Fail("Illefgurar type " + obj.GetType().Name));
            }
            var strs = (string []) obj;
            if (strs.Length != 3) {
                result.Invoke(Fail("Illefgurar element " + obj));
            }
            if (strs [0] != "1") {
                result.Invoke(Fail("Illefgurar element " + obj));
            }
            result.Invoke(Pass());
        });
    }

    public void Parse_OnlyObjectTest ( Result result ) {
        // {}
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<ObjDict>("{}", ( obj, ret ) =>
        {
            if (obj.GetType() != typeof(ObjDict)) {
                result.Invoke(Fail("Illefgurar type " + obj.GetType().Name));
            }
            if (obj.Count != 0) {
                result.Invoke(Fail("Illefgurar element " + obj.Count));
            }
            result.Invoke(Pass());
        });
    }

    public void Parse_SimpleObjectTest ( Result result ) {
        // { a:b }
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<ObjDict>(
            "{\"a\":\"b\"}", ( obj, ret ) =>
        {
            if (ret == false) { return; }
            if (obj.GetType() != typeof(ObjDict)) {
                result.Invoke(Fail("Illefgurar type " + obj.GetType().Name));
            }
            if (obj.Count != 1) {
                result.Invoke(Fail("Illefgurar element @count= " + obj.Count));
            }
            if (obj.ContainsKey("a") == false) {
                result.Invoke(Fail("This is not include element"));
            }
            JsonParseData.Parse<string>((string) obj ["a"], ( o, r ) =>
            {
                if (o != "b") {
                    result.Invoke(Fail("Illefgurar element " + obj ["a"]));
                }
                result.Invoke(Pass());
            });
        });
    }

    public void Parse_ObjectTest_2 ( Result result ) {
        // { a:b, c:d }
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<ObjDict>(
            "{\"a\":\"b\",\"c\":\"d\"}"
            , ( obj, ret ) =>
            {

                JsonParseData.Parse<string>((string) obj ["a"], ( o, r ) =>
                {
                    if (o == "b") { return; }
                    result.Invoke(Fail("Illefgurar element " + obj ["a"]));
                });

                JsonParseData.Parse<string>((string) obj ["c"], ( o, r ) =>
                {
                    if (o == "d") { return; }
                    result.Invoke(Fail("Illefgurar element " + obj ["c"]));
                });
                result.Invoke(Pass());
            });
    }

    public void Parse_ArrayIncludeObjectTest ( Result result ) {
        // { a:[1,2,3], c:d }
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });

        JsonParseData.Parse<ObjDict>(
            "{\"a\":\"b\",\"c\":\"d\"}"
            , ( obj, ret ) =>
            {

                JsonParseData.Parse<string>((string) obj ["a"], ( o, r ) =>
                {
                    if (o != "[1,2,3]") { result.Invoke(Fail("Illefgurar element " + obj ["a"])); }
                    JsonParseData.Parse<Array>(o, ( arr, re ) => {

                    });
                });

                JsonParseData.Parse<string>((string) obj ["c"], ( o, r ) =>
                {
                    if (o == "d") { return; }
                    result.Invoke(Fail("Illefgurar element " + obj ["c"]));
                });


                result.Invoke(Pass());
            });
    }

    public void Parse_ObjectIncludeObjectTest ( Result result ) {
        // { a: {c:k, t:y}, c:d }

        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }

    public void Parse_WordTest ( Result result ) {
        // { a:*, c:d }

        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }

    public void Parse_AbnormalWordTest ( Result result ) {
        // { a:*, c:d }
        MultiTask.CountDown(10, ( obj ) =>
        {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }
}
