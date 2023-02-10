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
    public bool isFront;                    //나의 카드라면 True
    public bool isField;                    //필드 위에 소환되어 있다면 True
    public bool attackable;                 //공격 가능?
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
    {   //덱에서 카드 드로우, 초기화
        this.cards = card;
        this.cost = card.cost;
        this.cardName = card.name;
        this.isFront = isFront;
        this.attack = card.attack;
        this.health = card.health;

        if (this.isFront) 
        {   //내 카드 일 때
            type = cards.type;
            character.sprite = this.cards.characterImage;
            nameTMP.text = this.cards.name;
            costTMP.text = cost.ToString();
            if(type == "마법")
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
        {   //상대 카드 일 때
            type = cards.type;
            character.sprite = this.cards.characterImage;
            nameTMP.text = this.cards.name;
            costTMP.text = this.cards.cost.ToString();
            if (type == "마법")
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
    {   //카드 움직임 (DoTween)
        if (useDotween)
        {   //DoTween 사용 (부드럽게 움직임)
            transform.DOMove(prs.pos, dotweenTime);
            //transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
            turnController.Loading(dotweenTime);
        }
        else
        {   //DoTween 미사용 (바로 움직임)
            transform.position = prs.pos;
            //transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }

    private void CardAlignment(bool isMine)
    {       //내 손 or 상대 손에 카드를 가져올 위치
        if (isMine)
        {   //내 카드
            int cardPos = deckManager.myHandBuffer.Count - 1;
            Vector3 pos = deckManager.myHandPosition[cardPos].position;
            originPRS = new PRS(pos, Utils.QI, transform.localScale);
        }
        else
        {   //상대 카드
            int cardPos = deckManager.enemyHandBuffer.Count - 1;
            Vector3 pos = deckManager.enemyHandPosition[cardPos].position;
            originPRS = new PRS(pos, Utils.QI, transform.localScale);
        }
        MoveTransform(originPRS, true, 0.7f);
    }

    private void OnMouseOver()
    {
        //마우스가 카드 위에 위치할 때 *OnMouseOver 말고 OnMouseEnter를 해도 되지 않나?
        if (isFront && !isField)
        {
            deckManager.CardMouseOver(this);
        }
    }

    private void OnMouseExit()
    {
        //마우스가 카드 에서 벗어날 때
        if (isFront && !isField)
        {
            deckManager.CardMouseExit(this);
        }
    }

    private void OnMouseDown()
    {
        if (isFront && !isField && !TurnController.isLoading)
        {
            //카드를 마우스로 클릭 할 때
            deckManager.CardMouseDown(this);
        }
        else if(isFront && isField && !TurnController.isLoading)
        {
            //필드에 소환되어 있는 카드륵 클릭 할 때
            fieldManager.FieldMouseDown(this);
        }
    }

    private void OnMouseUp()
    {
        if (isFront && !isField && !TurnController.isLoading)
        {
            //카드를 클릭한 마우스를 땔 때
            deckManager.CardMouseUp(this);
        }
        else if (isFront && isField && !TurnController.isLoading)
        {
            //필드에 소환되어 있는 카드에 마우스를 땔 때
            fieldManager.FieldMouseUp(this);
        }
    }

    private void OnMouseDrag()
    {
        if(isFront && isField && attackable && !isDead)
        {   //필드에 소환되어 있는 카드를 드래그 할 때
            if (TurnController.isLoading)
                return;
            
            fieldManager.FieldMouseDrag(this);
        }
    }

    public void InitStatus()
    {//공격 후 서로의 카드 생명력 초기화
        attackTMP.text = attack.ToString();
        healthTMP.text = health.ToString();
    }
}
