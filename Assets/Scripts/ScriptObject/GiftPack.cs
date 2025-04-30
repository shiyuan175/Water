using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftPack", menuName = "Game/Gift Pack")]
public class GiftPack : ScriptableObject,ICanGetUtility
{
    [Header("�������")]
    [SerializeField] private int coins;
    [SerializeField] private List<ItemReward> items;
    [Header("��������(��λ������)")]
    [SerializeField] private int unlimitedHp; 
    [SerializeField] private bool removeAds;

    public int Coins => coins;
    public IReadOnlyList<ItemReward> Items => items;
    public int UnlimitedHp => unlimitedHp;
    public bool RemoveAds => removeAds;


    public void BuyGiftPack()
    {
        CoinManager.Instance.AddCoin(coins);
        HealthManager.Instance.SetUnLimitHp(unlimitedHp);
    }

    public IArchitecture GetArchitecture()
    {
        return GameMainArc.Interface;
    }
}

[System.Serializable]
public class ItemReward
{
    [Tooltip("����������1~8")]
    [Range(1, 8)]
    [SerializeField] private int itemIndex;

    [Tooltip("��������")]
    [SerializeField] private int quantity;

    // ֻ������
    public int ItemIndex => itemIndex;
    public int Quantity => quantity;
}