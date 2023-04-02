using System;
using UnityEngine;
using Zenject;

namespace Core
{
    public class Crystal : MonoBehaviour, IPoolable<Tile, IMemoryPool>, IDisposable
    {
        public Tile Tile { get; set; }
        
        private SignalBus _signalBus;
        private IMemoryPool _pool;

        public void OnSpawned(Tile tile, IMemoryPool pool)
        {
            _pool = pool;
            gameObject.SetActive(true);
            transform.position = tile.transform.position;
            Tile = tile;
            Tile.Crystal = this;
        }

        public void OnDespawned()
        {
            _pool = null;
            gameObject.SetActive(false);
            Tile.Crystal = null;
            Tile = null;
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>() != null)
            {
                _signalBus.Fire(new CrystalPickedSignal { Crystal = this });
            }
        }

        [Inject]
        private void Inject(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public class Factory : PlaceholderFactory<Tile, Crystal> { }
    }
}