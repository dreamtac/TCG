using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//magicType :::: 1 -> 적 필드 전체 공격, 2 -> 적 필드 위 카드 한장 공격, 3 -> 카드 드로우
public class Cards
{
    public string name;
    public string type;
    public int magicType;
    public int magicValue;
    public int cost;
    public int attack;
    public int health;
    public Sprite characterImage;
}

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Cards[] cards;
}