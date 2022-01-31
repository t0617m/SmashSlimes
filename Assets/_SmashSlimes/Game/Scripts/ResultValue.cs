using DG.Tweening;
using SmashSlimes;
using TMPro;
using UniRx;
using UnityEngine;

public class ResultValue : MonoBehaviour
{
    public int GreatCount { set; get; }
    public int GoodCount { set; get; }
    public int BadCount { set; get; }

    private void Start()
    {
        Hide();
        var text = gameObject.GetComponent<TextMeshProUGUI>();

        var gameControllerObject = GameObject.FindWithTag("GameController");
        var gameControllerScript = gameControllerObject.GetComponent<GameController>();

        gameControllerScript.SlimeValueIObservable
            .Subscribe(hitValue =>
            {
                if (hitValue == 1.0f) GreatCount++;
                else if (hitValue == 0.5f) GoodCount++;
                else if (hitValue == 0.01f || hitValue == 0.0f) BadCount++;
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.GameStart)
            .Subscribe(_ =>
            {
                GreatCount = 0;
                GoodCount = 0;
                BadCount = 0;
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Complete)
            .Subscribe(_ =>
            {
                Show();
                text.text = "GREAT:" + GreatCount + "\nGOOD:" + GoodCount + "\nBAD:" + BadCount;
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Continue)
            .Subscribe(_ => { Hide(); })
            .AddTo(this);
    }

    private void Hide()
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(0, 0).SetLink(gameObject);
    }

    private void Show()
    {
        gameObject.transform.DOScale(1.0f, 1.0f).SetLink(gameObject);
    }
}