using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// IRayHitの実質的なAdapter
/// </summary>
public class RayEvent: MonoBehaviour, IRayHit {

    [SerializeField]
    private UnityEvent mOnEnter = new UnityEvent();


    [SerializeField]
    private UnityEvent mOnExit = new UnityEvent();

    public virtual void OnEnter ( Camera c ) {
        mOnEnter.Invoke();
    }

    public virtual void OnExit ( Camera c, float t ) {
        mOnExit.Invoke();
    }

    public virtual void OnStay ( Camera c, float t ) {
    }
}
