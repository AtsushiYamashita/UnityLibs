using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AlbedoColorChange : MonoBehaviour {

    private const string COLOR = "_Color";
    private Color mDefault;
    [SerializeField]
    private Color [] mColors = new Color [2];

    private Material GetColorMaterial () {
        var material = GetComponent<Renderer>().material;
        var isExist = material.HasProperty(COLOR);
        if (!isExist) { throw new System.Exception("this material cannot color change"); }
        return material;
    }

    private void Start () {
        mDefault = GetColorMaterial().GetColor(COLOR);
    }

    /// <summary>
    /// マテリアルのアルベドカラーを変化させる。
    /// index が 0より下ならデフォルトのカラーになる。
    /// </summary>
    /// <param name="index"></param>
    public void Change(int index) {
        var i = Mathf.Min(index, mColors.Length);
        var c = i < 0 ? mDefault : mColors[i];
        GetColorMaterial().SetColor(COLOR, c);
    }
}
