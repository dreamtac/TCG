using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private TurnController turnController;
    private DeckManager deckManager;
    private GameObject ingamePanel;
    private TMP_Text text_Time;
    private TMP_Text text_TurnCheck;
    private TMP_Text text_Info;

    private void Awake()
    {
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        deckManager = GameObject.Find("CardManager").GetComponent<DeckManager>();
        ingamePanel = GameObject.Find("InGamePanel");
        text_Time = GameObject.Find("Text_Time").GetComponent<TextMeshProUGUI>();
        text_TurnCheck = GameObject.Find("Text_TurnCheck").GetComponent<TextMeshProUGUI>();
        text_Info = GameObject.Find("Text_Info").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ingamePanel.SetActive(false);
    }

    public void TextTime(int time)
    {
        text_Time.text = "���� �ð� : " + time;
    }

    public void TurnChecker(bool myTurn)
    {
        if(myTurn == true)
        {
            text_TurnCheck.text = "<color=green>���� �� : �÷��̾�</color>";
        }
        else
        {
            text_TurnCheck.text = "<color=red>���� �� : ����</color>";
        }
        
    }

    public void GameInfo(bool myTurn)
    {
        if (myTurn)
        {
            text_Info.text = "<color=green>�� ī�� �� : ���� - </color>" +
                deckManager.myHandBuffer.Count + "<color=green> �� - </color>" + 
                deckManager.myDeckBuffer.Count;
                
        }

        else
        {
            text_Info.text = "<color=red>���� ī�� �� : ���� - </color>" +
                deckManager.enemyHandBuffer.Count + "<color=red> �� - </color>" + 
                deckManager.enemyDeckBuffer.Count;
        }
    }

    public void GameOver_ContinueBtn()
    {
        Time.timeScale = 1f;
    }

    public void GameOver_RestartBtn(bool restart = false)
    {
        //�ٽ� ����
        if (restart)
        {
            turnController.ResetGame(true);
        }
        else
        {
            turnController.ResetGame(false);   
        }
    }

    public void GameOver_QuitBtn()
    {
        //���� ����
        Application.Quit();
    }
}
