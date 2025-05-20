using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftPack", menuName = "Game/Gift Pack")]
public class GiftPackSO : ScriptableObject
{
    [Header("����ID")]
    [SerializeField] private string PackID;
    [Header("�������")]
    [SerializeField] private int coins;
    [SerializeField] private List<ItemReward> items;
    [Header("��������(��λ������)")]
    [SerializeField] private int unlimitedHp;
    [Header("�޹��")]
    [SerializeField] private bool removeAds;

    public int Coins => coins;
    public IReadOnlyList<ItemReward> Items => items;
    public int UnlimitedHp => unlimitedHp;
    public bool RemoveAds => removeAds;
    public string ID => PackID;
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