using DG.Tweening;
using SmashSlimes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class YourTitle : MonoBehaviour
{
    [SerializeField] private MyObjectId _myObjectId = MyObjectId.Id0;
    [SerializeField] private Sprite[] _sprite = new Sprite[6];
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        Hide();
        EventManager
            .OnReceiveAsObservable(StateEvent.Complete)
            .Subscribe(_ =>
            {
                if (_myObjectId == MyObjectId.Id0)
                {
                    Show();
                    _image.sprite = _sprite[0];
                }
            })
            .AddTo(this);

        EventManager
            .OnReceiveAsObservable(StateEvent.Continue)
            .Subscribe(_ => Hide())
            .AddTo(this);

        if (_myObjectId != MyObjectId.Id0)
        {
            var gameControllerObject = GameObject.FindWithTag("GameController");
            var gameControllerScript = gameControllerObject.GetComponent<GameController>();

            gameControllerScript.ResultIObservable
                .Subscribe(x =>
                {
                    if (x == 0) _image.sprite = _sprite[1];
                    else if (x == 1) _image.sprite = _sprite[2];
                    else if (x == 2) _image.sprite = _sprite[3];
                    else if (x == 3) _image.sprite = _sprite[4];
                    else if (x == 4) _image.sprite = _sprite[5];
                    Show();
                })
                .AddTo(this);
        }
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