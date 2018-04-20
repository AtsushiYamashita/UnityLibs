using System;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;


public class TransformRecorderTest: TestComponentMulti {

    protected override void Init () {
        base.Init();

        var rececorder = GetComponent<TransformRecorder>();
        int count = 0;
        rececorder.AddRecEvent(( r ) =>
        {
            count++;
            if (count  > 10) { return; }
            transform.position = transform.position + transform.forward;
        });
    }

    public void EventCallTest ( Result result ) {
        var rececorder = GetComponent<TransformRecorder>();
        rececorder.AddRecEvent(( r ) =>
        {
            if (result.IsActive() == false) { return; }
            result.Invoke( Pass());
        });
        MultiTask.CountDown(10, ( obj ) => {
            result.Invoke( Fail("Time Over"));
            return MultiTask.End.TRUE;
        });
    }

    public void RecTest1 ( Result result ) {
        //保存ができるか
        var rececorder = GetComponent<TransformRecorder>();
        rececorder.AddRecEvent(( r ) =>
        {
            if (result.IsActive() == false) { return; }
            var list = r.GetRecList();
            var recDelta = list [0].RelativeSecond();
            if (recDelta < 0) {
                result.Invoke( Fail("異常な値を検知しました。"));
                return;
            }
            if (recDelta > 1) {
                result.Invoke( Fail("処理時間がかかりすぎています。異常と判断しました。"));
                return;
            }

            var e0 = list [0].Value;
            if (e0.POS.magnitude != 0) {
                result.Invoke( Fail("テストオブジェクトが原点に無いようです"));
                return;
            }

            if (e0.ROT.magnitude != 0) {
                result.Invoke( Fail("テストオブジェクトの回転量が異常です。"));
                return;
            }

            if (e0.SCA.x != 1 || e0.SCA.y != 1 || e0.SCA.z != 1) {
                result.Invoke( Fail("テストオブジェクトのスケールが異常です。"+ e0.ToJson()));
                return;
            }
            result.Invoke( Pass());
        });
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

        var rececorder = GetComponent<TransformRecorder>();
        rececorder.AddRecEvent(( r ) =>
        {
            if (result.IsActive() == false) { return; }
            var list = r.GetRecList();
            if (list.Count < 2) { return; }
            var recDelta = list [1].RelativeSecond();
            if (recDelta < 0) {
                result.Invoke(Fail("異常な値を検知しました。"));
                return;
            }
            if (recDelta > 1) {
                result.Invoke(Fail("処理時間がかかりすぎています。異常と判断しました。"));
                return;
            }

            var e0 = list [0].Value;
            if (e0.POS.magnitude != 0) {
                result.Invoke(Fail("テストオブジェクトが原点に無いようです"));
                return;
            }

            var e1 = list [1].Value;
            if (e1.POS.magnitude == 0) {
                result.Invoke(Fail("テストオブジェクトが移動していないようです"));
                return;
            }
            var d = r.Delta(0, 1);
            if (d.Key == 0) {
                result.Invoke(Fail("処理時間が正しく計測できていないようです。"));
                return;
            }
            result.Invoke(Pass());
        });
    }

    /// <summary>
    /// 複数回の保存ができるか2 => 大量にあった場合
    /// </summary>
    public void RecTest2_multi2 ( Result result ) {
        var recorder = GetComponent<TransformRecorder>();
        var record = recorder.GetRecord();

        int testSize = 10;
        int lostSize = testSize - record.GetRecSize();

        UnityEngine.Events.UnityAction<TransformRecord> check = ( r ) =>
         {
             if (result.IsActive() == false) { return; }
             var list = r.GetRecList();

             var sample = list [0].Value;
             if (sample.POS.magnitude == 0) {
                 result.Invoke(Fail("テストオブジェクトが移動していないようです"));
                 return;
             }

             if (sample.POS.x != 0 || sample.POS.z != testSize - lostSize) {
                 result.Invoke(Fail("テストオブジェクトの移動に異常があります"));
                 return;
             }

             result.Invoke(Pass());
         };

        MultiTask.CountDown(testSize * recorder.GetSpan() - 1, ( obj ) => {
            recorder.AddRecEvent( check);
            return MultiTask.End.TRUE;
        });
    }



    //正しく移動量が出せるか
    public void RecTest3_delta ( Result result ) {
        var recorder = GetComponent<TransformRecorder>();
        int testSize = 10;

        UnityEngine.Events.UnityAction<TransformRecord> check = ( rec ) =>
        {
            if (result.IsActive() == false) { return; }

            //var list = r.GetRecList();
            //for (int i = 0; i < 4; i++) {
            //    Debug.LogFormat("{0}<={1}", i, list [i].ToJson());
            //}

            int def = 1;
            var sample = rec.Delta(0, def).Value;
            if (sample.POS.x != 0 || sample.POS.z != def) {
                result.Invoke(Fail("テストオブジェクトの移動量に異常があります"));
                return;
            }

             def = 2;
             sample = rec.Delta(0, def).Value;
            if (sample.POS.x != 0 || sample.POS.z != def) {
                result.Invoke(Fail("テストオブジェクトの移動量に異常があります"));
                return;
            }
            result.Invoke(Pass());
        };

        MultiTask.CountDown(testSize * recorder.GetSpan() - 1, ( obj ) => {
            recorder.AddRecEvent(check);
            return MultiTask.End.TRUE;
        });
    }

}
