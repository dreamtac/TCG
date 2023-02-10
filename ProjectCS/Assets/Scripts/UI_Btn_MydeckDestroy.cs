using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Btn_MydeckDestroy : MonoBehaviour
{
    private UI_MyDeckList myDeckList;

    private void Start()
    {
        myDeckList = GameObject.Find("MyDeckList").GetComponent<UI_MyDeckList>();
    }
    public void DeleteBtn()
    {
        //ī�� ����Ʈ���� Ư�� ī�� ���ý� ������ ����
        string cardName = gameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        for(int i = 0; i < myDeckList.cardList.Count; i++)
        {
            if (myDeckList.cardList[i].name == cardName)
            {
                myDeckList.cardList.RemoveAt(i);
                break;
            }
        }
        myDeckList.TMP_Totalcard.text = myDeckList.cardList.Count.ToString() + " / 30";
        Destroy(gameObject);
    }
}
