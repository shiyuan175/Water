using Game.Water;
using QFramework;

namespace Game.Water
{
    public class GameMainArc : Architecture<GameMainArc>
    {
        protected override void Init()
        {
            ResKit.Init();
            RegisterUtilitys();
            RegisterModels();
            CreateInstance();
        }

        private void RegisterModels()
        {
            RegisterModel(new GameGlobalModel());
        }

        private void RegisterUtilitys()
        {
            RegisterUtility(new SaveDataUtility());
            RegisterUtility(new LevelManagerUtility());
        }

        private void CreateInstance()
        {
            var tenjinManager = TenjinManager.Instance;
            var firebaseManager = FirebaseManager.Instance;
            var topOnADManager = TopOnADManager.Instance;
            GameUtilityManager gameUtilityManager = GameUtilityManager.Instance;
        }
    }
}
