using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.Managers
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField, Range(3, 9)] private int _startSize;
        [SerializeField] private int _createTilesRefreshMS = 1000;
        [SerializeField] private int _removeTilesRefreshMS = 100;

        private const int NTilesMax = 1000;
        private readonly LinkedList<Tile> _activeTiles = new();
        private Tile.Factory _tileFactory;
        private Camera _camera;
        private GameSettings _gameSettings;
        private SignalBus _signalBus;
        private float _tileSize;
        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _defaultCameraPosition;

        private void Start()
        {
            Reset();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Reset()
        {
            foreach (var tile in _activeTiles)
            {
                tile.Dispose();
            }
            _activeTiles.Clear();

            _camera.transform.position = _defaultCameraPosition;
            CreateBlock(_startSize, Direction.FORWARD);
            while (_camera.WorldToViewportPoint(_activeTiles.Last().transform.position).y < 1f && _activeTiles.Count < NTilesMax)
            {
                CreateBlock(_gameSettings.PathWidth, (Direction)Random.Range((int)Direction.FORWARD, (int)(Direction.RIGHT + 1)));
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CreateNewTilesAsync(_cancellationTokenSource.Token);
            RemoveOldTilesAsync(_cancellationTokenSource.Token);
        }

        [Inject]
        private void Inject(Camera gameCamera, GameSettings gameSettings, SignalBus signalBus, Tile.Factory tileFactory)
        {
            _camera = gameCamera;
            _gameSettings = gameSettings;
            _signalBus = signalBus;
            _tileFactory = tileFactory;

            _defaultCameraPosition = _camera.transform.position;
        }

        private async void CreateNewTilesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_camera.WorldToViewportPoint(_activeTiles.Last().transform.position).y < 1f && _activeTiles.Count < NTilesMax)
                {
                    CreateBlock(_gameSettings.PathWidth, (Direction)Random.Range((int)Direction.FORWARD, (int)(Direction.RIGHT + 1)));
                }

                try
                {
                    await Task.Delay(_createTilesRefreshMS, cancellationToken);
                }
                catch (Exception _)
                {
                    // ignored
                }
            }
        }
        
        private async void RemoveOldTilesAsync(CancellationToken cancellationToken)
        {
            var nTilesToRemove = _startSize * _startSize;
            
            while (!cancellationToken.IsCancellationRequested && _activeTiles.Count >= nTilesToRemove + 1)
            {
                var tile = _activeTiles.ElementAt(nTilesToRemove);
                if (_camera.WorldToViewportPoint(tile.transform.position).y < 0.2f)
                {
                    for (int i = 0; i < nTilesToRemove; i++)
                    {
                        tile = _activeTiles.First();
                        _activeTiles.RemoveFirst();
                        tile.DisposeAfterFalling();
                    }

                    nTilesToRemove = _gameSettings.PathWidth * _gameSettings.PathWidth;
                }

                try
                {
                    await Task.Delay(_removeTilesRefreshMS, cancellationToken);
                }
                catch (Exception _)
                {
                    // ignored
                }
            }
        }

        private void CreateBlock(int nTilesInRow, Direction direction)
        {
            var tilePosition = Vector3.zero;
            if (_activeTiles.Count != 0)
            {
                Vector3 tileShift = Vector3.zero;
                switch (direction)
                {
                    case Direction.RIGHT:
                    {
                        tileShift = Vector3.right * _tileSize + Vector3.back * _tileSize * (nTilesInRow - 1);
                        break;
                    }
                    case Direction.FORWARD:
                    {
                        tileShift = Vector3.forward * _tileSize + Vector3.left * _tileSize * (nTilesInRow - 1);
                        break;
                    }
                }
                tilePosition = _activeTiles.Last().transform.position + tileShift;
            }

            for (var i = 0; i < nTilesInRow; ++i)
            {
                for (var j = 0; j < nTilesInRow; ++j)
                {
                    var tile = _tileFactory.Create();
                    _activeTiles.AddLast(tile);
                    if (tilePosition != Vector3.zero)
                    {
                        tile.transform.position = tilePosition;
                    }
                    else
                    {
                        tilePosition = tile.transform.position;
                        _tileSize = tile.transform.localScale.x;
                    }
                    _signalBus.Fire(new TileCreatedSignal{Tile = tile}); 
                    tilePosition += Vector3.right * _tileSize;
                }

                tilePosition -= Vector3.right * _tileSize * nTilesInRow;
                tilePosition += Vector3.forward * _tileSize;
            }
        }
    }
}