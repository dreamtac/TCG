using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Card : MonoBehaviour
{
    private DeckManager deckManager;
    private Cards cards;
    private FieldManager fieldManager;
    private TurnController turnController;
    [SerializeField] GameObject card;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;
    [SerializeField] TMP_Text costTMP;
    public string type;
    public PRS originPRS;
    public GameObject damageImage;

    public string cardName;
    public int cost;
    public int attack;
    public int health;
    public bool isFront;                    //���� ī���� True
    public bool isField;                    //�ʵ� ���� ��ȯ�Ǿ� �ִٸ� True
    public bool attackable;                 //���� ����?
    public bool isDead;
    public int magicType;
    public int magicValue;

    private void Awake()
    {
        deckManager = GameObject.Find("CardManager").GetComponent<DeckManager>();
        fieldManager = GameObject.Find("CardManager").GetComponent<FieldManager>();
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
    }
    private void Update()
    {
        if (!attackable)
        {
            character.color = Color.red;
        }
        else
        {
            character.color = Color.white;
        }
    }

    public void Setup(Cards card, bool isFront)
    {   //������ ī�� ��ο�, �ʱ�ȭ
        this.cards = card;
        this.cost = card.cost;
        this.cardName = card.name;
        this.isFront = isFront;
        this.attack = card.attack;
        this.health = card.health;

        if (this.isFront) 
        {   //�� ī�� �� ��
            type = cards.type;
            character.sprite = this.cards.characterImage;
            nameTMP.text = this.cards.name;
            costTMP.text = cost.ToString();
            if(type == "����")
            {
                attackTMP.text = "";
                healthTMP.text = "";
                this.magicType = card.magicType;
                this.magicValue = card.magicValue;
            }
            else
            {
                attackTMP.text = attack.ToString();
                healthTMP.text = health.ToString();
            }
            gameObject.transform.rotation = Quaternion.Euler(0,-180, 0);
            attackable = true;
            CardAlignment(true);
        }
        else
        {   //��� ī�� �� ��
            type = cards.type;
            character.sprite = this.cards.characterImage;
            nameTMP.text = this.cards.name;
            costTMP.text = this.cards.cost.ToString();
            if (type == "����")
            {
                attackTMP.text = "";
                healthTMP.text = "";
                this.magicType = card.magicType;
                this.magicValue = card.magicValue;
            }
            else
            {
                attackTMP.text = attack.ToString();
                healthTMP.text = health.ToString();
            }
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            attackable = true;
            CardAlignment(false);
        }
    }

    public void Damaged(int damage)
    {
        GameObject image;
        image = Instantiate(damageImage);
        image.GetComponentInChildren<TextMeshPro>().text = "-" + damage.ToString();
        image.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.1f, gameObject.transform.position.z);
        image.transform.SetParent(gameObject.transform);
        //gameObject.GetComponent<BGMController>().Card_Attack();
        Destroy(image.gameObject, 1f);
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {   //ī�� ������ (DoTween)
        if (useDotween)
        {   //DoTween ��� (�ε巴�� ������)
            transform.DOMove(prs.pos, dotweenTime);
            //transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
            turnController.Loading(dotweenTime);
        }
        else
        {   //DoTween �̻�� (�ٷ� ������)
            transform.position = prs.pos;
            //transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }

    private void CardAlignment(bool isMine)
    {       //�� �� or ��� �տ� ī�带 ������ ��ġ
        if (isMine)
        {   //�� ī��
            int cardPos = deckManager.myHandBuffer.Count - 1;
            Vector3 pos = deckManager.myHandPosition[cardPos].position;
            originPRS = new PRS(pos, Utils.QI, transform.localScale);
        }
        else
        {   //��� ī��
            int cardPos = deckManager.enemyHandBuffer.Count - 1;
            Vector3 pos = deckManager.enemyHandPosition[cardPos].position;
            originPRS = new PRS(pos, Utils.QI, transform.localScale);
        }
        MoveTransform(originPRS, true, 0.7f);
    }

    private void OnMouseOver()
    {
        //���콺�� ī�� ���� ��ġ�� �� *OnMouseOver ���� OnMouseEnter�� �ص� ���� �ʳ�?
        if (isFront && !isField)
        {
            deckManager.CardMouseOver(this);
        }
    }

    private void OnMouseExit()
    {
        //���콺�� ī�� ���� ��� ��
        if (isFront && !isField)
        {
            deckManager.CardMouseExit(this);
        }
    }

    private void OnMouseDown()
    {
        if (isFront && !isField && !TurnController.isLoading)
        {
            //ī�带 ���콺�� Ŭ�� �� ��
            deckManager.CardMouseDown(this);
        }
        else if(isFront && isField && !TurnController.isLoading)
        {
            //�ʵ忡 ��ȯ�Ǿ� �ִ� ī�帤 Ŭ�� �� ��
            fieldManager.FieldMouseDown(this);
        }
    }

    private void OnMouseUp()
    {
        if (isFront && !isField && !TurnController.isLoading)
        {
            //ī�带 Ŭ���� ���콺�� �� ��
            deckManager.CardMouseUp(this);
        }
        else if (isFront && isField && !TurnController.isLoading)
        {
            //�ʵ忡 ��ȯ�Ǿ� �ִ� ī�忡 ���콺�� �� ��
            fieldManager.FieldMouseUp(this);
        }
    }

    private void OnMouseDrag()
    {
        if(isFront && isField && attackable && !isDead)
        {   //�ʵ忡 ��ȯ�Ǿ� �ִ� ī�带 �巡�� �� ��
            if (TurnController.isLoading)
                return;
            
            fieldManager.FieldMouseDrag(this);
        }
    }

    public void InitStatus()
    {//���� �� ������ ī�� ����� �ʱ�ȭ
        attackTMP.text = attack.ToString();
        healthTMP.text = health.ToString();
    }
}
