using UnityEngine;
using Zenject;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private Player _player;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
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
        }

        [Inject]
        private void Inject(Player player)
        {
            _player = player;
        }
    }
}