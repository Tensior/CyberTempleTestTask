using Core;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Transform _tilePrefab;

        public override void InstallBindings()
        {
            Container.Bind<Player>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<Camera>().FromComponentInHierarchy().AsSingle().NonLazy();
            
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TileCreatedSignal>();
            
            //Container.BindMemoryPool<GameObject, TilePool>().AsSingle().NonLazy()/*.WithInitialSize(50)*/;
            Container.BindMemoryPool<Transform, TilePool>()
                .WithInitialSize(50)
                .FromComponentInNewPrefab(_tilePrefab)
                .UnderTransformGroup("Tiles");
        }
    }
}