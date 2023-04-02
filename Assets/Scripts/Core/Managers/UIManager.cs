using TMPro;
using UnityEngine;

namespace Core.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameEndTextObject;
        
        public void SetGameEndActive(bool isActive)
        {
            _gameEndTextObject.SetActive(isActive);
        }
        
        public void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}