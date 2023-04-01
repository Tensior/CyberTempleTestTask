using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Random = UnityEngine.Random;

namespace Core
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField, Range(3, 9)] private int _startSize;

        private readonly List<GameObject> _activeTiles = new();
        private IObjectPool<GameObject> _tilePool;
        private Camera _camera;
        private GameSettings _gameSettings;
        private float _tileSize;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _tilePool = new ObjectPool<GameObject>(CreateTile, 
                                                   OnTakeFromPool, 
                                                   OnReturnedToPool,
                                                   OnDestroyPoolObject,
                                                   true,
                                                   50);

            _tileSize = _tilePrefab.transform.localScale.x;

            CreateBlock(_startSize, Direction.FORWARD);

            _cancellationTokenSource = new CancellationTokenSource();
            CreateNewTilesAsync(_cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        [Inject]
        private void Inject(Camera gameCamera, GameSettings gameSettings)
        {
            _camera = gameCamera;
            _gameSettings = gameSettings;
        }

        private async void CreateNewTilesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_camera.WorldToViewportPoint(_activeTiles.Last().transform.position).y < 1f && _activeTiles.Count < 1000)
                {
                    CreateBlock(_gameSettings.PathWidth, (Direction)Random.Range((int)Direction.FORWARD, (int)(Direction.RIGHT + 1)));
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        private void CreateBlock(int nTilesInRow, Direction direction)
        {
            var tilePosition = _tilePrefab.transform.position;
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
                    var tile = _tilePool.Get();
                    _activeTiles.Add(tile);
                    tile.transform.position = tilePosition;
                    tilePosition += Vector3.right * _tileSize;
                }

                tilePosition -= Vector3.right * _tileSize * nTilesInRow;
                tilePosition += Vector3.forward * _tileSize;
            }
        }

        private GameObject CreateTile()
        {
            return Instantiate(_tilePrefab);
        }

        private void OnTakeFromPool(GameObject go)
        {
            go.SetActive(true);
        }

        private void OnReturnedToPool(GameObject go)
        {
            go.SetActive(false);
        }
        
        private void OnDestroyPoolObject(GameObject go)
        {
            Destroy(go);
        }
    }
}