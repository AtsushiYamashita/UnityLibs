using System;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;


public class TransformRecorderTest: TestComponentMulti {

    protected override void Init () {
        base.Init();
    }

    public void EventCallTest ( Result result ) {
        MultiTask.CountDown(10, ( obj ) => {
            result.Invoke( Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }

    public void RecTest1 ( Result result ) {
        //保存ができるか

        MultiTask.CountDown(10, ( obj ) => {
            result.Invoke( Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }

    /// <summary>
    /// 複数回の保存ができるか 1
    /// </summary>
    public void RecTest2_multi ( Result result ) {
        // 強制終了条件
        MultiTask.CountDown(10, ( obj ) => {
            result.Invoke(Fail("Time Over"));
            return MultiTask.End.TRUE;
        });


    }

    /// <summary>
    /// 複数回の保存ができるか2 => 大量にあった場合
    /// </summary>
    public void RecTest2_multi2 ( Result result ) {

    }



    //正しく移動量が出せるか
    public void RecTest3_delta ( Result result ) {


    }

}
