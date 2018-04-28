using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class MethodChainTest: TestComponent {

    public string DefaultErrorTest () {
        var chain = new MethodChain<string>();
        try {
            chain.Invoke("test");
        }catch(System.Exception) {
            return Pass();
        }
        return Fail("返されるべきエラー処理が失敗しています");
    }

    public string DefaultTest () {
        var chain = new MethodChain<string>();
        bool result = false;
        chain.SetDefault(( str ) =>
        {
            result = true;
            return true;
        });

        try {
            chain.Invoke("test");
        }catch(System.Exception e) {
            return Fail("デフォルト動作の上書きに失敗しています" + e.ToString());
        }

        return result ? Pass() : Fail("コールバックが正常に動作していません");
    }

    public string AddTest_normal_1 () {
        var chain = new MethodChain<string>();
        bool result = false;
        var list = chain.GetProcessHolder();

        list.Add(( str ) =>
        {
            if (str != "test") { return false; };
            result = true;
            return true;
        });

        try {
            chain.Invoke("test");
        } catch (System.Exception e){
            return Fail("追加した関数が正常に呼ばれていません"+e.ToString());
        }
        return result ? Pass() : Fail();
    }

    public string AddTest_error () {
        var chain = new MethodChain<string>();
        var list = chain.GetProcessHolder();

        list.Add(( str ) =>
        {
            if (str != "tt") { return false; };
            return true;
        });
        try {
            chain.Invoke("test");
        } catch (System.Exception ) {
            return Pass(); 
        }
        return Fail("追加した関数が正常に呼ばれていません");
    }

    public string AddTest_normal_some () {
        var chain = new MethodChain<string>();
        bool result = false;
        var list = chain.GetProcessHolder();

        list.Add(( str ) =>
        {
            if (str != "test1") { return false; };
            return true;
        });
        list.Add(( str ) =>
        {
            if (str != "test2") { return false; };
            result = true;
            return true;
        });

        try {
            chain.Invoke("test2");
        } catch (System.Exception e) {
            return Fail("追加した関数が正常に呼ばれていません" + e.ToString());
        }
        return result ? Pass() : Fail();
    }

    public string AddTest_error_some () {
        var chain = new MethodChain<string>();
        bool result = false;
        var list = chain.GetProcessHolder();

        list.Add(( str ) =>
        {
            if (str != "test1") { return false; };
            return true;
        });
        list.Add(( str ) =>
        {
            if (str != "test2") { return false; };
            result = true;
            return true;
        });

        try {
            chain.Invoke("test1");
        } catch (System.Exception e) {
            return Fail("追加した関数が正常に呼ばれていません"+ e.ToString());
        }
        return result == false ? Pass() : Fail();
    }
}
