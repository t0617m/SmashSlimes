using System;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _resultValue;
    [SerializeField] private float _slimeMaxValue = 20;

    private readonly Subject<bool> _explanationMessageSubject = new Subject<bool>();
    private bool _gameEnd;
    private float _greatCount;
    private int _intervalCounter;
    private int _intervalCounter6;
    private readonly CompositeDisposable _intervalDisposable = new CompositeDisposable();
    private bool _IntervalWait;
    private bool _isExplanation;
    private bool _isTimer;

    private readonly Subject<int> _resultSubject = new Subject<int>();
    private ResultValue _resultValueScript;
    private readonly CompositeDisposable _slimeCompositeDisposable = new CompositeDisposable();

    private readonly Subject<int> _slimeCreateSubject = new Subject<int>();

    private readonly Subject<float> _slimeValueSubject = new Subject<float>();
    private float _timer;
    private float _turnCounter = 1;

    public IObservable<int> SlimeCreateIObservable => _slimeCreateSubject;

    public IObservable<int> ResultIObservable => _resultSubject;

    public IObservable<bool> ExplanationMessageIObservable => _explanationMessageSubject;

    public IObservable<float> SlimeValueIObservable => _slimeValueSubject;

    private void Start()
    {
        _resultValueScript = _resultValue.GetComponent<ResultValue>();

        var _slimeCreateObject = GameObject.FindWithTag("SlimeCreate");
        var _slimeCreateScript = _slimeCreateObject.GetComponent<SlimeCreate>();

        _slimeCreateScript.SlimeCreatedIObservable
            .Subscribe(slimeGameObject =>
            {
                var _slimeScript = slimeGameObject.GetComponent<Slime>();
                _slimeScript.SlimeValueIObservable
                    .Subscribe(value =>
                    {
                        _greatCount += value;
                        _slimeValueSubject.OnNext(value);
                    })
                    .AddTo(_slimeCompositeDisposable);
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.GameStart)
            .Subscribe(_ =>
            {
                Observable.Interval(TimeSpan.FromSeconds(0.4f))
                    .Subscribe(x => IntervalProcess())
                    .AddTo(_intervalDisposable);
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.GameEnd)
            .Subscribe(_ => _intervalDisposable.Clear())
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Continue)
            .Subscribe(_ =>
            {
                _turnCounter = 1;
                _greatCount = 0;
                _slimeCompositeDisposable.Clear();
            })
            .AddTo(this);
    }

    private void Update()
    {
        var CountTotal = (float) _resultValueScript.GreatCount + _resultValueScript.GoodCount +
                         _resultValueScript.BadCount;
        if (_gameEnd) _isTimer = true;
        if (_isTimer)
        {
            _timer += Time.deltaTime;
            if (_timer >= 3.0f)
            {
                _timer = 0;
                _isTimer = false;
                CountTotal = _slimeMaxValue;
            }
        }
        else
        {
            _timer = 0;
        }

        if (CountTotal >= _slimeMaxValue && _gameEnd)
        {
            EventManager.Send(StateEvent.GameEnd);
            EventManager.Send(StateEvent.Complete);
            Judgement();
            _gameEnd = false;
        }
    }

    private void IntervalProcess()
    {
        if (_intervalCounter < 6) _intervalCounter++;
        else _intervalCounter = 0;
        if (_turnCounter > _slimeMaxValue)
        {
            _gameEnd = true;
            _IntervalWait = false;
        }

        if (_turnCounter < _slimeMaxValue * 0.5f)
        {
            if (_isExplanation)
            {
                _intervalCounter6++;
                if (_intervalCounter6 >= 6)
                {
                    _intervalCounter6 = 0;
                    _turnCounter++;
                    _slimeCreateSubject.OnNext(0);
                }
            }
            else
            {
                _explanationMessageSubject.OnNext(true);
                if (_intervalCounter6 <= 2 && _intervalCounter == 6) _intervalCounter6++;
                if (_intervalCounter6 == 1 && _intervalCounter == 6)
                {
                    _isExplanation = true;
                    _slimeCreateSubject.OnNext(0);
                    _turnCounter++;
                    _explanationMessageSubject.OnNext(false);
                    _intervalCounter6 = 0;
                }
            }
        }
        else
        {
            if (_turnCounter != 10 && _turnCounter != 16)
            {
                if (_IntervalWait)
                {
                    _intervalCounter6++;
                    if (_intervalCounter6 <= 2)
                    {
                    }
                    else
                    {
                        _intervalCounter6 = 0;
                        _turnCounter++;
                        _slimeCreateSubject.OnNext(1);
                    }
                }
                else
                {
                    if (_intervalCounter >= 1) _IntervalWait = true;
                }
            }
            else
            {
                _IntervalWait = false;
                _intervalCounter6++;
                if (_intervalCounter6 == 6) _slimeCreateSubject.OnNext(2);
                if (_intervalCounter6 == 15)
                {
                    _turnCounter++;
                    _intervalCounter6 = 0;
                }
            }
        }
    }

    private void Judgement()
    {
        if (_greatCount >= _slimeMaxValue) _resultSubject.OnNext(0);
        else if (_greatCount < _slimeMaxValue && _greatCount >= _slimeMaxValue * 0.75f) _resultSubject.OnNext(1);
        else if (_greatCount < _slimeMaxValue * 0.75f && _greatCount >= _slimeMaxValue * 0.5f) _resultSubject.OnNext(2);
        else if (_greatCount < _slimeMaxValue * 0.5f && _greatCount >= _slimeMaxValue * 0.25f) _resultSubject.OnNext(3);
        else _resultSubject.OnNext(4);
    }
}