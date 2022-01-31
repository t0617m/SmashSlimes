using System;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class PlayerLeftToeBase : MonoBehaviour
{
    [SerializeField] private GameObject _lightParticle;
    [SerializeField] private GameObject _midiumParticle;
    [SerializeField] private GameObject _heavyParticle;
    [SerializeField] private HitColliderRoot _hitColliderRootScript;
    private ParticleSystem _heavyParticleSystem;

    private ParticleSystem _lightParticleSystem;
    private ParticleSystem _midiumParticleSystem;

    private void Start()
    {
        _lightParticleSystem = _lightParticle.GetComponent<ParticleSystem>();
        _midiumParticleSystem = _midiumParticle.GetComponent<ParticleSystem>();
        _heavyParticleSystem = _heavyParticle.GetComponent<ParticleSystem>();


        _lightParticleSystem.Stop();
        _midiumParticleSystem.Stop();
        _heavyParticleSystem.Stop();

        _lightParticle.SetActive(false);
        _midiumParticle.SetActive(false);
        _heavyParticle.SetActive(false);

        _hitColliderRootScript.HitPointIObservable
            .Subscribe(hitColliderType => ParticleStart(hitColliderType))
            .AddTo(this);
    }

    private void ParticleStart(HitColliderType hitColliderType)
    {
        var timer = Observable.Timer(TimeSpan.FromSeconds(0.04f));
        timer.Subscribe(_ =>
            {
                switch (hitColliderType)
                {
                    case HitColliderType.Great:
                        _heavyParticle.SetActive(true);
                        _heavyParticleSystem.Play();
                        break;
                    case HitColliderType.Good:
                        _midiumParticle.SetActive(true);
                        _midiumParticleSystem.Play();
                        break;
                    case HitColliderType.Bad:
                        _lightParticle.SetActive(true);
                        _lightParticleSystem.Play();
                        break;
                }
            })
            .AddTo(this);
    }
}