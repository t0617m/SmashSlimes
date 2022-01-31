using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SmashSlimes
{
    public class FailedForm : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;

        [SerializeField] private TextMeshProUGUI _failedMessage;

        [SerializeField] private Image _screen;

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

        public void Hide()
        {
            _continueButton.gameObject.SetActive(false);
            _screen.DOKill();
            _screen.DOFade(0, 0).SetLink(gameObject);
            _screen.raycastTarget = false;
            _failedMessage.DOKill();
            _failedMessage.DOFade(0, 0).SetLink(gameObject);
        }

        public void Show()
        {
            _continueButton.gameObject.SetActive(true);
            _screen.DOFade(0.15f, 0.5f).SetLink(gameObject);
            _screen.raycastTarget = true;
            _failedMessage.DOFade(1, 0.2f).SetDelay(0.5f).SetLink(gameObject);
        }
    }
}