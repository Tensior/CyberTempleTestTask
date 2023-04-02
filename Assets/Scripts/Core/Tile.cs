using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class Tile : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [SerializeField] private float _fallSpeed = 3;
        [SerializeField] private float _fallTimeSeconds = 3;
        [SerializeField] private Vector3 _defaultPosition = new (0, -1.5f, 0);
        
        private IMemoryPool _pool;
        private Coroutine _fallingCoroutine;
        
        public Crystal Crystal { get; set; }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            gameObject.SetActive(true);
            transform.position = _defaultPosition;
        }

        public void OnDespawned()
        {
            _pool = null;
            gameObject.SetActive(false);
            if (Crystal != null)
            {
                Crystal.Dispose();
                Crystal = null;
            }
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public void DisposeAfterFalling()
        {
            if (_fallingCoroutine == null)
            {
                _fallingCoroutine = StartCoroutine(FallingCoroutine());
            }
        }

        private IEnumerator FallingCoroutine()
        {
            var secondsPassed = 0f;
            
            while (secondsPassed < _fallTimeSeconds && Time.timeScale > 0)
            {
                var shift = Vector3.down * _fallSpeed * Time.deltaTime;
                transform.position += shift;
                if (Crystal != null)
                {
                    Crystal.transform.position += shift;
                }
                secondsPassed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitUntil(() => Time.timeScale > 0);

            _fallingCoroutine = null;
            Dispose();
        }

        public class Factory : PlaceholderFactory<Tile> { }
    }
}