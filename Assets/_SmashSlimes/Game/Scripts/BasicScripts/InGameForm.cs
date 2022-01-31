using DG.Tweening;
using TMPro;
using UnityEngine;

namespace SmashSlimes
{
    public class InGameForm : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentLevel;

        public void Hide()
        {
            _currentLevel.DOFade(0.0f, 0.2f).OnComplete(() => { _currentLevel.gameObject.SetActive(false); })
                .SetLink(gameObject);
        }

        public void Show()
        {
            _currentLevel.gameObject.SetActive(true);
            _currentLevel.DOFade(1.0f, 0.5f).SetLink(gameObject);
            _currentLevel.SetText("Level " + PlayerPrefs.GetInt("CurrentLevel", 1));
        }
    }
}