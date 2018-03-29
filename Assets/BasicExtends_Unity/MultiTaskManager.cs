namespace BasicExtends {

    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class Worker {

        private Func<MultiTask.End> mWork;
        private static Action<int> mEnd;
        private int mId = -1;

        public Worker () {
            SetWork(Resting);
        }

        public Worker SetWork ( Func<MultiTask.End> work ) {
            mWork = work;
            return this;
        }

        public Worker SetCleanup ( int id, Action<int> end_cb ) {
            mEnd = end_cb;
            mId = id;
            return this;
        }

        public void Execute () {
            var end = mWork();
            if (end == MultiTask.End.FALSE) { return; }
            mEnd(mId);
            mWork = Resting;
        }

        public static MultiTask.End Resting () {
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

        public static MultiTask Push ( Func<End> work ) {
            Assert.IsNotNull(work);
            var done = work();
            if (done == End.TRUE) { return Instance; }
            var ins = Instance.GetWorker().SetWork(work);
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
    }

    public class MultiTaskManager: MonoBehaviour {
        public int mWorking = 0;
        private void Update () {
            mWorking =
            MultiTask.Instance.Update();
            Debug.Log(Time.deltaTime);
        }
    }
}
