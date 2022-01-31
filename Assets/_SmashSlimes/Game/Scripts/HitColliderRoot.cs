using System;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class HitColliderRoot : MonoBehaviour
{
    [SerializeField] private InputUi _inputUiScript;
    [SerializeField] private HitColliderChild _greatColliderScript;
    [SerializeField] private HitColliderChild _goodColliderScript;
    [SerializeField] private HitColliderChild _badColliderScript;
    [SerializeField] private HitColliderChild _AssistUiColliderScript;
    private readonly Subject<GameObject> _destructionSlimeSubject = new Subject<GameObject>();

    private readonly Subject<bool> _hitColliderEnterOrExitSubject = new Subject<bool>();

    private readonly Subject<HitColliderType> _hitPointSubject = new Subject<HitColliderType>();
    private bool _isBad;
    private bool _isGood;

    private bool _isGreat;

    public IObservable<HitColliderType> HitPointIObservable => _hitPointSubject;

    public IObservable<GameObject> DestructionSlimeIObservable => _destructionSlimeSubject;

    public IObservable<bool> HitColliderEnterOrExitIObservable => _hitColliderEnterOrExitSubject;

    private void Start()
    {
        _greatColliderScript.HitColliderEnterOrExitIObservable
            .Subscribe(isEnter =>
            {
                if (isEnter) _isGreat = true;
                else _isGreat = false;
            })
            .AddTo(this);

        _goodColliderScript.HitColliderEnterOrExitIObservable
            .Subscribe(isEnter =>
            {
                if (isEnter) _isGood = true;
                else _isGood = false;
            })
            .AddTo(this);

        _badColliderScript.HitColliderEnterOrExitIObservable
            .Subscribe(isEnter =>
            {
                if (isEnter) _isBad = true;
                else _isBad = false;
                //_hitColliderEnterOrExitSubject.OnNext(false);
            })
            .AddTo(this);
        _AssistUiColliderScript.HitColliderEnterOrExitIObservable
            .Subscribe(_isEnter =>
            {
                if (_isEnter) _hitColliderEnterOrExitSubject.OnNext(true);
            })
            .AddTo(this);

        _inputUiScript.InputDownIObservable
            .Subscribe(_ => KickProcess())
            .AddTo(this);

        _greatColliderScript.ObserveEveryValueChanged(_ => _greatColliderScript.DestructionSlimeObject)
            .Subscribe(DestructionSlimeObject => _destructionSlimeSubject.OnNext(DestructionSlimeObject))
            .AddTo(gameObject);

        _goodColliderScript.ObserveEveryValueChanged(_ => _goodColliderScript.DestructionSlimeObject)
            .Subscribe(DestructionSlimeObject => _destructionSlimeSubject.OnNext(DestructionSlimeObject))
            .AddTo(gameObject);

        _badColliderScript.ObserveEveryValueChanged(_ => _badColliderScript.DestructionSlimeObject)
            .Subscribe(DestructionSlimeObject => _destructionSlimeSubject.OnNext(DestructionSlimeObject))
            .AddTo(gameObject);
    }

    private void KickProcess()
    {
        if (_isGreat && _isGood && _isBad) _hitPointSubject.OnNext(HitColliderType.Great);
        else if (_isGood && _isBad) _hitPointSubject.OnNext(HitColliderType.Good);
        else if (_isBad) _hitPointSubject.OnNext(HitColliderType.Bad);
    }
}