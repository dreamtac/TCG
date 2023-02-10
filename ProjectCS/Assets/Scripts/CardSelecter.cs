using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelecter : MonoBehaviour
{
    //덱 편성 화면
    //cardBuffer 리스트에 모든 카드들(스크립터블 오브젝트)을 추가한 후 한 페이지에 8개씩 cardBuffer에서 꺼내서 나열하는 방식
    [SerializeField] ItemSO allCards;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text costText;

    public List<Cards> cardBuffer;
    public List<GameObject> slot;
    public int count = 0;               
    private int page;                   //현재 페이지
    private int slotCount = 0;          //슬롯 (1~8)

    // Start is called before the first frame update
    void Start()
    {
        characterImage = cardPrefab.transform.Find("CharacterImage").gameObject.GetComponent<Image>();
        nameText = cardPrefab.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        attackText = cardPrefab.transform.Find("Attack").gameObject.GetComponent<TextMeshProUGUI>();
        healthText = cardPrefab.transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
        costText = cardPrefab.transform.Find("Cost").gameObject.GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < allCards.cards.Length; i++)
        {
            cardBuffer.Add(allCards.cards[i]);
        }

        NextPage();       
    }

    public void NextPage()
    {         
        if((page * 8) < cardBuffer.Count)       
        {
            slotCount = 0;
            page++;
            count = page * 8;

            for (int i = 0; i < 8; i++)
            {
                RemoveChild(slot[i]);
            }

            for (int i = count - 8; i < count; i++)
            {
                if (i < cardBuffer.Count)
                {
                    characterImage.sprite = cardBuffer[i].characterImage;
                    nameText.text = cardBuffer[i].name;
                    attackText.text = cardBuffer[i].attack.ToString();
                    healthText.text = cardBuffer[i].health.ToString();
                    costText.text = cardBuffer[i].cost.ToString();
                    cardPrefab.GetComponent<UI_CardBtn>().cardInfo = cardBuffer[i];
                    Instantiate(cardPrefab, slot[slotCount].transform);
                    slotCount++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void PrevPage()
    {
        if(page > 1)        //페이지 0으로 가는거 막기
        {
            slotCount = 0;
            page--;
            count = page * 8;

            for (int i = 0; i < 8; i++)
            {
                RemoveChild(slot[i]);
            }

            for (int i = count - 8; i < count; i++)
            {
                if (i < cardBuffer.Count)
                {
                    characterImage.sprite = cardBuffer[i].characterImage;
                    nameText.text = cardBuffer[i].name;
                    attackText.text = cardBuffer[i].attack.ToString();
                    healthText.text = cardBuffer[i].health.ToString();
                    costText.text = cardBuffer[i].cost.ToString();
                    cardPrefab.GetComponent<UI_CardBtn>().cardInfo = cardBuffer[i];
                    Instantiate(cardPrefab, slot[slotCount].transform);
                    slotCount++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void RemoveChild(GameObject parent)
    {
        //슬롯 자식 객체 삭제
        Transform[] child = parent.GetComponentsInChildren<Transform>();

        if (child != null)
        {
            for(int i = 1; i < child.Length; i++)
            {
                if (child[i] != transform)
                {
                    Destroy(child[i].gameObject);
                }
            }
        }
    }
}
