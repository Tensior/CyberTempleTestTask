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
            }
        }

        [Inject]
        private void Inject(Player player, TileManager tileManager, CrystalManager crystalManager)
        {
            _player = player;
            _tileManager = tileManager;
            _crystalManager = crystalManager;
        }

        private void ResetGame()
        {
            _isGameEnd = false;
            Time.timeScale = 1f;
            _player.Reset();
            _crystalManager.Reset();
            _tileManager.Reset();
        }
    }
}