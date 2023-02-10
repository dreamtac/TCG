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
        if(cardList.Count != 30)
        {
            //����Ʈ�� 30 ���϶�� ����Ʈ�� ī�� �߰�
            cardList.Add(card);
            deckListBtn.GetComponentInChildren<TextMeshProUGUI>().text = card.name;
            Instantiate(deckListBtn, transform.position, transform.rotation).transform.SetParent(gameObject.transform, false);
            TMP_Totalcard.text = cardList.Count.ToString() + " / 30";
            //AddBtn();
        }
        else
        {
            Debug.Log("���� ���� �� �ִ� �ִ� ī��� �ʰ�!!!");
        }
    }

    public void ConfirmList()
    {
        //���� ��ư ������ ����Ʈ�� �� ���� ��
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
                    Debug.Log("30�� ä��");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("����Ʈ�� �ּ� 1���� �ʿ�");
            return;
        }
    }
}
