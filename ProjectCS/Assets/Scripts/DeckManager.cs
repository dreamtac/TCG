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
    {   //상태
        Nothing = 0,    //마우스 오버, 드래그 불가능
        CanMouseOver,   //마우스 오버 가능, 드래그 불가능
        CanMouseDrag    //마우스 오버, 드래그 모두 가능
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
        }       // *치트* 내 드로우
        else if (Input.GetKeyDown(KeyCode.X))
        {
            InstantiateCard(false);
        }   // *치트* 상대 카드 드로우

        if (isMyCardDrag)
            CardDrag();         //카드 드래그

        DetectCardArea();       //레이 캐스트
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
                        Debug.Log("position [" + i + "] 위에 카드 있음!!!!");
                    else
                        Debug.Log("position [" + i + "] 위에 카드 X!!!!");
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
            //내 카드 드로우
            if (myDeckBuffer.Count != 0)
            {//내 덱에 카드가 있다면
                if(myHandCard.Count < 10)
                {//내 손패가 10장 미만 일 때 드로우
                    var cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Utils.QI);
                    var card = cardObject.GetComponent<Card>();
                    card.Setup(PopItem(), isMine);
                    card.gameObject.name = card.cardName;
                    myHandCard.Add(card.gameObject);
                }
                else
                {
                    Debug.Log("손패 10장 초과 불가");
                }
            }
            else
            {
                //내 덱에 카드 다 떨어졌을 때
                fieldManager.EmptyDeckDraw(true);
            }
        }
        else if (!isMine)
        {
            //상대 카드 드로우
            if(enemyDeckBuffer.Count != 0)
            {//상대 덱에 카드가 있다면
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
                    Debug.Log("Enemy_손패 10장 초과 불가");
                }
            }
            else
            {
                //상대 덱에 카드가 다 떨어졌을 때
                fieldManager.EmptyDeckDraw(false);
            }
        }
    }

    public Cards PopItem()
    {
        if (myDeckBuffer.Count != 0)
        {
            Cards card = myDeckBuffer[0];
            myHandBuffer.Add(card);             //내 손에 해당 카드 추가
            myDeckBuffer.RemoveAt(0);           //내 덱에 해당 카드 삭제
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
            enemyHandBuffer.Add(card);          //상대 손에 해당 카드 추가
            enemyDeckBuffer.RemoveAt(0);        //상대 덱에 해당 카드 삭제
            UiManager.GameInfo(false);
            return card;
        }
        else
        {   
            return null;
        }
    }

    public void CardMouseOver(Card card)
    {   //손패 카드 위에 마우스 오버
        if (eCardState == ECardState.Nothing || TurnController.isLoading)
            return;
        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Card card)
    {   //손패 카드 위에서 마우스 벗어남
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
        isMyCardDrag = true;        //isMyCardDrag가 true 라면 드래그 가능 (Update)
    }

    public void CardMouseUp(Card card)
    {
        isMyCardDrag = false;       //isMyCardDrag가 false 라면 드래그 불가능 (Update)

        if (eCardState != ECardState.CanMouseDrag)
            return;
        else if (!card.isField)
        {
            if(card.type == "마법" && turnController.playerCurMana >= card.cost)
            {//마법 카드 소환할 때
                for (int i = 0; i < mousePosHits.Length; i++)
                {   //카드 드래그 중 버튼을 땟을 때 레이 발사
                    if (mousePosHits[i].transform.tag == "Board")
                    {//카드를 필드 위로 드래그 하면 마법 카드 발동
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
                                Debug.Log(myHandBuffer[n].name + " 리스트 제거..");
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
            {//일반 하수인 카드 소환 할 때
                for (int i = 0; i < mousePosHits.Length; i++)
                {   //카드 드래그 중 버튼을 땟을 때 레이 발사
                    if (mousePosHits[i].transform.tag == "Field")
                    {//필드가 감지 되었다면(마우스를 뗀 자리에 필드가 있다면)
                        for (int j = 0; j < mousePosHits.Length; j++)
                        {//필드 위에 이미 카드가 소환되어 있는지 체크
                            if (mousePosHits[j].transform.tag.Equals("Card"))
                            {
                                detectCard++;
                                continue;
                            }
                        }

                        if (detectCard < 2 && turnController.playerCurMana >= card.cost)
                        {//만약 마우스 위치에 카드가 2장 보다 적다면(하나는 드래그 중인 카드, 또 하나는 이미 소환되어 있는 카드)
                            card.originPRS = new PRS(mousePosHits[i].transform.position, Utils.QI, card.transform.localScale);
                            card.MoveTransform(card.originPRS, true, 0.5f);
                            card.isField = true;                    //필드에 있는가? true
                            StartCoroutine(RefreshHand(true, 0.5f));    //손패 재정렬 (내 손패)
                                                                        //RefreshHand(true, 0.5f);           //손패 재정렬 (내 손패)
                            turnController.playerCurMana -= card.cost;
                            turnController.CurrentManaChange();
                            myHandCard.Remove(card.gameObject);     //내 손패 리스트에서 해당 카드 삭제
                            card.attackable = false;
                            fieldManager.myFieldCard.Add(card.gameObject);      //필드 리스트에 카드 추가

                            for (int n = 0; n < myHandBuffer.Count; n++)
                            {
                                if (myHandBuffer[n].name == card.cardName)
                                {
                                    Debug.Log(myHandBuffer[n].name + " 리스트 제거..");
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
        {//마우스가 카드 위에 위치해 있다면 카드 확대
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, 10.5f, -4f);  //선택된 카드 확대될 위치(position)
            //new Vector3(2.603866f, 0.01962664f, 3.925328f) = 카드가 확대 될 크기 (Scale)
            card.MoveTransform(new PRS(enlargePos, Utils.QI, new Vector3(2.603866f, 0.01962664f, 3.925328f)), false);
        }
        else
            card.MoveTransform(card.originPRS, false);  //카드 위에서 마우스 벗어나면 다시 원래대로

    }

    private void CardDrag()
    {   //카드 드래그 할때 마우스 좌표 값으로 카드 이동
        if (!onMyCardArea)
        {
            selectCard.MoveTransform(new PRS(rayPos, Utils.QI, selectCard.originPRS.scale), false);
        }
    }

    private void DetectCardArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition) ;    //마우스 포인터의 위치에서 레이 쏨
        RaycastHit[] hits = Physics.RaycastAll(ray);                    //레이에 맞은 모든 오브젝트들 가져옴
        RaycastHit hit;
        mousePosHits = hits;

        int layer = LayerMask.NameToLayer("PlayerCardArea");
        onMyCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
        
        if(boardCollider.Raycast(ray, out hit, 1000))
        {       //카드 드래그 할 때 마우스 좌표 값
            rayPos = new Vector3(hit.point.x, 10f, hit.point.z);
        }
    }

    public void ShuffleDeck()
    {
        //덱 섞기
        myDeckBuffer = ShuffleList(myDeckBuffer);
        enemyDeckBuffer = ShuffleList(enemyDeckBuffer);
        //shuffleMyDeckBuffer = ShuffleList(myDeckBuffer);
        //shuffleEnemyDeckBuffer = ShuffleList(enemyDeckBuffer);
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        //덱 섞는 함수
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
        //상태
        if(turnController.my_Turn == false &&
            turnController.enemy_Turn == false)
        {
            //게임 시작 x - 카드 오버, 드래그 불가능
            eCardState = ECardState.Nothing;
        }
        else if(turnController.my_Turn == false &&
            turnController.enemy_Turn == true)
        {
            //상대 턴 - 카드 오버만 가능
            eCardState = ECardState.CanMouseOver;
        }
        else if(turnController.my_Turn == true &&
            turnController.enemy_Turn == false &&
            TurnController.isLoading == false)
        {
            //내 턴 - 카드 오버, 드래그 가능
            eCardState = ECardState.CanMouseDrag;
        }
    }

    public IEnumerator RefreshHand(bool isMine, float waitTime = 0)
    {//손패 재정렬
        yield return new WaitForSeconds(waitTime);
        Debug.Log("손패 재정렬");
        if (isMine)
        {//내 손패 재정렬
            for (int i = 0; i < myHandPosition.Count; i++)
            {
                if (i - 1 >= myHandCard.Count)
                    break;
                Vector3 originPos = new Vector3(myHandPosition[i].position.x, myHandPosition[i].position.y - 0.1f, myHandPosition[i].position.z);
                if (Physics.Raycast(originPos, myHandPosition[i].transform.up, out RaycastHit hit, 10f))
                {
                    if (hit.transform.CompareTag("Card"))
                    {
                        Debug.Log((i + 1) + "번 포지션 카드 ::: " + hit.transform.name);
                    }
                }
                else
                {
                    /*Debug.Log((i + 1) + "번 포지션 재정렬 실행");
                    myHandCard[i].GetComponent<Card>().MoveTransform(
                        new PRS(myHandPosition[i].position, Utils.QI, myHandCard[i + 1].transform.localScale), false);*/
                    for (int j = i; j < myHandCard.Count; j++)
                    {
                        Debug.Log((j + 1) + "번 포지션 재정렬 실행 " + myHandCard[j].name);
                        myHandCard[j].GetComponent<Card>().originPRS =
                            new PRS(myHandPosition[j].position, Utils.QI, myHandCard[j].transform.localScale);
                        myHandCard[j].GetComponent<Card>().MoveTransform(
                        new PRS(myHandPosition[j].position, Utils.QI, myHandCard[j].transform.localScale), false);
                        Debug.Log((j + 1) + "번 포지션 정렬 완료!!!!!!");
                    }
                    break;
                }
            }
        }
        else
        {//상대 손패 재정렬
            for(int i = 0; i < enemyHandPosition.Count; i++)
            {
                if (i - 1 >= enemyHandCard.Count)
                    break;
                Vector3 originPos = new Vector3(enemyHandPosition[i].position.x, enemyHandPosition[i].position.y - 0.1f, enemyHandPosition[i].position.z);
                if(Physics.Raycast(originPos, enemyHandPosition[i].transform.up, out RaycastHit hit, 10f))
                {
                    if (hit.transform.CompareTag("Card"))
                    {
                        Debug.Log((i + 1) + "번 포지션 카드 ::: " + hit.transform.name);
                    }
                }
                else
                {
                    for(int j = i; j < enemyHandCard.Count; j++)
                    {
                        Debug.Log((j + 1) + "번 포지션 재정렬 실행 " + enemyHandCard[j].name);
                        enemyHandCard[j].GetComponent<Card>().originPRS =
                            new PRS(enemyHandPosition[j].position, Utils.QI, enemyHandCard[j].transform.localScale);
                        enemyHandCard[j].GetComponent<Card>().MoveTransform(
                            new PRS(enemyHandPosition[j].position, Utils.QI, enemyHandCard[j].transform.localScale), false);
                        Debug.Log((j + 1) + "번 포지션 정렬 완료!!!!!!");
                    }
                    break;
                }
            }
        }
    }

    public void MoveToGrave(Card card, bool isMine)
    {//죽은 카드 무덤으로 보내기, isMine = true -> 내 카드 무덤으로 보냄
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
        //카드 무덤에 추가되면 해당 카드 화면 밖으로 이동시키기
    }



    #region Enemy 활동
    public void EnemyTurnStart()
    {//적 턴 시작
        StartCoroutine(EnemyActivity());
    }

    IEnumerator EnemyActivity()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("상대편 턴 시작");
        CheckEnemyMoreSummon();
    }

    private void CheckEnemyMoreSummon()
    {
        //enemyMoreSummon = false;
        if(enemyHandCard.Count != 0 && fieldManager.enemyFieldCard.Count != 6)  //상대방 손패 있고, 필드위 카드가 6장이 아닐 때
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
        else                                                  //상대방 손패 없을 때 바로 공격 시작
            StartCoroutine(EnemyAttackTest());
    }

    IEnumerator EnemySummon()
    {
        Card enemySelectCard = EnemyCardSelect(turnController.enemyCurMana);
        if (enemySelectCard != null)
        {
            EnemyCardSummon(enemySelectCard);   //카드 소환
            yield return new WaitForSeconds(0.7f);
            StartCoroutine(RefreshHand(false));
            //RefeshHand(false);
        }
        enemySelectCard = null;
        CheckEnemyMoreSummon();
    }

    void EnemyCardSummon(Card selectCard)
    {//적 카드 필드 소환
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
                    turnController.enemyCurMana -= selectCard.cost;        //소환한 카드의 코스트만큼 마나 차감
                    turnController.CurrentManaChange();                 //상대방 현재 마나 체크

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
                    EnemyHandBufferInit(selectCard);    //버퍼 초기화
                    break;
                }
                else
                {
                    Debug.Log("마나 부족");
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
                Debug.Log(i+1 + "번째 카드 선택");
                break;
            }
        }
        return enemySelectCard;
    }

    private IEnumerator EnemyAttackTest()
    {
        Card[] enemyFieldCard = new Card[6];
        Card[] myFieldCard = new Card[6];
        Debug.Log("적 공격 활동 시작");

        for (int i = 0; i < fieldManager.enemyFieldCard.Count; i++)
        {//i = 0 번일 때 (첫번째 카드가 공격을 시작할 때) 해당 카드 공격 후 파괴되면 필드 리스트에서 제거됨, 그러면 다음으로 i = 1 이면 결과적으로 3번째 카드가 공격을 하게 됨
         //해결 방법? for 문을 돌려서 필드 위 소환되어 있는 카드를 인식 후 각 카드의 인스턴스를 생성 후 그 인스턴스를 가지고 공격 코드 작성?
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
                Debug.Log("상대방" + (i + 1) + "번째 카드 공격 시작");
                for (int j = 0; j < myFieldCard.Length; j++)
                {
                        if (myFieldCard[j] != null && myFieldCard[j].isDead == false && myFieldCard[j].health - enemyFieldCard[i].attack <= 0)
                        {
                            fieldManager.AttackTarget(enemyFieldCard[i], myFieldCard[j]);
                            yield return new WaitForSeconds(0.9f);
                            break;
                        }
                }
                //만약 상대방의 카드가 공격 가능 상태이고 내 필드의 카드를 공격하지 않았으면(조건 불만족) 내 명치를 공격
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

        //상대 공격 다 끝나고 내 피가 0 이하면 게임오버 (게임오버 이후에 적 턴 종료를 하게되면 오류남)
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
                Debug.Log(enemyHandBuffer[n].name + " 리스트 제거..");
                enemyHandBuffer.RemoveAt(n);
                UiManager.GameInfo(false);
                break;
            }
        }
    }
    #endregion
}
