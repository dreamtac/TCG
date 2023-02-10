using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    private UIManager uIManager;
    private IEnumerator coroutine;
    private DeckManager deckManager;
    private CameraMove cameraMove;
    private FieldManager fieldManager;

    public GameObject playerManaTMP;
    public GameObject enemyManaTMP;

    public int playerHp;
    public int enemyHp;
    public int turnCount;
    public int playerMana = 0;
    public int enemyMana = 0;
    public int playerCurMana = 0;
    public int enemyCurMana = 0;
    public int time = 30;

    public bool my_Turn = false;
    public bool enemy_Turn = false;

    public bool testLoading = false;        //isLoading 확인용 (테스트)
    public bool enemyDone = false;          //상대 턴 종료
    static public bool isLoading;           //로딩 중

    const int MAX_MANA = 10;
    private Button turnEndBtn;
    private Button enemySpawnBtn;
    private Button cardSelectBtn;
    private GameObject enemyObject;               //게임 시작 시 생성 되는 적
    private GameObject mainmenu;
    private GameObject ingamePanel;

    private void Awake()
    {
        deckManager = GameObject.Find("CardManager").GetComponent<DeckManager>();
        uIManager = GameObject.Find("EventSystem").GetComponent<UIManager>();
        fieldManager = GameObject.Find("CardManager").GetComponent<FieldManager>();
        coroutine = CountDown();
        playerManaTMP = GameObject.Find("PlayerMana");
        enemyManaTMP = GameObject.Find("EnemyMana");
        turnEndBtn = GameObject.Find("Btn_EndTurn").GetComponent<Button>();
        enemySpawnBtn = GameObject.Find("Btn_EnemySelect").GetComponent<Button>();
        cardSelectBtn = GameObject.Find("Btn_CardSelect").GetComponent<Button>();
        cameraMove = GameObject.Find("Main Camera").GetComponent<CameraMove>();
        mainmenu = GameObject.Find("MainMenuPanel");
        ingamePanel = GameObject.Find("InGamePanel");
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {        
        if (enemyDone)
        {               //true 라면 상대 턴 종료 희망
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            fieldManager.CheckCharacterHP();
        }

        //내 턴, 로딩중이 아닐 때 End 버튼 활성화
        if(my_Turn && !isLoading)
            turnEndBtn.interactable = true;
        else
            turnEndBtn.interactable = false;
    }

    IEnumerator CountDown()
    {
        while(time != 0)
        {
            uIManager.TurnChecker(my_Turn);
            yield return new WaitForSeconds(1f);
            time--;
            uIManager.TextTime(time);
        }
        if (time == 0)
        {
            EndTurn();
        }
    }

    public void EndTurn()
    {                      //턴 넘기기 버튼 눌렀을 때
        if (!isLoading)
        {
            StopCoroutine(coroutine);
            if (my_Turn == true)
            {
                my_Turn = false;
                fieldManager.CardAttackableInit(true);
                enemy_Turn = true;

                if (enemyMana < MAX_MANA)
                    enemyMana++;
                else
                    enemyMana = MAX_MANA;

                ManaTextInit();
                time = 30;
                coroutine = CountDown();
                deckManager.InstantiateCard(my_Turn);   //턴 종료, 상대 카드 드로우
                deckManager.EnemyTurnStart();
                StartCoroutine(coroutine);
            }
            else if (enemy_Turn == true)
            {
                enemyDone = false;
                enemy_Turn = false;
                fieldManager.CardAttackableInit(false);
                my_Turn = true;

                if (playerMana < MAX_MANA)
                    playerMana++;
                else
                    playerMana = MAX_MANA;

                ManaTextInit();
                time = 30;
                coroutine = CountDown();
                deckManager.InstantiateCard(my_Turn);   //턴 종료, 마이 카드 드로우
                turnCount++;
                Debug.Log("---- Turn : " + turnCount + " ----");
                StartCoroutine(coroutine);
            }
        }
    }

    public void StartBtn()
    {
        //스타트 버튼 누르면 게임 스타트
        if (deckManager.mySelectDeck.Count == 30)
        {
            enemyObject = Resources.Load<GameObject>("Enemy/Enemy_01");
            Instantiate(enemyObject);
            //enemySpawnBtn.gameObject.SetActive(false);
            //cardSelectBtn.gameObject.SetActive(false);
            cameraMove.MoveCamera_InGame();
            mainmenu.SetActive(false);
            StartCoroutine(StartGame());
        }
        else
        {
            //덱 카드 30장 채워야 게임 시작 가능

        }
    }

    public void ResetGame(bool restart = false)
    {
        StopCoroutine(coroutine);
        //현재 진행 중인 게임 초기화
        for (int i = 0; i < deckManager.myHandCard.Count; i++)
        {
            Destroy(deckManager.myHandCard[i].gameObject);
        }
        for (int i = 0; i < deckManager.playerCardGrave.Count; i++)
        {
            Destroy(deckManager.playerCardGrave[i]);
        }
        for (int i = 0; i < deckManager.enemyHandCard.Count; i++)
        {
            Destroy(deckManager.enemyHandCard[i].gameObject);
        }
        for (int i = 0; i < deckManager.enemyCardGrave.Count; i++)
        {
            Destroy(deckManager.enemyCardGrave[i]);
        }
        for (int i = 0; i< fieldManager.myFieldCard.Count; i++)
        {
            Destroy(fieldManager.myFieldCard[i]);
        }
        for (int i = 0; i < fieldManager.enemyFieldCard.Count; i++)
        {
            Destroy(fieldManager.enemyFieldCard[i]);
        }
        //enemy 리스트들은 EnemyDeckList 스크립트 에서 초기화 함
        deckManager.myHandCard.Clear();
        deckManager.myHandBuffer.Clear();
        deckManager.playerCardGrave.Clear();
        deckManager.myDeckBuffer.Clear();
        fieldManager.myFieldCard.Clear();
        Time.timeScale = 1f;

        if (restart)
        {//초기화가 끝난 후 게임 재시작
            StartCoroutine(StartGame());
        }
        else
        {//초기화 끝난 후 메인메뉴로 돌아가기
            cameraMove.MoveCamera_MainMenu();
            mainmenu.SetActive(true);
        }
    }

    IEnumerator StartGame()
    {   //게임 스타트
        //플레이어, 상대 명치 Hp 초기화
        yield return new WaitForSeconds(0.5f);
        Debug.Log("게임 스타트");
        ingamePanel.SetActive(true);
        playerHp = 30;
        enemyHp = 30;
        time = 30;
        playerMana = 0;
        playerCurMana = 0;
        enemyMana = 0;
        enemyCurMana = 0;
        turnCount = 0;
        enemyDone = false;
        enemy_Turn = false;
        for(int i = 0; i < deckManager.mySelectDeck.Count; i++)
        {
            //선택한 카드 리스트들 덱으로 복사
            deckManager.myDeckBuffer.Add(deckManager.mySelectDeck[i]);
        }
        GameObject.Find("PlayerHpText").GetComponent<TMP_Text>().text = playerHp.ToString();
        GameObject.Find("EnemyHpText").GetComponent<TMP_Text>().text = enemyHp.ToString();
        deckManager.ShuffleDeck();
        my_Turn = true;                             //게임 시작시 플레이어의 턴이 먼저
        playerMana++;
        ManaTextInit();
        turnCount = 1;
        Debug.Log("Game Start \n---- Turn : " + turnCount + " ----");

        for (int i = 0; i < 4; i++)
        {
            //시작 시 카드 4장 드로우
            deckManager.InstantiateCard(my_Turn);
            deckManager.InstantiateCard(false);
        }
        yield return new WaitForSeconds(0.7f);
        deckManager.InstantiateCard(my_Turn);       //시작 시 카드 드로우
        coroutine = CountDown();
        StartCoroutine(coroutine);
    }

    public void ManaTextInit()
    {
        if (my_Turn)
        {
            playerCurMana = playerMana;
            playerManaTMP.GetComponent<TextMeshPro>().text = playerCurMana + " / " + playerMana;
        }
        else
        {
            enemyCurMana = enemyMana;
            enemyManaTMP.GetComponent<TextMeshPro>().text = enemyCurMana + " / " + enemyMana;
        }
    }

    public void CurrentManaChange()
    {
        if (my_Turn)
        {
            playerManaTMP.GetComponent<TextMeshPro>().text = playerCurMana + " / " + playerMana;
        }
        else
        {
            enemyManaTMP.GetComponent<TextMeshPro>().text = enemyCurMana + " / " + enemyMana;
        }
    }

    public void Loading(float second)
    {//second 만큼 조작 불가
        //Debug.Log(second + "초 동안 로딩 시작");
        StartCoroutine(PauseGame(second));
    }

    IEnumerator PauseGame(float second)
    {
        isLoading = true;
        yield return new WaitForSeconds(second);
        isLoading = false;
        //Debug.Log("로딩 종료");
    }
}
