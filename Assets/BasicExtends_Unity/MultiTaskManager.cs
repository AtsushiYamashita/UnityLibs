namespace BasicExtends {

    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEngine.Assertions;
    using WorkFunc = System.Func<object [], MultiTask.End>;

    public class Worker {

        private WorkFunc mWork;
        private static Action<int> mEnd;
        private int mId = -1;
        private object [] args = null;

        public Worker () {
            SetWork(Resting);
        }

        public Worker SetWork ( WorkFunc work, params object [] obj ) {
            mWork = work;
            return this;
        }

        public Worker SetCleanup ( int id, Action<int> end_cb ) {
            mEnd = end_cb;
            mId = id;
            return this;
        }

        public void Execute () {
            var end = mWork(args);
            if (end == MultiTask.End.FALSE) { return; }
            mEnd(mId);
            mWork = Resting;
        }

        public static MultiTask.End Resting ( object [] obj ) {
            return MultiTask.End.FALSE;
        }

        public bool IsWorking () {
            return mWork != Resting;
        }
    }

    public class MultiTask: Singleton<MultiTask> {

        public enum End { TRUE, FALSE }
        private Dictionary<int, Worker> mWorkers = new Dictionary<int, Worker>();
        private Stack<Worker> mRestingWorker = new Stack<Worker>();
        private float mLastUpdated = 0;
        private bool mIsExistManager = false;
        public void ManagerExist () {
            mIsExistManager = true;
        }

        private Worker GetWorker () {
            if (mRestingWorker.Count > 0) { return mRestingWorker.Pop(); }
            var count = mWorkers.Count;
            var worker = (new Worker().SetCleanup(count, delegate ( int id ) {
                var target = mWorkers [id];
                target.SetWork(Worker.Resting);
                mRestingWorker.Push(target);
            }));
            mWorkers.Add(count, worker);
            return worker;
        }

        public static MultiTask Push ( WorkFunc work, params object [] args ) {
            Assert.IsNotNull(work);
            var done = work(args);
            if (done == End.TRUE) { return Instance; }
            Instance.GetWorker().SetWork(work, args);
            return Instance;
        }

        public int Update () {
            if (mWorkers.IsNull()) { return 0; }
            if (mWorkers.Count < 1) { return 0; }
            var time = Time.time;
            var delta = time - mLastUpdated;
            var isOverUpdate = delta < 0.001;
            if (isOverUpdate && time > 1) { Debug.LogWarning("同一フレームで過剰な更新が行われています"); }
            for (int i = 0; i < mWorkers.Count; i++) { mWorkers [i].Execute(); }
            mLastUpdated = time;
            return mWorkers.Count - mRestingWorker.Count;
        }

        public int Working () {
            return mWorkers.Count - mRestingWorker.Count;
        }

        public static MultiTask CountDown ( int times, WorkFunc work, params object [] obj ) {
            // TimeOver(), active, result
            Assert.IsTrue(Instance.mIsExistManager);
            int count = 0;
            Push( ( not_use ) =>
            {
                //Debug.Log("count = "+ count);
                count++;
                if (count < times) { return End.FALSE; }
                Push(work, obj);
                return End.TRUE;
            },obj);
            return Instance;
        }
    }

    public class MultiTaskManager: MonoBehaviour {
        //private int mWorking = 0;
        private void Start () {
             MultiTask.Instance.ManagerExist();
        }
        private void Update () {
            //mWorking =
            MultiTask.Instance.Update();
            //Debug.Log(Time.deltaTime);
        }
    }
}
