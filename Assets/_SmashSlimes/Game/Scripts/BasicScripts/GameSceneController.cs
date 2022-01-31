using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace SmashSlimes
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField] private UIPresenter _uiPresenter;
        [SerializeField] private ParticleSystem _confettiEffect;
        [SerializeField] private TextAsset levelMapText;
        [SerializeField] private AudioClip sound1;
        private readonly List<string> _levelMap = new List<string>();
        private AudioSource _audioSource;
        private bool _isContinued;
        private bool _isFirst;
        private bool _isGameEnd;
        private bool _isGameStart;
        private bool _isLevelCompleted;
        private bool _isLevelFailed;

        private readonly Subject<int> _stageCreateSubject = new Subject<int>();
        private GameObject _stageObject;

        public IObservable<int> StageCreateIObservable => _stageCreateSubject;

        private List<string> LevelMap
        {
            get
            {
                if (_levelMap.Count == 0)
                {
                    var stringReader = new StringReader(levelMapText.text);
                    while (stringReader.Peek() > -1)
                    {
                        var line = stringReader.ReadLine();
                        _levelMap.Add(line);
                    }
                }

                return _levelMap;
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.volume = 0.15f;

            EventManager
                .OnReceiveAsObservable(StateEvent.GameStart)
                .Subscribe(_ => OnGameStart())
                .AddTo(this);
            EventManager
                .OnReceiveAsObservable(StateEvent.Complete)
                .Subscribe(_ => OnComplete())
                .AddTo(this);
            EventManager
                .OnReceiveAsObservable(StateEvent.Failed)
                .Subscribe(_ => OnFailed())
                .AddTo(this);
            EventManager
                .OnReceiveAsObservable(StateEvent.Continue)
                .Subscribe(_ => OnContinue())
                .AddTo(this);
            EventManager
                .OnReceiveAsObservable(StateEvent.GameEnd)
                .Subscribe(_ => OnGameEnd())
                .AddTo(this);
            _audioSource.volume = 0.1f;
            _audioSource.Play();
        }

        private void Start()
        {
            GamePlay();
        }

        public async void GamePlay()
        {
            while (true)
            {
                _isLevelCompleted = false;
                _isLevelFailed = false;
                _isContinued = false;
                _isGameStart = false;
                _isGameEnd = false;

                var currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
                CreateStage(LevelMap, currentLevel);

                if (!_isFirst) _isFirst = true;
                else EventManager.Send(StateEvent.GameStart);
                await UniTask.WaitUntil(() => _isGameStart);
                _uiPresenter.OnGameStart();

                await UniTask.WaitUntil(() => _isGameEnd);

                await UniTask.WaitUntil(() => _isLevelCompleted || _isLevelFailed);
                if (_isLevelFailed)
                {
                    _uiPresenter.OnFailed();
                }
                else
                {
                    _confettiEffect.Play();
                    currentLevel++;
                    PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                    _uiPresenter.OnCompleted();
                }

                await UniTask.WaitUntil(() => _isContinued);
                _confettiEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void CreateStage(List<string> levelMap, int level)
        {
            if (_stageObject != null) Destroy(_stageObject);
            _stageObject = null;
            var stageLevel = level;
            if (levelMap.Count < level)
            {
                level %= levelMap.Count;
                if (level == 0) level = levelMap.Count;
            }

            var prefab = (GameObject) Resources.Load("Game/" + levelMap[level - 1]);
            _stageObject = Instantiate(prefab, transform);
            _stageCreateSubject.OnNext(stageLevel);
        }

        public void OnGameStart()
        {
            _audioSource.Stop();
            _audioSource.volume = 0.15f;
            _audioSource.Play();
            _isGameStart = true;
        }

        public void OnFailed()
        {
            _isLevelFailed = true;
        }

        private void OnContinue()
        {
            _isContinued = true;
            _uiPresenter.OnContinue();
        }

        private void OnComplete()
        {
            _isLevelCompleted = true;
        }

        private void OnGameEnd()
        {
            _isGameEnd = true;
            _audioSource.DOFade(0.0f, 1.0f).OnComplete(() => _audioSource.Stop()).SetLink(gameObject);
        }
    }
}