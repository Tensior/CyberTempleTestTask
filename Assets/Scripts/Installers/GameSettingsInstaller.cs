using Core;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameSettings _gameSettings;
    
        public override void InstallBindings()
        {
            Container.BindInstances(_gameSettings);
        }
    }
}