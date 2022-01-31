using DG.Tweening;
using UniRx;
using UnityEngine;

public class StartMessage : MonoBehaviour
{
    private void Start()
    {
        Hide();

        var gameControllerObject = GameObject.FindWithTag("GameController");
        var gameControllerScript = gameControllerObject.GetComponent<GameController>();

        gameControllerScript.ExplanationMessageIObservable
            .Subscribe(_isShow =>
            {
                if (_isShow) Show();
                else Hide();
            })
            .AddTo(this);
    }

    private void Hide()
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(0, 0).SetLink(gameObject);
    }

    private void Show()
    {
        gameObject.transform.DOScale(2.0f, 0.5f).SetLink(gameObject);
    }
}