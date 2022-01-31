using System;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class InputUi : MonoBehaviour
{
    private readonly Subject<bool> _inputDownSubject = new Subject<bool>();
    private bool _isInput;

    public IObservable<bool> InputDownIObservable => _inputDownSubject;

    private void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => ButtonDown())
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.GameStart)
            .Subscribe(_ => _isInput = true)
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.GameEnd)
            .Subscribe(_ => _isInput = false)
            .AddTo(this);
    }

    private void ButtonDown()
    {
        if (_isInput) _inputDownSubject.OnNext(true);
    }
}