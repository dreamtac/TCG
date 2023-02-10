using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//magicType :::: 1 -> �� �ʵ� ��ü ����, 2 -> �� �ʵ� �� ī�� ���� ����, 3 -> ī�� ��ο�
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