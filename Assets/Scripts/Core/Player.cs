using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Player : MonoBehaviour
    {
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
        }

        private void FixedUpdate()
        {
            var velocityY = _rigidBody.velocity.y;
            var horizontalVelocity = _velocity * _velocityNormalized;
            _rigidBody.velocity = new Vector3(horizontalVelocity.x, velocityY, horizontalVelocity.z);

        }

        [Inject]
        private void Inject(Settings settings)
        {
            _velocity = settings.BallVelocity;
        }
    }
}