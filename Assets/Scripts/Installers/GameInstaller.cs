using Core;
using Core.Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Crystal _crystalPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Player>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<TileManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<CrystalManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle().NonLazy();
            Container.Bind<Camera>().FromComponentInHierarchy().AsSingle().NonLazy();
            
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TileCreatedSignal>();
            Container.DeclareSignal<CrystalPickedSignal>();

            Container.BindFactory<Tile, Tile.Factory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(50)
                                                .FromComponentInNewPrefab(_tilePrefab)
                                                .UnderTransformGroup("Tiles"));
            
            Container.BindFactory<Tile, Crystal, Crystal.Factory>()
                .FromMonoPoolableMemoryPool(x => x.WithInitialSize(10)
                                                .FromComponentInNewPrefab(_crystalPrefab)
                                                .UnderTransformGroup("Crystals"));
        }
    }
}