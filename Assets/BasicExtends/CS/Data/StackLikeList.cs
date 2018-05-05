namespace BasicExtends {
    using System.Collections.Generic;

    /// <summary>
    /// 基本的にはListだが、
    /// 先頭要素についてはStackのようにpush,popができる
    /// </summary>
    [System.Serializable]
    public class StackLikeList<T> : List<T> {

        public StackLikeList<T> Push ( T t ) {
            Insert(0, t);
            return this;
        }

        public bool Contain ( T t ) {
            return Contains(t);
        }

        public CheckedRet<T> Pop () {
            if (Count < 1) { return CheckedRet<T>.Fail(); }
            var r = this [0];
            RemoveAt(0);
            return CheckedRet<T>.Gen(true, r);
        }

        public T Head
        {
            get
            {
                if (Count < 1) { return default(T); }
                return this [0];
            }
        }
    }
}