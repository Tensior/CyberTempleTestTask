using Core;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Player>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}