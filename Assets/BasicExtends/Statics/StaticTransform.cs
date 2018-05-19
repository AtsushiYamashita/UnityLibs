namespace BasicExtends
{
    using UnityEngine;

    public static class StaticTransform
    {
        /// <summary>
        /// ワールド座標で指定位置に移動する
        /// </summary>
        public static void MoveTo(this Transform self, Vector3 to)
        {
            self.position = to;
        }

        /// <summary>
        /// ローカル座標で指定位置に移動する
        /// </summary>
        public static void MoveToLocal(this Transform self, Vector3 to)
        {
            self.localPosition = to;
        }

        /// <summary>
        /// ワールド座標で指定方向に回転する
        /// </summary>
        public static void RotateTo(this Transform self, Quaternion to)
        {
            self.rotation = to;
        }

        /// <summary>
        /// ローカル座標で指定方向に回転する
        /// </summary>
        public static void RotateToLocal(this Transform self, Quaternion to)
        {
            self.localRotation = to;
        }
    }
}
