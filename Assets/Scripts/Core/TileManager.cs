using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core
{
    public class TileManager : MonoBehaviour
    {
        //[SerializeField] private GameObject _tilePrefab;
        [SerializeField, Range(3, 9)] private int _startSize;
        [SerializeField] private int _createTilesRefreshMS = 1000;
        [SerializeField] private int _removeTilesRefreshMS = 100;

        private const int NTilesMax = 1000;
        private readonly LinkedList<Transform> _activeTiles = new();
        //private IObjectPool<GameObject> _tilePool;
        private TilePool _tilePool;
        private Camera _camera;
        private GameSettings _gameSettings;
        private SignalBus _signalBus;
        private float _tileSize;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            /*_tilePool = new ObjectPool<GameObject>(CreateTile, 
                                                   OnTakeFromPool, 
                                                   OnReturnedToPool,
                                                   OnDestroyPoolObject,
                                                   true,
                                                   50);*/

            CreateBlock(_startSize, Direction.FORWARD);
            while (_camera.WorldToViewportPoint(_activeTiles.Last().transform.position).y < 1f && _activeTiles.Count < NTilesMax)
            {
                CreateBlock(_gameSettings.PathWidth, (Direction)Random.Range((int)Direction.FORWARD, (int)(Direction.RIGHT + 1)));
            }

            _cancellationTokenSource = new CancellationTokenSource();
            CreateNewTilesAsync(_cancellationTokenSource.Token);
            RemoveOldTilesAsync(_cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        [Inject]
        private void Inject(Camera gameCamera, GameSettings gameSettings, SignalBus signalBus, TilePool tilePool)
        {
            _camera = gameCamera;
            _gameSettings = gameSettings;
            _signalBus = signalBus;
            _tilePool = tilePool;
        }

        private async void CreateNewTilesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_camera.WorldToViewportPoint(_activeTiles.Last().transform.position).y < 1f && _activeTiles.Count < NTilesMax)
                {
                    CreateBlock(_gameSettings.PathWidth, (Direction)Random.Range((int)Direction.FORWARD, (int)(Direction.RIGHT + 1)));
                }

                await Task.Delay(_createTilesRefreshMS);
            }
        }
        
        private async void RemoveOldTilesAsync(CancellationToken cancellationToken)
        {
            var nTilesToRemove = _startSize * _startSize;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_activeTiles.Count < nTilesToRemove + 1)
                {
                    continue;
                }
                
                var tile = _activeTiles.ElementAt(nTilesToRemove);
                if (_camera.WorldToViewportPoint(tile.transform.position).y < 0.2f)
                {
                    for (int i = 0; i < nTilesToRemove; i++)
                    {
                        tile = _activeTiles.First();
                        _activeTiles.RemoveFirst();
                        _tilePool.Despawn(tile);
                    }

                    nTilesToRemove = _gameSettings.PathWidth * _gameSettings.PathWidth;
                }

                await Task.Delay(_removeTilesRefreshMS);
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
                    var tile = _tilePool.Spawn();
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