namespace BasicExtends {
    using System;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class TextureDrawer {

        Pair<int, int> mSize = new Pair<int, int>();
        Texture2D texture;


        public void SetWidth ( int w ) {
            mSize.W = w;
        }

        public Pair<int, int> GetSize () {
            return mSize;
        }

        public TextureDrawer SetSize ( Pair<int, int> size ) {
            Assert.IsNotNull(texture);
            mSize = size;
            texture.Resize(size.W, size.Y);
            return this;
        }

        public TextureDrawer SetupTexture ( Renderer renderer, Pair<int, int> size ) {
            mSize = size;
            texture = new Texture2D(mSize.W, mSize.H);
            renderer.material.mainTexture = texture;
            return this;
        }

        public TextureDrawer DrawLine ( Func<int, Pair<int, int>> func, Color clr, int start = 0, int end = -1 ) {
            var e = (end < start ? mSize.W : end);
            for (int x = start; x < e; x++) {
                var yrange = func(x);
                yrange.Min = yrange.Min < 0 ? 0 : yrange.Min;
                for (int y = yrange.Min; y < yrange.Max; y++)
                    texture.SetPixel(x, y, clr);
            }
            return this;
        }

        public TextureDrawer DrawLine ( Vector2 s, Vector2 e, Color clr ) {
            var dif = e - s;
            var fix = Vector2.one * 0.5f;
            for (int i = 0; i < dif.magnitude; i++) {
                var p = s + dif.normalized * i + fix;
                texture.SetPixel((int) p.x, (int) p.y, clr);
            }
            return this;
        }

        /// <summary>
        /// 変更を反映する
        /// </summary>
        public TextureDrawer Apply () {
            texture.Apply();
            return this;
        }
    }


    //public class DrawTask {

    //    List<Draw> mList = new List<Draw>();
    //    public static DrawTask Gen () {
    //        return new DrawTask();
    //    }

    //    public delegate void Draw ( Texture2D tex, Vector2 pos, Vector2 rot, Vector2 sca );
    //    public void DrawInvoke ( Texture2D tex, Vector2 pos, Vector2 rot, Vector2 sca ) {
    //        foreach (var d in mList) { d(tex, pos, rot, sca); }
    //    }

    //    public static readonly Draw sPoint = ( t, p, r, s ) =>
    //    {
    //        var arr = new Vector2 [] { new Vector2(0, 0) };
    //        Matrix4x4 mat = new Matrix4x4();
    //    };
    //    public static readonly Draw sHolizontal = ( t, p, r, s ) =>
    //    {
    //        var arr = new Vector2 [] { new Vector2(-1, 0), new Vector2(-1, 0) };
    //        Matrix4x4 mat = new Matrix4x4();
    //    };
    //}
}