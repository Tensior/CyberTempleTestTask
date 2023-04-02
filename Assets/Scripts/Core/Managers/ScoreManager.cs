using System;
using Zenject;

namespace Core.Managers
{
    public class ScoreManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly UIManager _uiManager;
        private int _score;

        private int Score
        {
            get => _score;
            set
            {
                _score = value;
                _uiManager.UpdateScore(_score);
            }
        }

        public ScoreManager(SignalBus signalBus, UIManager uiManager)
        {
            _signalBus = signalBus;
            _uiManager = uiManager;
        }

        public void Reset()
        {
            Score = 0;
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<CrystalPickedSignal>(OnCrystalPicked);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void OnCrystalPicked()
        {
            Score++;
        }
    }
}