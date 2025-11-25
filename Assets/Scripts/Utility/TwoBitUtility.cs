using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class TwoBitUtility : IUtility
{
    public int Get2BitValue(int bitNumber)
    {
        return 1 << bitNumber;
    }
    public bool HasBitValue(int count,int bitNumber)
    {
        return (count & Get2BitValue(bitNumber)) != 0;
    }

}
