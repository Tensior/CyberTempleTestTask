using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _deathZoneY = -1;
        
        private Player _player;
        private bool _isGameEnd;

        private void Start()
        {
            Time.timeScale = 1f;
        }

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
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }

            if (_player.transform.position.y < _deathZoneY)
            {
                Time.timeScale = 0f;
                _isGameEnd = true;
            }
        }

        [Inject]
        private void Inject(Player player)
        {
            _player = player;
        }
    }
}