using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField, Range(3, 9)] private int _startSize;

        private readonly List<GameObject> _activeTiles = new List<GameObject>();
        private IObjectPool<GameObject> _tilePool;
        private float _tileSize;

        private async void Start()
        {
            _tilePool = new ObjectPool<GameObject>(CreateTile, 
                                                   OnTakeFromPool, 
                                                   OnReturnedToPool,
                                                   OnDestroyPoolObject,
                                                   true,
                                                   50);

            _tileSize = _tilePrefab.transform.localScale.x;

            CreateBlock(_startSize, Direction.FORWARD);
            await Task.Delay(2000);
            CreateBlock(_startSize, Direction.FORWARD);
            await Task.Delay(2000);
            CreateBlock(_startSize, Direction.RIGHT);
            await Task.Delay(2000);
            CreateBlock(2, Direction.RIGHT);
            await Task.Delay(2000);
            CreateBlock(2, Direction.FORWARD);
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