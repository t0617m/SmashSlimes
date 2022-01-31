using System;
using UniRx;
using UnityEngine;

public class HitColliderChild : MonoBehaviour
{
    private readonly Subject<bool> _hitColliderEnterOrExitSubject = new Subject<bool>();

    public IObservable<bool> HitColliderEnterOrExitIObservable => _hitColliderEnterOrExitSubject;

    public GameObject DestructionSlimeObject { set; get; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Slime")
        {
            _hitColliderEnterOrExitSubject.OnNext(true);
            DestructionSlimeObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Slime") _hitColliderEnterOrExitSubject.OnNext(false);
    }
}