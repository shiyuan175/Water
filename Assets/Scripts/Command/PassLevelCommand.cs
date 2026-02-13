using Game.Water;
using QFramework;

namespace Game.Water
{
    public class PassLevelCommand : AbstractCommand ,ICanGetModel
    {
        private GameGlobalModel mGameGlobalModel;

        protected override void OnExecute()
        {
            int currentLevel = this.GetUtility<SaveDataUtility>().GetCurrentLevel();
            mGameGlobalModel ??= this.GetModel<GameGlobalModel>();
            mGameGlobalModel?.PassLevel();
        }
    }
}
