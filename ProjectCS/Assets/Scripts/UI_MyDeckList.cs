using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MyDeckList : MonoBehaviour
{
    public Button deckListBtn;
    public Button testDeckBtn;
    public List<Cards> cardList;
    public List<Button> cardBtnList;

    private DeckManager deckManager;
    public TMP_Text TMP_Totalcard;
    private GameObject cardSelectPanel;
    private GameObject warningPanel;
    private GameObject mainMenuPanel;

    private void Awake()
    {
        deckManager = GameObject.Find("CardManager").GetComponent<DeckManager>();
        cardSelectPanel = GameObject.Find("CardSelectPanel");
        warningPanel = GameObject.Find("WarningPanel");
        TMP_Totalcard = GameObject.Find("Text_TotalCard").GetComponent<TextMeshProUGUI>();
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        warningPanel.SetActive(false);
    }

    public void AddCardList(Cards card)
    {
        if (cardList.Count != 30)
        {
            // 리스트가 30 이하라면 리스트에 카드 추가
            if (cardList.Count != 0)
            {
                int sameCardCount = 0; // 같은 카드 개수 초기화
                for (int i = 0; i < cardList.Count; i++)
                {
                    if (cardList[i].name.Equals(card.name))
                    {
                        sameCardCount++;
                        Debug.Log("중복 카드 : " + sameCardCount);
                    }
                }

                if (sameCardCount == 2)
                {
                    return;
                }
            }
            cardList.Add(card);
            deckListBtn.GetComponentInChildren<TextMeshProUGUI>().text = card.name;
            //Instantiate(deckListBtn, transform.position, transform.rotation).transform.SetParent(gameObject.transform, false);
            Instantiate(deckListBtn, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0)).transform.SetParent(gameObject.transform, false);
            TMP_Totalcard.text = cardList.Count.ToString() + " / 30";

        }
    }

    public void ConfirmList()
    {
        //컨펌 버튼 누르면 리스트들 내 덱에 들어감
        if(cardList.Count == 30)
        {
            //deckManager.myDeckBuffer = cardList;
            deckManager.mySelectDeck = cardList;
            cardSelectPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            warningPanel.SetActive(true);
        }
    }

    public void TestDeckButton()
    {
        if(cardList.Count != 0)
        {
            for (int i = 0; i < 30; i++)
            {
                if (cardList.Count != 30)
                {
                    cardList.Add(cardList[i]);
                    deckListBtn.GetComponentInChildren<Text>().text = cardList[cardList.Count-1].name;
                    Instantiate(deckListBtn, transform.position, transform.rotation).transform.SetParent(gameObject.transform, false);
                    TMP_Totalcard.text = cardList.Count.ToString() + " / 30";
                }
                else
                {
                    Debug.Log("30장 채움");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("리스트에 최소 1장이 필요");
            return;
        }
    }
}
