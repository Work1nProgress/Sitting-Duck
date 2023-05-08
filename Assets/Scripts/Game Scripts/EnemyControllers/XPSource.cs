using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPSource : Pickup
{
    private int _experience;

    public void SetExp(int _exp)
    {
        _experience = _exp;
    }

    public int GetExp => _experience;
   
}
