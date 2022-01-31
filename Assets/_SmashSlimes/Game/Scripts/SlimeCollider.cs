using UnityEngine;

public class SlimeCollider : MonoBehaviour
{
    private bool _isCollider;
    private float _time;

    private void Update()
    {
        if (_isCollider)
        {
            _time += Time.deltaTime;
            if (_time >= 0.15f)
            {
                _time = 0;
                _isCollider = false;
            }
        }
        else
        {
            _time = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LeftToeBase") _isCollider = true;
    }
}