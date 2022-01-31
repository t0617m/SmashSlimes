using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SmashSlimes
{
    public class CompletedForm : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _completeMessage;
        [SerializeField] private Image _screen;
        private Tween _tween;

        private void Start()
        {
            _continueButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _continueButton.gameObject.SetActive(false);
                    EventManager.Send(StateEvent.Continue);
                })
                .AddTo(this);
        }

        private void OnDisable()
        {
            // Tween破棄
            if (DOTween.instance != null) _tween?.Kill();
        }

        public void Hide()
        {
            _continueButton.gameObject.SetActive(false);
            _completeMessage.DOKill();
            _tween = _completeMessage.DOFade(0, 0).SetLink(gameObject);
            _screen.DOKill();
            _tween = _screen.DOFade(0, 0).SetLink(gameObject);
            _screen.raycastTarget = false;
        }

        public void Show()
        {
            var sequence = DOTween.Sequence();
            _tween = sequence.Append(_screen.DOFade(0.15f, 0.5f)).SetLink(gameObject);
            _tween = sequence.AppendInterval(0.5f).SetLink(gameObject);
            _tween = sequence.Append(_completeMessage.DOFade(1, 0)).SetLink(gameObject);
            _tween = sequence.AppendInterval(0.6f).SetLink(gameObject);
            _tween = sequence.OnComplete(() => { _continueButton.gameObject.SetActive(true); }).SetLink(gameObject);
            _screen.raycastTarget = true;
        }
    }
}