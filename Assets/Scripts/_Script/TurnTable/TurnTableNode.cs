using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTableNode : MonoBehaviour
{
    [SerializeField] private GiftPackSO packSo;
    [SerializeField] private GameDefine.AwardBaseProbability awardLevel;


    public GiftPackSO PackSo => packSo;
    public GameDefine.AwardBaseProbability AwardLevel =>awardLevel;
}
