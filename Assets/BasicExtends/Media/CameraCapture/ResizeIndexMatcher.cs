using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ある解像度の直列化したBit集合があるとき、
/// その中から一定の密度でサンプリングを行い、
/// 適切な大きさにできるような
/// 適切なIndexを計算するためのクラス
/// </summary>
public class ResizeIndexMatcher {
    private Vector2Int mBefore = new Vector2Int();
    private Vector2Int mAfter = new Vector2Int();
    private byte [] mC32;

    public ResizeIndexMatcher Set ( Vector2Int before, Vector2Int after, byte [] c32 ) {
        mBefore = before;
        mAfter = after;
        mC32 = c32;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    public ResizeIndexMatcher Set ( int bx, int by, int ax, int ay, byte [] c32 ) {
        mBefore.Set(bx, by);
        mAfter.Set(ax, ay);
        mC32 = c32;
        return this;
    }

    /// <summary>
    /// 表示画像の座標ピクセルを渡すと、
    /// 32bit(bgra)の左端bのindexを返す。
    /// これはリトルエンディアン用
    /// </summary>
    public int SamplingStartIndexLittle ( int tx, int ty ) {
        var x = (mBefore.x * tx) / mAfter.x;
        var y = (mBefore.y * ty) / mAfter.y;
        var width = mBefore.x * 4;
        var pos = y * width + x * 4;
        return pos;
    }


    /// <summary>
    /// 表示画像の座標ピクセルを渡すと、
    /// 32bit(bgra)の左端bのindexを返す。
    /// これはビッグエンディアン用
    /// </summary>
    public int SamplingStartIndexBig ( int tx, int ty ) {
        var x1 = (mBefore.x * tx) / mAfter.x;
        var y1 = (mBefore.y * ty) / mAfter.y;
        var x = mBefore.x - x1;
        var y = mBefore.y - y1;
        var width = mBefore.x * 4;
        var pos = y * width + x * 4;
        return pos - 4;
    }
}
