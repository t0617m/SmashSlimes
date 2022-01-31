using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AssistUi : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprite = new Sprite[3];
    private Image _image;
    private readonly CompositeDisposable _slimeCompositeDisposable = new CompositeDisposable();

    private void Start()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
        var hitColliderRootObject = GameObject.FindWithTag("HitCollider");
        var hitColliderRootScript = hitColliderRootObject.GetComponent<HitColliderRoot>();

        hitColliderRootScript.HitColliderEnterOrExitIObservable
            .Subscribe(isEnter =>
            {
                if (isEnter)
                {
                    _image.enabled = true;
                    gameObject.transform.DOScale(2, 0f).SetLink(gameObject);
                    _image.DOFade(1.0f, 0f).SetLink(gameObject);
                    _image.sprite = _sprite[0];
                    _slimeCompositeDisposable.Clear();
                    _image.DOFade(1.0f, 0.3f).OnComplete(() => OnAnimation()).SetLink(gameObject);
                }
            })
            .AddTo(this);
        hitColliderRootScript.HitPointIObservable
            .Subscribe(isEnter => OnAnimation())
            .AddTo(this);

        var _slimeCreateObject = GameObject.FindWithTag("SlimeCreate");
        var _slimeCreateScript = _slimeCreateObject.GetComponent<SlimeCreate>();

        _slimeCreateScript.SlimeCreatedIObservable
            .Subscribe(slimeGameObject =>
            {
                var _slimeScript = slimeGameObject.GetComponent<Slime>();
                _slimeScript.SlimeCountIObservable
                    .Subscribe(value =>
                    {
                        _image.enabled = true;
                        gameObject.transform.DOScale(2, 0f).SetLink(gameObject);
                        _image.DOFade(1.0f, 0f).SetLink(gameObject);
                        if (value == 1) _image.sprite = _sprite[1];
                        else _image.sprite = _sprite[2];
                        _image.DOFade(1.0f, 0.3f).OnComplete(() => OnAnimation()).SetLink(gameObject);
                    })
                    .AddTo(_slimeCompositeDisposable);
            })
            .AddTo(this);
    }

    private void OnAnimation()
    {
        _image.DOFade(0.0f, 0.2f).SetLink(gameObject)
            .OnComplete(() => _image.enabled = false).SetLink(gameObject);
    }
}