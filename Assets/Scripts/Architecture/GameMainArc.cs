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
    }
}
