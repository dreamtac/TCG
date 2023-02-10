using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    private GameObject cardSelectPanel;
    private TurnController turnController;
    private FieldManager fieldManager;
    private UIManager UiManager;

    public List<Cards> mySelectDeck;
    public List<Cards> myDeckBuffer;
    public List<Cards> enemyDeckBuffer;
    public List<Cards> myHandBuffer;
    public List<Cards> enemyHandBuffer;
    public List<GameObject> playerCardGrave;
    public List<GameObject> enemyCardGrave;
    public List<GameObject> myHandCard;
    public List<GameObject> enemyHandCard;
    public List<Transform> myHandPosition;
    public List<Transform> enemyHandPosition;
    public List<Transform> cardGravePosition;

    private MeshCollider boardCollider;
    private GameObject gameBoard;
    public static Vector3 rayPos;
    [SerializeField] ECardState eCardState;
    [SerializeField] Transform cardSpawnPoint;
    private bool onMyCardArea;
    private bool isMyCardDrag;
   // private bool enemyMoreSummon = false;
    private RaycastHit[] mousePosHits;
    Card selectCard;
    int detectCard = 0;
    enum ECardState
    {   //����
        Nothing = 0,    //���콺 ����, �巡�� �Ұ���
        CanMouseOver,   //���콺 ���� ����, �巡�� �Ұ���
        CanMouseDrag    //���콺 ����, �巡�� ��� ����
    }

    private void Start()
    {
        UiManager = GameObject.Find("EventSystem").GetComponent<UIManager>();
        cardSelectPanel = GameObject.Find("CardSelectPanel");
        cardSelectPanel.SetActive(false);
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        boardCollider = GameObject.Find("BoardGround").GetComponent<MeshCollider>();
        gameBoard = GameObject.Find("BoardQuad");
        fieldManager = GetComponent<FieldManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InstantiateCard(true);
        }       // *ġƮ* �� ��ο�
        else if (Input.GetKeyDown(KeyCode.X))
        {
            InstantiateCard(false);
        }   // *ġƮ* ��� ī�� ��ο�

        if (isMyCardDrag)
            CardDrag();         //ī�� �巡��

        DetectCardArea();       //���� ĳ��Ʈ
        SetECardState();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < myHandPosition.Count; i++)
            {
                Debug.DrawRay(myHandPosition[i].position, myHandPosition[i].up * 100f, Color.red, 2f);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            RaycastHit hit;
            for(int i = 0; i < fieldManager.enemyFieldPosition.Count; i++)
            {
                if (Physics.Raycast(fieldManager.enemyFieldPosition[i].position,
                    fieldManager.enemyFieldPosition[i].up * 100f, out hit, 100f))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.tag == "Card")
                        Debug.Log("position [" + i + "] ���� ī�� ����!!!!");
                    else
                        Debug.Log("position [" + i + "] ���� ī�� X!!!!");
                }
            }

            for (int i = 0; i < fieldManager.enemyFieldPosition.Count; i++)
            {
                Debug.DrawRay(fieldManager.enemyFieldPosition[i].position,
                    fieldManager.enemyFieldPosition[i].up * 100f, Color.red, 2f);
            }
        }
    }

    public void InstantiateCard(bool isMine)
    {
        if (isMine)
        {
            //�� ī�� ��ο�
            if (myDeckBuffer.Count != 0)
            {//�� ���� ī�尡 �ִٸ�
                if(myHandCard.Count < 10)
                {//�� ���а� 10�� �̸� �� �� ��ο�
                    var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
                    var card = cardObject.GetComponent<Card>();
                    card.Setup(PopItem(), isMine);
                    card.gameObject.name = card.cardName;
                    myHandCard.Add(card.gameObject);
                }
                else
                {
                    Debug.Log("���� 10�� �ʰ� �Ұ�");
                }
            }
            else
            {
                //�� ���� ī�� �� �������� ��
                fieldManager.EmptyDeckDraw(true);
            }
        }
        else if (!isMine)
        {
            //��� ī�� ��ο�
            if(enemyDeckBuffer.Count != 0)
            {//��� ���� ī�尡 �ִٸ�
                if(enemyHandCard.Count < 10)
                {
                    var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
                    var card = cardObject.GetComponent<Card>();
                    card.gameObject.name = card.name;
                    card.Setup(EnemyPopItem(), isMine);
                    card.gameObject.name = "Enemy_" + card.cardName;
                    enemyHandCard.Add(card.gameObject);
                }
                else
                {
                    Debug.Log("Enemy_���� 10�� �ʰ� �Ұ�");
                }
            }
            else
            {
                //��� ���� ī�尡 �� �������� ��
                fieldManager.EmptyDeckDraw(false);
            }
        }
    }

    public Cards PopItem()
    {
        if (myDeckBuffer.Count != 0)
        {
            Cards card = myDeckBuffer[0];
            myHandBuffer.Add(card);             //�� �տ� �ش� ī�� �߰�
            myDeckBuffer.RemoveAt(0);           //�� ���� �ش� ī�� ����
            UiManager.GameInfo(true);
            return card;
        }
        else
        {
            return null;
        }
    }

    public Cards EnemyPopItem()
    {
        if (enemyDeckBuffer.Count != 0)
        {
            Cards card = enemyDeckBuffer[0];
            enemyHandBuffer.Add(card);          //��� �տ� �ش� ī�� �߰�
            enemyDeckBuffer.RemoveAt(0);        //��� ���� �ش� ī�� ����
            UiManager.GameInfo(false);
            return card;
        }
        else
        {   
            return null;
        }
    }

    public void CardMouseOver(Card card)
    {   //���� ī�� ���� ���콺 ����
        if (eCardState == ECardState.Nothing || TurnController.isLoading)
            return;
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {   //���� ī�� ������ ���콺 ���
        EnlargeCard(false, card);
        //selectCard = null;
    }

    public void CardMouseDown(Card card)
    {
        if (eCardState != ECardState.CanMouseDrag)
            return;

        if (card.isField)
        {
            selectCard = card;
        }
        isMyCardDrag = true;        //isMyCardDrag�� true ��� �巡�� ���� (Update)
    }

    public void CardMouseUp(Card card)
    {
        isMyCardDrag = false;       //isMyCardDrag�� false ��� �巡�� �Ұ��� (Update)

        if (eCardState != ECardState.CanMouseDrag)
            return;
        else if (!card.isField)
        {
            if(card.type == "����" && turnController.playerCurMana >= card.cost)
            {//���� ī�� ��ȯ�� ��
                for (int i = 0; i < mousePosHits.Length; i++)
                {   //ī�� �巡�� �� ��ư�� ���� �� ���� �߻�
                    if (mousePosHits[i].transform.tag == "Board")
                    {//ī�带 �ʵ� ���� �巡�� �ϸ� ���� ī�� �ߵ�
                        card.originPRS = new PRS(gameBoard.transform.position, Utils.QI, card.transform.localScale);
                        myHandCard.Remove(card.gameObject);
                        card.MoveTransform(card.originPRS, true, 0.5f);
                        card.isField = true;
                        StartCoroutine(RefreshHand(true));

                        turnController.playerCurMana -= card.cost;
                        turnController.CurrentManaChange();

                        for (int n = 0; n < myHandBuffer.Count; n++)
                        {
                            if (myHandBuffer[n].name == card.cardName)
                            {
                                Debug.Log(myHandBuffer[n].name + " ����Ʈ ����..");
                                myHandBuffer.RemoveAt(n);
                                break;
                            }
                        }
                        UiManager.GameInfo(true);
                        fieldManager.MagicCardSummon(card, true);
                    }
                }
            }
            else
            {//�Ϲ� �ϼ��� ī�� ��ȯ �� ��
                for (int i = 0; i < mousePosHits.Length; i++)
                {   //ī�� �巡�� �� ��ư�� ���� �� ���� �߻�
                    if (mousePosHits[i].transform.tag == "Field")
                    {//�ʵ尡 ���� �Ǿ��ٸ�(���콺�� �� �ڸ��� �ʵ尡 �ִٸ�)
                        for (int j = 0; j < mousePosHits.Length; j++)
                        {//�ʵ� ���� �̹� ī�尡 ��ȯ�Ǿ� �ִ��� üũ
                            if (mousePosHits[j].transform.tag.Equals("Card"))
                            {
                                detectCard++;
                                continue;
                            }
                        }

                        if (detectCard < 2 && turnController.playerCurMana >= card.cost)
                        {//���� ���콺 ��ġ�� ī�尡 2�� ���� ���ٸ�(�ϳ��� �巡�� ���� ī��, �� �ϳ��� �̹� ��ȯ�Ǿ� �ִ� ī��)
                            card.originPRS = new PRS(mousePosHits[i].transform.position, Utils.QI, card.transform.localScale);
                            card.MoveTransform(card.originPRS, true, 0.5f);
                            card.isField = true;                    //�ʵ忡 �ִ°�? true
                            StartCoroutine(RefreshHand(true, 0.5f));    //���� ������ (�� ����)
                                                                        //RefreshHand(true, 0.5f);           //���� ������ (�� ����)
                            turnController.playerCurMana -= card.cost;
                            turnController.CurrentManaChange();
                            myHandCard.Remove(card.gameObject);     //�� ���� ����Ʈ���� �ش� ī�� ����
                            card.attackable = false;
                            fieldManager.myFieldCard.Add(card.gameObject);      //�ʵ� ����Ʈ�� ī�� �߰�

                            for (int n = 0; n < myHandBuffer.Count; n++)
                            {
                                if (myHandBuffer[n].name == card.cardName)
                                {
                                    Debug.Log(myHandBuffer[n].name + " ����Ʈ ����..");
                                    myHandBuffer.RemoveAt(n);
                                    break;
                                }
                            }
                            UiManager.GameInfo(true);
                        }
                    }
                }
            }
            detectCard = 0;
        }
    }

    public void EnlargeCard(bool isEnlarge, Card card)
    {
        if (isEnlarge)
        {//���콺�� ī�� ���� ��ġ�� �ִٸ� ī�� Ȯ��
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, 10.5f, -4f);  //���õ� ī�� Ȯ��� ��ġ(position)
            //new Vector3(2.603866f, 0.01962664f, 3.925328f) = ī�尡 Ȯ�� �� ũ�� (Scale)
            card.MoveTransform(new PRS(enlargePos, Utils.QI, new Vector3(2.603866f, 0.01962664f, 3.925328f)), false);
        }
        else
            card.MoveTransform(card.originPRS, false);  //ī�� ������ ���콺 ����� �ٽ� �������

    }

    private void CardDrag()
    {   //ī�� �巡�� �Ҷ� ���콺 ��ǥ ������ ī�� �̵�
        if (!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(rayPos, Utils.QI, selectCard.originPRS.scale), false);
        }
    }

    private void DetectCardArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition) ;    //���콺 �������� ��ġ���� ���� ��
        RaycastHit[] hits = Physics.RaycastAll(ray);                    //���̿� ���� ��� ������Ʈ�� ������
        RaycastHit hit;
        mousePosHits = hits;

        int layer = LayerMask.NameToLayer("PlayerCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
        
        if(boardCollider.Raycast(ray, out hit, 1000))
        {       //ī�� �巡�� �� �� ���콺 ��ǥ ��
            rayPos = new Vector3(hit.point.x, 10f, hit.point.z);
        }
    }

    public void ShuffleDeck()
    {
        //�� ����
        myDeckBuffer = ShuffleList(myDeckBuffer);
        enemyDeckBuffer = ShuffleList(enemyDeckBuffer);
        //shuffleMyDeckBuffer = ShuffleList(myDeckBuffer);
        //shuffleEnemyDeckBuffer = ShuffleList(enemyDeckBuffer);
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        //�� ���� �Լ�
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);

            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
        return list;
    }

    private void SetECardState()
    {
        //����
        if(turnController.my_Turn == false &&
            turnController.enemy_Turn == false)
        {
            //���� ���� x - ī�� ����, �巡�� �Ұ���
            eCardState = ECardState.Nothing;
        }
        else if(turnController.my_Turn == false &&
            turnController.enemy_Turn == true)
        {
            //��� �� - ī�� ������ ����
            eCardState = ECardState.CanMouseOver;
        }
        else if(turnController.my_Turn == true &&
            turnController.enemy_Turn == false &&
            TurnController.isLoading == false)
        {
            //�� �� - ī�� ����, �巡�� ����
            eCardState = ECardState.CanMouseDrag;
        }
    }

    public IEnumerator RefreshHand(bool isMine, float waitTime = 0)
    {//���� ������
        yield return new WaitForSeconds(waitTime);
        Debug.Log("���� ������");
        if (isMine)
        {//�� ���� ������
            for (int i = 0; i < myHandPosition.Count; i++)
            {
                if (i - 1 >= myHandCard.Count)
                    break;
                Vector3 originPos = new Vector3(myHandPosition[i].position.x, myHandPosition[i].position.y - 0.1f, myHandPosition[i].position.z);
                if (Physics.Raycast(originPos, myHandPosition[i].transform.up, out RaycastHit hit, 10f))
                {
                    if (hit.transform.CompareTag("Card"))
                    {
                        Debug.Log((i + 1) + "�� ������ ī�� ::: " + hit.transform.name);
                    }
                }
                else
                {
                    /*Debug.Log((i + 1) + "�� ������ ������ ����");
                    myHandCard[i].GetComponent<Card>().MoveTransform(
                        new PRS(myHandPosition[i].position, Utils.QI, myHandCard[i + 1].transform.localScale), false);*/
                    for (int j = i; j < myHandCard.Count; j++)
                    {
                        Debug.Log((j + 1) + "�� ������ ������ ���� " + myHandCard[j].name);
                        myHandCard[j].GetComponent<Card>().originPRS =
                            new PRS(myHandPosition[j].position, Utils.QI, myHandCard[j].transform.localScale);
                        myHandCard[j].GetComponent<Card>().MoveTransform(
                        new PRS(myHandPosition[j].position, Utils.QI, myHandCard[j].transform.localScale), false);
                        Debug.Log((j + 1) + "�� ������ ���� �Ϸ�!!!!!!");
                    }
                    break;
                }
            }
        }
        else
        {//��� ���� ������
            for(int i = 0; i < enemyHandPosition.Count; i++)
            {
                if (i - 1 >= enemyHandCard.Count)
                    break;
                Vector3 originPos = new Vector3(enemyHandPosition[i].position.x, enemyHandPosition[i].position.y - 0.1f, enemyHandPosition[i].position.z);
                if(Physics.Raycast(originPos, enemyHandPosition[i].transform.up, out RaycastHit hit, 10f))
                {
                    if (hit.transform.CompareTag("Card"))
                    {
                        Debug.Log((i + 1) + "�� ������ ī�� ::: " + hit.transform.name);
                    }
                }
                else
                {
                    for(int j = i; j < enemyHandCard.Count; j++)
                    {
                        Debug.Log((j + 1) + "�� ������ ������ ���� " + enemyHandCard[j].name);
                        enemyHandCard[j].GetComponent<Card>().originPRS =
                            new PRS(enemyHandPosition[j].position, Utils.QI, enemyHandCard[j].transform.localScale);
                        enemyHandCard[j].GetComponent<Card>().MoveTransform(
                            new PRS(enemyHandPosition[j].position, Utils.QI, enemyHandCard[j].transform.localScale), false);
                        Debug.Log((j + 1) + "�� ������ ���� �Ϸ�!!!!!!");
                    }
                    break;
                }
            }
        }
    }

    public void MoveToGrave(Card card, bool isMine)
    {//���� ī�� �������� ������, isMine = true -> �� ī�� �������� ����
        if (isMine)
        {
            playerCardGrave.Add(card.gameObject);
            card.MoveTransform(new PRS(cardGravePosition[0].position, Utils.QI, card.transform.localScale), true);
        }
        else
        {
            enemyCardGrave.Add(card.gameObject);
            card.transform.position = cardGravePosition[1].position;
        }
        //ī�� ������ �߰��Ǹ� �ش� ī�� ȭ�� ������ �̵���Ű��
    }



    #region Enemy Ȱ��
    public void EnemyTurnStart()
    {//�� �� ����
        StartCoroutine(EnemyActivity());
    }

    IEnumerator EnemyActivity()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("����� �� ����");
        CheckEnemyMoreSummon();
    }

    private void CheckEnemyMoreSummon()
    {
        //enemyMoreSummon = false;
        if(enemyHandCard.Count != 0 && fieldManager.enemyFieldCard.Count != 6)  //���� ���� �ְ�, �ʵ��� ī�尡 6���� �ƴ� ��
        {
            for (int i = 0; i < enemyHandCard.Count; i++)
            {
                if (turnController.enemyCurMana >= enemyHandCard[i].GetComponent<Card>().cost)
                {
                    //enemyMoreSummon = true;
                    StartCoroutine(EnemySummon());
                    break;
                }
                else
                {
                    if (i == enemyHandCard.Count - 1)
                        StartCoroutine(EnemyAttackTest());
                }
            }
        }
        else                                                  //���� ���� ���� �� �ٷ� ���� ����
            StartCoroutine(EnemyAttackTest());
    }

    IEnumerator EnemySummon()
    {
        Card enemySelectCard = EnemyCardSelect(turnController.enemyCurMana);
        if (enemySelectCard != null)
        {
            EnemyCardSummon(enemySelectCard);   //ī�� ��ȯ
            yield return new WaitForSeconds(0.7f);
            StartCoroutine(RefreshHand(false));
            //RefeshHand(false);
        }
        enemySelectCard = null;
        CheckEnemyMoreSummon();
    }

    void EnemyCardSummon(Card selectCard)
    {//�� ī�� �ʵ� ��ȯ
        for(int i = 0; i <= 6; i++)
        {
            if(fieldManager.enemyFieldCard.Count <= i)
            {
                if(fieldManager.enemyFieldCard.Count == 6)
                {
                    break;
                }
                if(turnController.enemyMana >= selectCard.cost)
                {
                    turnController.enemyCurMana -= selectCard.cost;        //��ȯ�� ī���� �ڽ�Ʈ��ŭ ���� ����
                    turnController.CurrentManaChange();                 //���� ���� ���� üũ

                    Vector3 fieldPos;
                    fieldPos = fieldManager.EnemyFieldCheck(selectCard.originPRS.pos);     
                    selectCard.originPRS = new PRS(fieldPos, Utils.QI, selectCard.transform.localScale);
                    selectCard.originPRS.pos.y += 0.01f;
                    selectCard.transform.rotation = Quaternion.Euler(180, -180, -180);
                    turnController.Loading(0.51f);
                    selectCard.MoveTransform(selectCard.originPRS, true, 0.5f);
                    selectCard.attackable = false;
                    selectCard.isField = true;

                    fieldManager.enemyFieldCard.Add(selectCard.gameObject);
                    enemyHandCard.Remove(selectCard.gameObject);
                    EnemyHandBufferInit(selectCard);    //���� �ʱ�ȭ
                    break;
                }
                else
                {
                    Debug.Log("���� ����");
                    break;
                }
            }
        }
    }

    private Card EnemyCardSelect(int curMana)
    {
        Card enemySelectCard = null;
        for(int i = 0; i < enemyHandCard.Count; i++)
        {
            if (enemyHandCard[i].GetComponent<Card>().cost <= curMana)
            {
                enemySelectCard = enemyHandCard[i].GetComponent<Card>();
                Debug.Log(i+1 + "��° ī�� ����");
                break;
            }
        }
        return enemySelectCard;
    }

    private IEnumerator EnemyAttackTest()
    {
        Card[] enemyFieldCard = new Card[6];
        Card[] myFieldCard = new Card[6];
        Debug.Log("�� ���� Ȱ�� ����");

        for (int i = 0; i < fieldManager.enemyFieldCard.Count; i++)
        {//i = 0 ���� �� (ù��° ī�尡 ������ ������ ��) �ش� ī�� ���� �� �ı��Ǹ� �ʵ� ����Ʈ���� ���ŵ�, �׷��� �������� i = 1 �̸� ��������� 3��° ī�尡 ������ �ϰ� ��
         //�ذ� ���? for ���� ������ �ʵ� �� ��ȯ�Ǿ� �ִ� ī�带 �ν� �� �� ī���� �ν��Ͻ��� ���� �� �� �ν��Ͻ��� ������ ���� �ڵ� �ۼ�?
            if (fieldManager.enemyFieldCard[i] != null)
            {
                enemyFieldCard[i] = fieldManager.enemyFieldCard[i].GetComponent<Card>();
            }
        }
        for(int i = 0; i < fieldManager.myFieldCard.Count; i++)
        {
            if (fieldManager.myFieldCard[i] != null)
            {
                myFieldCard[i] = fieldManager.myFieldCard[i].GetComponent<Card>();
            }
        }

        for(int i = 0; i < enemyFieldCard.Length; i++)
        {
            if (enemyFieldCard[i] != null && enemyFieldCard[i].attackable && enemyFieldCard[i].isDead == false)
            {
                Debug.Log("����" + (i + 1) + "��° ī�� ���� ����");
                for (int j = 0; j < myFieldCard.Length; j++)
                {
                        if (myFieldCard[j] != null && myFieldCard[j].isDead == false && myFieldCard[j].health - enemyFieldCard[i].attack <= 0)
                        {
                            fieldManager.AttackTarget(enemyFieldCard[i], myFieldCard[j]);
                            yield return new WaitForSeconds(0.9f);
                            break;
                        }
                }
                //���� ������ ī�尡 ���� ���� �����̰� �� �ʵ��� ī�带 �������� �ʾ�����(���� �Ҹ���) �� ��ġ�� ����
                if (enemyFieldCard[i].attackable)
                {
                    fieldManager.AttackMainCharacter(enemyFieldCard[i], false);
                    yield return new WaitForSeconds(0.9f);
                }
            }
        }

        for(int i = 0; i < 6; i++)
        {
            enemyFieldCard[i] = null;
            myFieldCard[i] = null;
        }

        //��� ���� �� ������ �� �ǰ� 0 ���ϸ� ���ӿ��� (���ӿ��� ���Ŀ� �� �� ���Ḧ �ϰԵǸ� ������)
        yield return new WaitForSeconds(0.9f);
        if(turnController.playerHp > 0)
        {
            turnController.enemyDone = true;
        }
    }

    void EnemyHandBufferInit(Card selectCard)
    {
        for (int n = 0; n < enemyHandBuffer.Count; n++)
        {
            if (enemyHandBuffer[n].name == selectCard.cardName)
            {
                Debug.Log(enemyHandBuffer[n].name + " ����Ʈ ����..");
                enemyHandBuffer.RemoveAt(n);
                UiManager.GameInfo(false);
                break;
            }
        }
    }
    #endregion
}
