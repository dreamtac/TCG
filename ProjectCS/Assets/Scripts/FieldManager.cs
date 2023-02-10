using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class FieldManager : MonoBehaviour
{
    public GameObject textDamage;
    private TurnController turnController;
    private DeckManager deckManager;
    private Button continueBtn;
    private Button restartBtn;
    private Button rebuildDeckBtn;
    private Button goToMainBtn;
    private Button quitBtn;
    private Vector3 quitBtnOriginPos;
    private Vector3 restartBtnOriginPos;
    private Vector3 continueBtnOriginPos;
    private Vector3 rebuildDeckBtnOriginPos;
    private CameraMove cameraMove;
    private GameObject ingamePanel;

    private Card magicCard;
    private Card magicTarget;

    public List<Transform> myFieldPosition;
    public List<Transform> enemyFieldPosition;
    public List<GameObject> myFieldCard;
    public List<GameObject> enemyFieldCard;

    Card selectCard;    //공격을 실행하는 카드
    Card targetCard;    //공격을 당하는 카드

    public bool magicSelect;
    private GameObject gameOverPanel;
    public TMP_Text resultText;

    [SerializeField] bool onAttack;     //MouseUp 시 true 일 때 공격 실행
    [SerializeField] bool onEnemyAttack;
    SpriteRenderer playerMainCharaceter;
    SpriteRenderer enemyMainCharaceter;

    private void Awake()
    {
        restartBtn = GameObject.Find("RestartBtn").GetComponent<Button>();
        quitBtn = GameObject.Find("QuitBtn").GetComponent<Button>();
        continueBtn = GameObject.Find("ContinueBtn").GetComponent<Button>();
        rebuildDeckBtn = GameObject.Find("RebuildDeckBtn").GetComponent<Button>();
        cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
        ingamePanel = GameObject.Find("InGamePanel");
        goToMainBtn = GameObject.Find("GoToMainBtn").GetComponent<Button>();
        quitBtnOriginPos = quitBtn.transform.localPosition;
        restartBtnOriginPos = restartBtn.gameObject.transform.localPosition;
        continueBtnOriginPos = continueBtn.gameObject.transform.localPosition;
        rebuildDeckBtnOriginPos = rebuildDeckBtn.gameObject.transform.localPosition;
    }
    private void Start()
    {
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        playerMainCharaceter = GameObject.Find("PlayerCharacter").GetComponent<SpriteRenderer>();
        enemyMainCharaceter = GameObject.Find("EnemyCharacter").GetComponent<SpriteRenderer>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        resultText = GameObject.Find("ResultText").GetComponent<TextMeshProUGUI>();
        deckManager = GetComponent<DeckManager>();
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (magicSelect)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    //magicSelect = false;
                    if(hit.transform.tag == "Card")
                    {
                        if (hit.transform.GetComponent<Card>().isFront == false && hit.transform.GetComponent<Card>().isField == true)
                        {//클릭한 카드가 상대 카드이고, 필드위에 소환된 상태라면
                            Debug.Log(hit.transform.gameObject);
                            magicTarget = hit.transform.GetComponent<Card>();

                            for (int i = 0; i < enemyFieldCard.Count; i++)
                            {//상대 필드 위 카드들 포커싱 해제
                                enemyFieldCard[i].GetComponent<Card>().MoveTransform(enemyFieldCard[i].GetComponent<Card>().originPRS,
                                    false);
                            }

                            magicSelect = false;
                            MagicType2(magicCard, magicTarget);
                            magicCard = null;
                            magicTarget = null;
                        }
                        else
                        {
                            Debug.Log("필드 위 카드를 선택해 주세요");
                        }
                    }
                }
            }
        }
    }

    public void FieldMouseDown(Card card)
    {
        if (turnController.my_Turn)
        {//내 턴일 때
            Debug.Log("필드 ::: 마우스 다운");
            selectCard = card;
        }
    }

    public void FieldMouseUp(Card card)
    {
        if (turnController.my_Turn)
        {//내 턴일 때
            if (onAttack)
            {//상대 필드위 카드 공격
                AttackTarget(selectCard, targetCard); //공격 실행
                Debug.Log("공격 실행 ::: \n SelectCard = " + selectCard.cardName +
                    "\n TargetCard = " + targetCard.cardName);
            }
            else if (onEnemyAttack)
            {//상대 명치 공격
                AttackMainCharacter(selectCard, true);
            }

            selectCard = null;
            targetCard = null;
            onAttack = false;
            onEnemyAttack = false;
        }
    }

    public void FieldMouseDrag(Card card)
    {
        if (turnController.my_Turn)
        {//내 턴일 때
            selectCard = card;
            selectCard.MoveTransform(new PRS(DeckManager.rayPos, Utils.QI, selectCard.originPRS.scale), false);
            bool existTarget = false;
            foreach (var hit in Physics.RaycastAll(DeckManager.rayPos, Vector3.down))
            {
                Card hitCard = hit.collider?.GetComponent<Card>();
                if (hitCard != null && !hitCard.isFront && selectCard.attackable && hitCard.isField)
                {
                    targetCard = hitCard;
                    existTarget = true;
                    Debug.Log("타겟 카드 확인 ::: " + targetCard.cardName);
                    onAttack = true;
                    break;
                }
                if (hit.collider.tag.Equals("Enemy"))
                {
                    Debug.Log("상대 명치 확인 ::: " + hit.transform.name);
                    existTarget = true;
                    onEnemyAttack = true;
                }
            }
            if (!existTarget)
            {
                targetCard = null;
                onAttack = false;
                onEnemyAttack = false;
                Debug.Log("타겟 카드, 명치 없음");
            }            
        }
    }

    public void MagicCardSummon(Card card, bool isMine)
    {
        //마법 카드 소환
        if (isMine)
        {//내가 낸 마법카드라면
            //Type == 1 ------------------------------------------------------------
            if (card.magicType == 1)
            {//적 필드위 카드 전체 공격
                for (int i = 0; i < enemyFieldCard.Count; i++)
                {
                    enemyFieldCard[i].GetComponent<Card>().health -= card.magicValue;
                    enemyFieldCard[i].GetComponent<Card>().Damaged(card.magicValue);
                    enemyFieldCard[i].GetComponent<Card>().InitStatus();
                    if (enemyFieldCard[i].GetComponent<Card>().health <= 0)
                    {
                        enemyFieldCard[i].GetComponent<Card>().isDead = true;
                    }
                    Invoke("CheckCardHp", 0.85f);
                }
                StartCoroutine(MagicCardActive(card, isMine));    //마법 카드는 필드위에 소환 x 효과만 발동하고 바로 묘지로 이동
            }
            //Type == 2 ------------------------------------------------------------
            else if (card.magicType == 2)
            {//상대 필드 위 카드 한장 공격
                 StartCoroutine(MagicCardActive(card, isMine, 2));
            }
            //Type == 3 ------------------------------------------------------------
            else if (card.magicType == 3)
            {//카드 드로우
                StartCoroutine(MagicCardActive(card, isMine, 3));    //마법 카드는 필드위에 소환 x 효과만 발동하고 바로 묘지로 이동
            }
        }
    }

    IEnumerator MagicCardActive(Card card, bool isMine, int type = 1)
    {
        if(type == 1)
        {
            yield return new WaitForSeconds(0.55f);
            deckManager.MoveToGrave(card, isMine);
        }        

        else if(type == 2)
        {//상대 필드 위 카드 약간 확대, 플레이어가 공격할 카드 선택
            //추가 할 것 - 상대 필드 위에 카드 없으면 취소? 혹은 그냥 카드 버리기?
            magicSelect = true;
            magicCard = card;

            for (int i = 0; i < enemyFieldCard.Count; i++)
            {//상대 필드 위 카드들 공중으로 띄우기 (포커싱)
                enemyFieldCard[i].transform.position = new Vector3(enemyFieldCard[i].transform.position.x,
                    enemyFieldCard[i].transform.position.y + 1.5f, enemyFieldCard[i].transform.position.z);
            }
        }

        else if(type == 3)
        {
            for (int i = 0; i < card.magicValue; i++)
            {
                deckManager.StartCoroutine(deckManager.RefreshHand(isMine));
                deckManager.InstantiateCard(isMine);
                yield return new WaitForSeconds(0.5f);
                deckManager.MoveToGrave(card, isMine);
            }
        }
    }

    public void MagicType2(Card magicCard, Card targetCard)
    {
        deckManager.MoveToGrave(magicCard, magicCard.isFront);

        targetCard.health -= magicCard.magicValue;
        targetCard.Damaged(magicCard.magicValue);
        targetCard.InitStatus();
        if(targetCard.health <= 0)
        {
            targetCard.isDead = true;
            Invoke("CheckCardHp", 0.85f);
        }
    }

    public void AttackTarget(Card select, Card target)
    {//공격 실행 select => 공격 실행 카드/ target => 공격 받는 카드
        select.MoveTransform(select.originPRS, false);
        turnController.Loading(0.85f);
        Sequence sequence = DOTween.Sequence()
            .Append(select.transform.DOMove(target.originPRS.pos, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {
                select.gameObject.GetComponent<BGMController>().Card_Attack();          //카드 공격 효과음
                select.health -= target.attack;
                select.Damaged(target.attack);
                target.health -= select.attack;
                target.Damaged(select.attack);
                cameraMove.ShakeCamera();

                select.attackable = false;
                select.InitStatus();    //대상 카드 생명력 초기화
                target.InitStatus();    //선택 카드 생명력 초기화

                if (select.health <= 0)
                {//hp가 0으로 떨어지면 해당 카드 Dead 상태로
                    select.isDead = true;
                }
                if (target.health <= 0)
                {//hp가 0으로 떨어지면 해당 카드 Dead 상태로
                    target.isDead = true;
                }
            })
            .Append(select.transform.DOMove(select.originPRS.pos, 0.4f)).SetEase(Ease.OutSine);
        Invoke("CheckCardHp", 0.85f);
    }

    public void CardAttackableInit(bool myTurn)
    {//myTurn == true -> 내 턴 종료 , false -> 상대 턴 종료
        if (myTurn)
        {
            for(int i = 0; i < myFieldCard.Count; i++)
            {
                myFieldCard[i].GetComponent<Card>().attackable = true;
            }
        }
        else
        {
            for(int i = 0; i < enemyFieldCard.Count; i++)
            {
                enemyFieldCard[i].GetComponent<Card>().attackable = true;
            }
        }
    }

    public Vector3 EnemyFieldCheck(Vector3 originPos)
    {//상대방 카드 소환 전 필드 위 소환 위치 탐색
        RaycastHit hit;
        Vector3 pos;
        for (int i = 0; i < enemyFieldPosition.Count; i++)
        {
            if (Physics.Raycast(enemyFieldPosition[i].position,
                enemyFieldPosition[i].up * 100f, out hit, 100f))
            {
                if (hit.transform.tag == "Card")
                    Debug.Log("position [" + i + "] ::: " + hit.transform.name);
                continue;
            }
            else
            {
                pos = enemyFieldPosition[i].position;
                return pos;
            }
        }

        return originPos;
    }

    public void AttackMainCharacter(Card card, bool target)
    {// 명치 공격 ::: target = true -> 내가 상대방 공격, false -> 상대방이 나를 공격
        if (target)
        {//내가 상대방 명치 공격
            card.MoveTransform(card.originPRS, false);
            turnController.Loading(0.81f);
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(enemyMainCharaceter.gameObject.transform.position, 0.4f))
                .SetEase(Ease.InSine).AppendCallback(() =>
                {
                    card.gameObject.GetComponent<BGMController>().Character_Attack();       //캐릭터 공격 효과음
                    turnController.enemyHp -= card.attack;
                    cameraMove.ShakeCamera();
                    card.attackable = false;
                    GameObject.Find("EnemyHpText").GetComponent<TMP_Text>().text = turnController.enemyHp.ToString();
                    if(turnController.playerHp <= 0 || turnController.enemyHp <= 0)
                    {
                        CheckCharacterHP();
                        ingamePanel.SetActive(false);
                    }
                })
                .Append(card.transform.DOMove(card.originPRS.pos, 0.4f)).SetEase(Ease.OutSine);
        }
        else
        {//상대방이 내 명치 공격
            card.MoveTransform(card.originPRS, false);
            turnController.Loading(0.81f);
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(playerMainCharaceter.gameObject.transform.position, 0.4f))
                .SetEase(Ease.InSine).AppendCallback(() =>
                {
                    card.gameObject.GetComponent<BGMController>().Character_Attack();       //캐릭터 공격 효과음
                    turnController.playerHp -= card.attack;
                    cameraMove.ShakeCamera();
                    card.attackable = false;
                    GameObject.Find("PlayerHpText").GetComponent<TMP_Text>().text = turnController.playerHp.ToString();
                    if (turnController.playerHp <= 0 || turnController.enemyHp <= 0)
                    {
                        CheckCharacterHP();
                        ingamePanel.SetActive(false);
                    }
                })
                .Append(card.transform.DOMove(card.originPRS.pos, 0.4f)).SetEase(Ease.OutSine);
        }
    }

    public void EmptyDeckDraw(bool target)
    {
        if (target)
        {//내 덱에 카드가 없을 때
            Debug.Log("덱에 카드 없음 데미지 3");
            turnController.playerHp -= 3;
            cameraMove.ShakeCamera();
            GameObject.Find("PlayerHpText").GetComponent<TMP_Text>().text = turnController.playerHp.ToString();
            if (turnController.playerHp <= 0 || turnController.enemyHp <= 0)
            {
                CheckCharacterHP();
                ingamePanel.SetActive(false);
            }
        }
        else
        {//상대방 덱에 카드가 없을 때
            turnController.enemyHp -= 3;
            cameraMove.ShakeCamera();
            GameObject.Find("EnemyHpText").GetComponent<TMP_Text>().text = turnController.enemyHp.ToString();
            if (turnController.playerHp <= 0 || turnController.enemyHp <= 0)
            {
                CheckCharacterHP();
                ingamePanel.SetActive(false);
            }

        }
    }

    public void CheckCardHp()
    {//카드 Hp체크 및 무덤으로 이동
        for (int i = 0; i < myFieldCard.Count; i++)
        {
            if (myFieldCard[i].GetComponent<Card>().isDead)
            {// 내 필드 i번째 카드가 Dead 상태라면
                deckManager.MoveToGrave(myFieldCard[i].GetComponent<Card>(), true);
                myFieldCard.RemoveAt(i);
                i--;    //i번째 카드가 리스트에서 빠지면 List.Count에서 꼬이게 됨 그래서 i를 -1 해줌
            }
        }

        for (int i = 0; i < enemyFieldCard.Count; i++)
        {
            if (enemyFieldCard[i].GetComponent<Card>().isDead)
            {// 상대방 필드 i번째 카드가 Dead 상태라면
                deckManager.MoveToGrave(enemyFieldCard[i].GetComponent<Card>(), false);
                enemyFieldCard.RemoveAt(i);
                i--;    //i번째 카드가 리스트에서 빠지면 List.Count에서 꼬이게 됨 그래서 i를 -1 해줌
            }
        }
    }

    public void CheckCharacterHP()
    {
        //명치 체력 체크 (게임 오버)
        if(turnController.playerHp <= 0)
        {
            //재도전, 덱변경, 게임종료
            gameObject.GetComponent<BGMController>().Game_Defeat();             //패배 효과음 재생
            gameOverPanel.SetActive(true);
            resultText.text = "패배";
            Time.timeScale = 0;
            restartBtn.gameObject.SetActive(true);
            rebuildDeckBtn.gameObject.SetActive(true);
            quitBtn.gameObject.SetActive(true);
            continueBtn.gameObject.SetActive(false);
            goToMainBtn.gameObject.SetActive(false);
            restartBtn.transform.localPosition = continueBtnOriginPos;
            rebuildDeckBtn.transform.localPosition = rebuildDeckBtnOriginPos;
            quitBtn.transform.localPosition = quitBtnOriginPos;
        }
        else if(turnController.enemyHp <= 0)
        {
            //메인메뉴, 재도전, 게임종료
            gameObject.GetComponent<BGMController>().Game_Victory();             //승리 효과음 재생
            gameOverPanel.SetActive(true);
            resultText.text = "승리";
            Time.timeScale = 0;
            goToMainBtn.gameObject.SetActive(true);
            continueBtn.gameObject.SetActive(false);
            rebuildDeckBtn.gameObject.SetActive(false);
            restartBtn.gameObject.SetActive(true);
            restartBtn.transform.localPosition = restartBtnOriginPos;
            quitBtn.transform.localPosition = quitBtnOriginPos;
        }
        else
        {
            if(turnController.turnCount != 0)
            {
                //경기재개, 재도전, 게임종료
                if (gameOverPanel.activeSelf == true && resultText.text == "일시정지")
                {
                    gameOverPanel.SetActive(false);
                    ingamePanel.SetActive(true);
                    Time.timeScale = 1;
                    return;
                }
                gameObject.GetComponent<BGMController>().Game_Pause();              //일시정지 효과음 재생
                resultText.text = "일시정지";
                Time.timeScale = 0;
                continueBtn.gameObject.SetActive(true);
                restartBtn.gameObject.SetActive(true);
                quitBtn.gameObject.SetActive(true);
                goToMainBtn.gameObject.SetActive(false);
                rebuildDeckBtn.gameObject.SetActive(false);
                continueBtn.transform.localPosition = continueBtnOriginPos;
                restartBtn.transform.localPosition = restartBtnOriginPos;
                quitBtn.transform.localPosition = quitBtnOriginPos;
                gameOverPanel.SetActive(true);
            }
            else
            {
                ////경기재개, 덱변경, 게임종료
                //if (gameOverPanel.activeSelf == true && resultText.text == "일시정지")
                //{
                //    quitBtn.gameObject.transform.localPosition = quitBtnOriginPos;
                //    restartBtn.gameObject.SetActive(true);
                //    gameOverPanel.SetActive(false);
                //    Time.timeScale = 1;
                //    return;
                //}
                //gameObject.GetComponent<BGMController>().Game_Pause();              //일시정지 효과음 재생
                //continueBtn.gameObject.SetActive(true);
                //rebuildDeckBtn.gameObject.SetActive(true);
                //quitBtn.gameObject.SetActive(true);
                //goToMainBtn.gameObject.SetActive(false);
                //restartBtn.gameObject.SetActive(false);
                //continueBtn.transform.localPosition = continueBtnOriginPos;
                //rebuildDeckBtn.transform.localPosition = rebuildDeckBtnOriginPos;
                //quitBtn.transform.localPosition = quitBtnOriginPos;
                //resultText.text = "일시정지";
                //Time.timeScale = 0;
                //gameOverPanel.SetActive(true);
            }
        }
    }
}
