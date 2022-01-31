using DG.Tweening;
using SmashSlimes;
using UniRx;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private void Start()
    {
        EventManager
            .OnReceiveAsObservable(StateEvent.GameStart)
            .Subscribe(_ => gameObject.transform.DOMove(new Vector3(-2.5f, 2f, 15f), 0.5f).SetLink(gameObject))
            .AddTo(this);

        var gameControllerObject = GameObject.FindWithTag("GameController");
        var gameControllerScript = gameControllerObject.GetComponent<GameController>();

        gameControllerScript.SlimeValueIObservable
            .Subscribe(hitValue =>
            {
                if (hitValue == 1.0f) transform.DOPunchPosition(new Vector3(0f, 0.1f, 0), 0.5f, 5);
            })
            .AddTo(this);
    }
}