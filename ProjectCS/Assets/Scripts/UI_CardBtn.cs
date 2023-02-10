using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CardBtn : MonoBehaviour
{
    public string name, attack, health, type;
    public Sprite characterImage;

    private UI_MyDeckList myDeckList;

    public Cards cardInfo;

    private void Start()
    {
        myDeckList = GameObject.Find("MyDeckList").GetComponent<UI_MyDeckList>();
    }

    public void SelectCard()
    {   //UI 카드 이미지 프리팹의 버튼 이벤트
        name = this.cardInfo.name;
        attack = gameObject.transform.Find("Attack")
            .GetComponent<TextMeshProUGUI>().text;
        health = gameObject.transform.Find("Health")
            .GetComponent<TextMeshProUGUI>().text;
        characterImage = gameObject.transform.Find("CharacterImage")
            .GetComponent<Image>().sprite;

        Debug.Log("선택한 카드는 :: " + name + ", 공격력 :: " + attack + ", 체력 :: " + health);

        myDeckList.AddCardList(cardInfo);
    }

    public void PrintName()
    {
        Debug.Log(gameObject.transform.Find("Name")
            .GetComponent<TextMeshProUGUI>().text);
    }
}
