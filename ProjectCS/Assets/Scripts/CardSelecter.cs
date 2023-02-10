using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelecter : MonoBehaviour
{
    //�� �� ȭ��
    //cardBuffer ����Ʈ�� ��� ī���(��ũ���ͺ� ������Ʈ)�� �߰��� �� �� �������� 8���� cardBuffer���� ������ �����ϴ� ���
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
    private int page;                   //���� ������
    private int slotCount = 0;          //���� (1~8)

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
        if(page > 1)        //������ 0���� ���°� ����
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
        //���� �ڽ� ��ü ����
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
