using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemEnum { Weapon, Coin, Heart };
    public ItemEnum ItemType;
    public int ItemValue;
}