using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlayer : MonoBehaviour
{
    private Action<ParticlePlayer> _callback;
    private void OnParticleSystemStopped()
    {
        _callback(this);
    }
    public void Play(Action<ParticlePlayer> callback)
    {
        GetComponent<ParticleSystem>().Play();
        _callback = callback;
    }
}