using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Managers
{
    public class CrystalManager : MonoBehaviour
    {
        [SerializeField] private CrystalSpawnRule _crystalSpawnRule;
        [SerializeField, Range(3, 10)] private int _blockSizeForCrystal;
        
        private SignalBus _signalBus;
        private List<Tile> _tilesBuffer;
        private int _orderedSpawnIndex;
        private Crystal.Factory _crystalFactory;

        private void Start()
        {
            _tilesBuffer = new List<Tile>(_blockSizeForCrystal);
            _signalBus.Subscribe<TileCreatedSignal>(OnTileCreated);
            _signalBus.Subscribe<CrystalPickedSignal>(OnCrystalPicked);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<TileCreatedSignal>(OnTileCreated);
            _signalBus.Unsubscribe<CrystalPickedSignal>(OnCrystalPicked);
        }

        [Inject]
        private void Inject(SignalBus signalBus, Crystal.Factory crystalFactory)
        {
            _signalBus = signalBus;
            _crystalFactory = crystalFactory;
        }

        private void OnTileCreated(TileCreatedSignal signal)
        {
            _tilesBuffer.Add(signal.Tile);

            if (_tilesBuffer.Count < _blockSizeForCrystal)
            {
                return;
            }
            
            switch (_crystalSpawnRule)
            {
                case CrystalSpawnRule.RANDOM:
                {
                    var tile = _tilesBuffer[Random.Range(0, _tilesBuffer.Count)];
                    var crystal = _crystalFactory.Create(tile);
                    break;
                }
                case CrystalSpawnRule.ORDERED:
                {
                    break;
                }
            }
            
            _tilesBuffer.Clear();
        }

        private void OnCrystalPicked(CrystalPickedSignal signal)
        {
            Debug.Log("OnCrystalPicked");
            signal.Crystal.Dispose();
        }

        private enum CrystalSpawnRule
        {
            RANDOM,
            ORDERED
        }
    }
}