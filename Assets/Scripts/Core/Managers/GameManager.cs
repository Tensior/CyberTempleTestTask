using UnityEngine;
using Zenject;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _deathZoneY = -1;
        
        private Player _player;
        private bool _isGameEnd;
        private TileManager _tileManager;
        private CrystalManager _crystalManager;
        private ScoreManager _scoreManager;
        private UIManager _uiManager;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(!_isGameEnd)
                {
                    if (_player.Direction is Direction.NONE or Direction.RIGHT)
                    {
                        _player.Direction = Direction.FORWARD;
                    }
                    else
                    {
                        _player.Direction = Direction.RIGHT;
                    }
                }
                else
                {
                    ResetGame();
                }
            }

            if (_player.transform.position.y < _deathZoneY)
            {
                Time.timeScale = 0f;
                _isGameEnd = true;
                _uiManager.SetGameEndActive(true);
            }
        }

        [Inject]
        private void Inject(
            Player player,
            TileManager tileManager,
            CrystalManager crystalManager,
            ScoreManager scoreManager,
            UIManager uiManager)
        {
            _player = player;
            _tileManager = tileManager;
            _crystalManager = crystalManager;
            _scoreManager = scoreManager;
            _uiManager =  uiManager;
        }

        private void ResetGame()
        {
            Time.timeScale = 1f;
            _isGameEnd = false;
            _uiManager.SetGameEndActive(false);
            _scoreManager.Reset();
            _player.Reset();
            _crystalManager.Reset();
            _tileManager.Reset();
        }
    }
}