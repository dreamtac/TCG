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

    Card selectCard;    //������ �����ϴ� ī��
    Card targetCard;    //������ ���ϴ� ī��

    public bool magicSelect;
    private GameObject gameOverPanel;
    public TMP_Text resultText;

    [SerializeField] bool onAttack;     //MouseUp �� true �� �� ���� ����
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
                        {//Ŭ���� ī�尡 ��� ī���̰�, �ʵ����� ��ȯ�� ���¶��
                            Debug.Log(hit.transform.gameObject);
                            magicTarget = hit.transform.GetComponent<Card>();

                            for (int i = 0; i < enemyFieldCard.Count; i++)
                            {//��� �ʵ� �� ī��� ��Ŀ�� ����
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
                            Debug.Log("�ʵ� �� ī�带 ������ �ּ���");
                        }
                    }
                }
            }
        }
    }

    public void FieldMouseDown(Card card)
    {
        if (turnController.my_Turn)
        {//�� ���� ��
            Debug.Log("�ʵ� ::: ���콺 �ٿ�");
            selectCard = card;
        }
    }

    public void FieldMouseUp(Card card)
    {
        if (turnController.my_Turn)
        {//�� ���� ��
            if (onAttack)
            {//��� �ʵ��� ī�� ����
                AttackTarget(selectCard, targetCard); //���� ����
                Debug.Log("���� ���� ::: \n SelectCard = " + selectCard.cardName +
                    "\n TargetCard = " + targetCard.cardName);
            }
            else if (onEnemyAttack)
            {//��� ��ġ ����
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
        {//�� ���� ��
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
                    Debug.Log("Ÿ�� ī�� Ȯ�� ::: " + targetCard.cardName);
                    onAttack = true;
                    break;
                }
                if (hit.collider.tag.Equals("Enemy"))
                {
                    Debug.Log("��� ��ġ Ȯ�� ::: " + hit.transform.name);
                    existTarget = true;
                    onEnemyAttack = true;
                }
            }
            if (!existTarget)
            {
                targetCard = null;
                onAttack = false;
                onEnemyAttack = false;
                Debug.Log("Ÿ�� ī��, ��ġ ����");
            }            
        }
    }

    public void MagicCardSummon(Card card, bool isMine)
    {
        //���� ī�� ��ȯ
        if (isMine)
        {//���� �� ����ī����
            //Type == 1 ------------------------------------------------------------
            if (card.magicType == 1)
            {//�� �ʵ��� ī�� ��ü ����
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
                StartCoroutine(MagicCardActive(card, isMine));    //���� ī��� �ʵ����� ��ȯ x ȿ���� �ߵ��ϰ� �ٷ� ������ �̵�
            }
            //Type == 2 ------------------------------------------------------------
            else if (card.magicType == 2)
            {//��� �ʵ� �� ī�� ���� ����
                 StartCoroutine(MagicCardActive(card, isMine, 2));
            }
            //Type == 3 ------------------------------------------------------------
            else if (card.magicType == 3)
            {//ī�� ��ο�
                StartCoroutine(MagicCardActive(card, isMine, 3));    //���� ī��� �ʵ����� ��ȯ x ȿ���� �ߵ��ϰ� �ٷ� ������ �̵�
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
        {//��� �ʵ� �� ī�� �ణ Ȯ��, �÷��̾ ������ ī�� ����
            //�߰� �� �� - ��� �ʵ� ���� ī�� ������ ���? Ȥ�� �׳� ī�� ������?
            magicSelect = true;
            magicCard = card;

            for (int i = 0; i < enemyFieldCard.Count; i++)
            {//��� �ʵ� �� ī��� �������� ���� (��Ŀ��)
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
    {//���� ���� select => ���� ���� ī��/ target => ���� �޴� ī��
        select.MoveTransform(select.originPRS, false);
        turnController.Loading(0.85f);
        Sequence sequence = DOTween.Sequence()
            .Append(select.transform.DOMove(target.originPRS.pos, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {
                select.gameObject.GetComponent<BGMController>().Card_Attack();          //ī�� ���� ȿ����
                select.health -= target.attack;
                select.Damaged(target.attack);
                target.health -= select.attack;
                target.Damaged(select.attack);
                cameraMove.ShakeCamera();

                select.attackable = false;
                select.InitStatus();    //��� ī�� ����� �ʱ�ȭ
                target.InitStatus();    //���� ī�� ����� �ʱ�ȭ

                if (select.health <= 0)
                {//hp�� 0���� �������� �ش� ī�� Dead ���·�
                    select.isDead = true;
                }
                if (target.health <= 0)
                {//hp�� 0���� �������� �ش� ī�� Dead ���·�
                    target.isDead = true;
                }
            })
            .Append(select.transform.DOMove(select.originPRS.pos, 0.4f)).SetEase(Ease.OutSine);
        Invoke("CheckCardHp", 0.85f);
    }

    public void CardAttackableInit(bool myTurn)
    {//myTurn == true -> �� �� ���� , false -> ��� �� ����
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
    {//���� ī�� ��ȯ �� �ʵ� �� ��ȯ ��ġ Ž��
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
    {// ��ġ ���� ::: target = true -> ���� ���� ����, false -> ������ ���� ����
        if (target)
        {//���� ���� ��ġ ����
            card.MoveTransform(card.originPRS, false);
            turnController.Loading(0.81f);
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(enemyMainCharaceter.gameObject.transform.position, 0.4f))
                .SetEase(Ease.InSine).AppendCallback(() =>
                {
                    card.gameObject.GetComponent<BGMController>().Character_Attack();       //ĳ���� ���� ȿ����
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
        {//������ �� ��ġ ����
            card.MoveTransform(card.originPRS, false);
            turnController.Loading(0.81f);
            Sequence sequence = DOTween.Sequence()
                .Append(card.transform.DOMove(playerMainCharaceter.gameObject.transform.position, 0.4f))
                .SetEase(Ease.InSine).AppendCallback(() =>
                {
                    card.gameObject.GetComponent<BGMController>().Character_Attack();       //ĳ���� ���� ȿ����
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
        {//�� ���� ī�尡 ���� ��
            Debug.Log("���� ī�� ���� ������ 3");
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
        {//���� ���� ī�尡 ���� ��
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
    {//ī�� Hpüũ �� �������� �̵�
        for (int i = 0; i < myFieldCard.Count; i++)
        {
            if (myFieldCard[i].GetComponent<Card>().isDead)
            {// �� �ʵ� i��° ī�尡 Dead ���¶��
                deckManager.MoveToGrave(myFieldCard[i].GetComponent<Card>(), true);
                myFieldCard.RemoveAt(i);
                i--;    //i��° ī�尡 ����Ʈ���� ������ List.Count���� ���̰� �� �׷��� i�� -1 ����
            }
        }

        for (int i = 0; i < enemyFieldCard.Count; i++)
        {
            if (enemyFieldCard[i].GetComponent<Card>().isDead)
            {// ���� �ʵ� i��° ī�尡 Dead ���¶��
                deckManager.MoveToGrave(enemyFieldCard[i].GetComponent<Card>(), false);
                enemyFieldCard.RemoveAt(i);
                i--;    //i��° ī�尡 ����Ʈ���� ������ List.Count���� ���̰� �� �׷��� i�� -1 ����
            }
        }
    }

    public void CheckCharacterHP()
    {
        //��ġ ü�� üũ (���� ����)
        if(turnController.playerHp <= 0)
        {
            //�絵��, ������, ��������
            gameObject.GetComponent<BGMController>().Game_Defeat();             //�й� ȿ���� ���
            gameOverPanel.SetActive(true);
            resultText.text = "�й�";
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
            //���θ޴�, �絵��, ��������
            gameObject.GetComponent<BGMController>().Game_Victory();             //�¸� ȿ���� ���
            gameOverPanel.SetActive(true);
            resultText.text = "�¸�";
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
                //����簳, �絵��, ��������
                if (gameOverPanel.activeSelf == true && resultText.text == "�Ͻ�����")
                {
                    gameOverPanel.SetActive(false);
                    ingamePanel.SetActive(true);
                    Time.timeScale = 1;
                    return;
                }
                gameObject.GetComponent<BGMController>().Game_Pause();              //�Ͻ����� ȿ���� ���
                resultText.text = "�Ͻ�����";
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
                ////����簳, ������, ��������
                //if (gameOverPanel.activeSelf == true && resultText.text == "�Ͻ�����")
                //{
                //    quitBtn.gameObject.transform.localPosition = quitBtnOriginPos;
                //    restartBtn.gameObject.SetActive(true);
                //    gameOverPanel.SetActive(false);
                //    Time.timeScale = 1;
                //    return;
                //}
                //gameObject.GetComponent<BGMController>().Game_Pause();              //�Ͻ����� ȿ���� ���
                //continueBtn.gameObject.SetActive(true);
                //rebuildDeckBtn.gameObject.SetActive(true);
                //quitBtn.gameObject.SetActive(true);
                //goToMainBtn.gameObject.SetActive(false);
                //restartBtn.gameObject.SetActive(false);
                //continueBtn.transform.localPosition = continueBtnOriginPos;
                //rebuildDeckBtn.transform.localPosition = rebuildDeckBtnOriginPos;
                //quitBtn.transform.localPosition = quitBtnOriginPos;
                //resultText.text = "�Ͻ�����";
                //Time.timeScale = 0;
                //gameOverPanel.SetActive(true);
            }
        }
    }
}
