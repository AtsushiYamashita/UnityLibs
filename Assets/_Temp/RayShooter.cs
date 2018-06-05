using UnityEngine;
using BasicExtends;

/// <summary>
/// RayShooterによってRayが当たるとこれらのメッセージを受け取る
/// </summary>
public interface IRayHit {
    void OnEnter ( Camera c );
    void OnStay ( Camera c, float t );
    void OnExit ( Camera c, float t );
}

/// <summary>
/// IRayHitをもつオブジェクトにRayが当たったらメッセージングする
/// Cameraから飛ばすことを前提にしている。
/// </summary>
[RequireComponent(typeof(Camera))]
public class RayShooter: MonoBehaviour {

    private IRayHit mPrev = null;

    /// <summary>
    /// 指定していなければ自分自身のカメラをもとにRayを飛ばす
    /// </summary>
    [SerializeField]
    private Camera mFrom = null;

    private float mHitStart = -1;

    private void Reset () {
        mFrom =  GetComponent<Camera>();
    }

    private void Start () {
        mFrom = mFrom ?? GetComponent<Camera>();
    }

    private void ExitToNull () {
        if (mPrev.IsNull()) { return; }
        mPrev.OnExit(mFrom, Time.time - mHitStart);
        mHitStart = -1;
        mPrev = null;
        Debug.Log("ExitToNull");
    }

    private void ExitToObject () {
        if (mPrev.IsNull()) { return; }
        mPrev.OnExit(mFrom, Time.time - mHitStart);
        mHitStart = Time.time;
        mPrev = null;
        Debug.Log("ExitToObject" );
    }

    private CheckedRet<IRayHit> HitCheck () {
        Ray ray = new Ray(mFrom.transform.position, mFrom.transform.forward);
        RaycastHit hit;
        var notHit = Physics.Raycast(ray, out hit, 10.0f) == false;
        if (notHit) { ExitToNull(); return CheckedRet<IRayHit>.Fail(); }

        var hitObj = hit.collider.GetComponent<IRayHit>();
        if (hitObj == null) { ExitToNull(); return CheckedRet<IRayHit>.Fail(); }
        if (hitObj != mPrev) { ExitToObject(); }
        return new CheckedRet<IRayHit>().Set(true, hitObj);
    }

    private void EnterCall ( IRayHit hit ) {
        if (mPrev.IsNotNull()) { return; }
        mHitStart = Time.time;
        mPrev = hit;
        hit.OnEnter(mFrom);
        return;
    }

    private void StayEventCall ( IRayHit hit ) {
        hit.OnStay(mFrom, Time.time - mHitStart);
    }

    private void Update () {
        var hit = HitCheck();
        if (hit.Key == false) { return; }
        EnterCall(hit.Value);
        StayEventCall(hit.Value);
    }
}
