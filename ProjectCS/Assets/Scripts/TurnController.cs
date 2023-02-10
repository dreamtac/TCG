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

    public bool testLoading = false;        //isLoading Ȯ�ο� (�׽�Ʈ)
    public bool enemyDone = false;          //��� �� ����
    static public bool isLoading;           //�ε� ��

    const int MAX_MANA = 10;
    private Button turnEndBtn;
    private Button enemySpawnBtn;
    private Button cardSelectBtn;
    private GameObject enemyObject;               //���� ���� �� ���� �Ǵ� ��
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
        {               //true ��� ��� �� ���� ���
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            fieldManager.CheckCharacterHP();
        }

        //�� ��, �ε����� �ƴ� �� End ��ư Ȱ��ȭ
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
    {                      //�� �ѱ�� ��ư ������ ��
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
                deckManager.InstantiateCard(my_Turn);   //�� ����, ��� ī�� ��ο�
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
                deckManager.InstantiateCard(my_Turn);   //�� ����, ���� ī�� ��ο�
                turnCount++;
                Debug.Log("---- Turn : " + turnCount + " ----");
                StartCoroutine(coroutine);
            }
        }
    }

    public void StartBtn()
    {
        //��ŸƮ ��ư ������ ���� ��ŸƮ
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
            //�� ī�� 30�� ä���� ���� ���� ����

        }
    }

    public void ResetGame(bool restart = false)
    {
        StopCoroutine(coroutine);
        //���� ���� ���� ���� �ʱ�ȭ
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
        //enemy ����Ʈ���� EnemyDeckList ��ũ��Ʈ ���� �ʱ�ȭ ��
        deckManager.myHandCard.Clear();
        deckManager.myHandBuffer.Clear();
        deckManager.playerCardGrave.Clear();
        deckManager.myDeckBuffer.Clear();
        fieldManager.myFieldCard.Clear();
        Time.timeScale = 1f;

        if (restart)
        {//�ʱ�ȭ�� ���� �� ���� �����
            StartCoroutine(StartGame());
        }
        else
        {//�ʱ�ȭ ���� �� ���θ޴��� ���ư���
            cameraMove.MoveCamera_MainMenu();
            mainmenu.SetActive(true);
        }
    }

    IEnumerator StartGame()
    {   //���� ��ŸƮ
        //�÷��̾�, ��� ��ġ Hp �ʱ�ȭ
        yield return new WaitForSeconds(0.5f);
        Debug.Log("���� ��ŸƮ");
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
            //������ ī�� ����Ʈ�� ������ ����
            deckManager.myDeckBuffer.Add(deckManager.mySelectDeck[i]);
        }
        GameObject.Find("PlayerHpText").GetComponent<TMP_Text>().text = playerHp.ToString();
        GameObject.Find("EnemyHpText").GetComponent<TMP_Text>().text = enemyHp.ToString();
        deckManager.ShuffleDeck();
        my_Turn = true;                             //���� ���۽� �÷��̾��� ���� ����
        playerMana++;
        ManaTextInit();
        turnCount = 1;
        Debug.Log("Game Start \n---- Turn : " + turnCount + " ----");

        for (int i = 0; i < 4; i++)
        {
            //���� �� ī�� 4�� ��ο�
            deckManager.InstantiateCard(my_Turn);
            deckManager.InstantiateCard(false);
        }
        yield return new WaitForSeconds(0.7f);
        deckManager.InstantiateCard(my_Turn);       //���� �� ī�� ��ο�
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
    {//second ��ŭ ���� �Ұ�
        //Debug.Log(second + "�� ���� �ε� ����");
        StartCoroutine(PauseGame(second));
    }

    IEnumerator PauseGame(float second)
    {
        isLoading = true;
        yield return new WaitForSeconds(second);
        isLoading = false;
        //Debug.Log("�ε� ����");
    }
}
