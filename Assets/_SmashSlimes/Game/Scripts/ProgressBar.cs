using DG.Tweening;
using SmashSlimes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider _slider;
    private int _slimeValue = 20;

    private void Start()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.value = 20;

        Hide();
        EventManager
            .OnReceiveAsObservable(StateEvent.GameStart)
            .Subscribe(_ => Show())
            .AddTo(this);
        EventManager
            .OnReceiveAsObservable(StateEvent.GameEnd)
            .Subscribe(_ => Hide())
            .AddTo(this);

        var hitColliderRootObject = GameObject.FindWithTag("HitCollider");
        var hitColliderRootScript = hitColliderRootObject.GetComponent<HitColliderRoot>();

        hitColliderRootScript.HitColliderEnterOrExitIObservable
            .Subscribe(isEnter =>
            {
                if (isEnter)
                {
                    _slimeValue--;
                    _slider.value = _slimeValue;
                }
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
        _slimeValue = 20;
        _slider.value = 20;
        gameObject.transform.DOScale(1.0f, 0f).SetLink(gameObject);
    }
}