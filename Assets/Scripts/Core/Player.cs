using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Player : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Direction _direction;
        private Rigidbody _rigidBody;
        private Vector3 _velocityNormalized;
        private int _velocity;

        public Direction Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _velocityNormalized = Vector3.zero;
                switch (_direction)
                {
                    case Direction.NONE:
                        _velocityNormalized = Vector3.zero;
                        break;
                    case Direction.FORWARD:
                        _velocityNormalized = Vector3.forward;
                        break;
                    case Direction.RIGHT:
                        _velocityNormalized = Vector3.right;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _startPosition = transform.position;
        }

        private void FixedUpdate()
        {
            var velocityY = _rigidBody.velocity.y;
            var horizontalVelocity = _velocity * _velocityNormalized;
            _rigidBody.velocity = new Vector3(horizontalVelocity.x, velocityY, horizontalVelocity.z);

        }

        public void Reset()
        {
            transform.position = _startPosition;
            Direction = Direction.NONE;
        }

        [Inject]
        private void Inject(GameSettings gameSettings)
        {
            _velocity = gameSettings.BallVelocity;
        }
    }
}