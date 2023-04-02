using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public sealed class Tile : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        public Crystal Crystal { get; set; }

        private IMemoryPool _pool;

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            gameObject.SetActive(true);
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