using GameGlobalJson;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftPack_", menuName = "Game/Shop Gift Pack")]
public class AbilityGiftPackSO : GiftPackSO
{
    /// <summary>
    /// 特权能力枚举(仅用于"永久能力")
    ///
    /// 设计约束：
    /// 1. 本枚举只表示【永久获得】的能力状态，必须写入 GameGlobalJson 进行持久化
    /// 2. 枚举名必须与 Json 字段名严格一致(通过反射写入)
    ///
    /// 注意：
    /// - 新增枚举项时，必须同时在 Json 数据结构中新增对应字段
    /// - 若能力不需要存档或非永久，不加入本枚举
    /// </summary>
    public enum PrivilegeAbility
    {
        None = 0,
        //特权礼包2
        ForeverRemoveAds,
        //特权礼包3
        ForeverDoubleCoinBuff,
        ForeverDailyReward_ByGiftPack3,
        //特权礼包4
        ForeverAddHalfBottle,
        //特权礼包5
        ForeverRemoveHide,
        //特权礼包6
        ForeverDoubleBuff,
    }
    
    public PrivilegeAbility[] Ability => mAbility;

    [SerializeField] private PrivilegeAbility[] mAbility;

    /// <summary>
    /// 获取特权礼包的能力
    /// </summary>
    /// <param name="gameGlobalModel"></param>
    public void GrantPrivilegeAbility(GameGlobalModel gameGlobalModel)
    {
        foreach (var ability in mAbility)
        {
            gameGlobalModel.SetFieldAndSave(JsonType.GameGlobalJson, gameGlobalModel.GameGlobalJsonData,
                ability.ToString(), true);
        }
    }


    //在这提供一个方法，通过能力获取到对应的精灵，

}
