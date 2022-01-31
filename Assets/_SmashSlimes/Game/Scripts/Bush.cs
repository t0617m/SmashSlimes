using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private AudioClip _grassSound;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.20f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Slime") _audioSource.PlayOneShot(_grassSound);
    }
}