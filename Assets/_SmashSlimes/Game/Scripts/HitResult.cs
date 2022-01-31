using DG.Tweening;
using SmashSlimes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HitResult : MonoBehaviour
{
    [SerializeField] private HitResultType _slimeType = HitResultType.Great;
    [SerializeField] private HitColliderRoot _hitColliderRootScript;
    private HitResultRoot _hitResultRootScript;
    private Image _image;
    private bool _isShow;
    private Tween _tween;

    private void Start()
    {
        _image = GetComponent<Image>();
        Hide();
        var hitResultRootObject = GameObject.FindWithTag("HitResultRoot");
        _hitResultRootScript = hitResultRootObject.GetComponent<HitResultRoot>();

        var gameControllerObject = GameObject.FindWithTag("GameController");
        var gameControllerScript = gameControllerObject.GetComponent<GameController>();

        gameControllerScript.SlimeValueIObservable
            .Subscribe(x =>
            {
                if (x == 0.0f && _slimeType == HitResultType.Bad2) Show();
            })
            .AddTo(this);

        _hitColliderRootScript.HitPointIObservable
            .Subscribe(hitColliderType =>
            {
                switch (hitColliderType)
                {
                    case HitColliderType.Great:
                        if (_slimeType == HitResultType.Great) Show();
                        break;
                    case HitColliderType.Good:
                        if (_slimeType == HitResultType.Good) Show();
                        break;
                    case HitColliderType.Bad:
                        if (_slimeType == HitResultType.Bad1) Show();
                        break;
                }
            })
            .AddTo(this);

        _hitResultRootScript.ObserveEveryValueChanged(_ => _hitResultRootScript.HitResultObject)
            .Subscribe(showObject =>
            {
                if (showObject != gameObject && _isShow)
                {
                    _isShow = false;
                    _tween.Kill(true);
                }
            })
            .AddTo(this);
    }

    private void SetTimeScale()
    {
        var tweenList = DOTween.TweensByTarget(transform);
        if (tweenList != null)
            foreach (var tween in tweenList)
                tween.Kill();
    }

    private void Hide()
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(0, 0).SetLink(gameObject);
    }

    private void Show()
    {
        _isShow = true;
        _hitResultRootScript.HitResultObject = gameObject;
        var sequence = DOTween.Sequence();
        _tween = sequence.Append(gameObject.transform.DOScale(0.6f, 0.1f).SetLink(gameObject));
        _tween = sequence.Append(gameObject.transform.DOScale(0.4f, 0.3f).SetLink(gameObject));
        _tween = sequence.Append(gameObject.transform.DOMoveY(45.0f, 0.5f).SetRelative(true).SetLink(gameObject));
        _tween = sequence.Join(_image.DOFade(0.0f, 0.5f).SetLink(gameObject));
        _tween = sequence.Append(gameObject.transform.DOScale(0.0f, 0.0f).SetLink(gameObject));
        _tween = sequence.Join(_image.DOFade(1.0f, 00f).SetLink(gameObject));
        _tween = sequence.Append(gameObject.transform.DOMoveY(-45.0f, 0.0f).SetRelative(true)
            .OnComplete(() => _isShow = false)).SetLink(gameObject);
    }
}