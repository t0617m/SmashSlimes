using System;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputUi _inputUiScript;
    [SerializeField] private GameObject _kickParticle;
    private Animator _animator;
    private ParticleSystem _kickParticleSystem;

    public float AnimationSpeed { set; get; }

    private void Start()
    {
        var _gameControllerObject = GameObject.FindWithTag("GameController");
        var _gameControllerScript = _gameControllerObject.GetComponent<GameController>();
        _animator = GetComponent<Animator>();

        _gameControllerScript.ResultIObservable
            .Subscribe(resultPoseValue =>
            {
                if (resultPoseValue == 0) _animator.SetBool("Win", true);
                else if (resultPoseValue == 1) _animator.SetBool("Win1", true);
                else if (resultPoseValue == 2 || resultPoseValue == 3) _animator.SetBool("Win2", true);
                else if (resultPoseValue == 4) _animator.SetBool("Failed", true);
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Continue)
            .Subscribe(_ =>
            {
                _animator.SetBool("Win", false);
                _animator.SetBool("Win1", false);
                _animator.SetBool("Win2", false);
                _animator.SetBool("Failed", false);
            })
            .AddTo(this);

        AnimationSpeed = 1.0f;
        _inputUiScript.InputDownIObservable
            .Subscribe(_ => InputDown())
            .AddTo(this);

        _kickParticleSystem = _kickParticle.GetComponent<ParticleSystem>();
        _kickParticleSystem.Stop();
        _kickParticle.SetActive(false);
    }

    private void FixedUpdate()
    {
        _animator.speed = AnimationSpeed;
    }

    private void InputDown()
    {
        _animator.SetTrigger("Kick");
    }


    //以降アニメーションのイベントから呼ばれる
    private void StartKickAnimationEvent()
    {
        _animator.SetFloat("Speed", 2f);
        _kickParticle.SetActive(true);
        _kickParticleSystem.Play();
    }

    private void KickAnimationEvent()
    {
        _animator.SetFloat("Speed", 0.01f);
        var timer = Observable.Timer(TimeSpan.FromSeconds(0.1f));
        timer.Subscribe(_ => _animator.SetFloat("Speed", 0.5f))
            .AddTo(this);
    }

    private void PeakKickAnimationEvent()
    {
    }

    private void EndKickAnimationEvent()
    {
    }
}