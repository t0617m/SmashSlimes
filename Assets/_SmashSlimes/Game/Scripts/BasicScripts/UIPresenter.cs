using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SmashSlimes
{
    public class UIPresenter : MonoBehaviour
    {
        [SerializeField] private InGameForm _inGameForm;
        [SerializeField] private CompletedForm _completedForm;
        [SerializeField] private FailedForm _failedForm;
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _titleForm;

        private void Awake()
        {
            _startButton
                .OnClickAsObservable()
                .Subscribe(_ => EventManager.Send(StateEvent.GameStart))
                .AddTo(this);

            _completedForm.Hide();
            _failedForm.Hide();
            _inGameForm.Show();
            _titleForm.SetActive(true);
        }

        public void OnGameStart()
        {
            _startButton.gameObject.SetActive(false);
            _titleForm.SetActive(false);
            _completedForm.Hide();
            _failedForm.Hide();
            _inGameForm.Show();
        }

        public void OnCompleted()
        {
            _completedForm.Show();
            //_inGameForm.Hide();
        }

        public void OnFailed()
        {
            _failedForm.Show();
            //_inGameForm.Hide();
        }

        public void OnContinue()
        {
            //_startButton.gameObject.SetActive(true);
            //_titleForm.SetActive(true);
            _completedForm.Hide();
            _failedForm.Hide();
            //_inGameForm.Show();
        }
    }
}