using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class Tile : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        public Crystal Crystal { get; set; }

        private readonly Vector3 _defaultPosition = new (0, -1.5f, 0);
        private IMemoryPool _pool;

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

        public class Factory : PlaceholderFactory<Tile> { }
    }
}