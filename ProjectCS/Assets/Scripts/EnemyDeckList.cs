using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeckList : MonoBehaviour
{
    public List<Cards> deckBuffer;
    private DeckManager deckManager;
    private FieldManager fieldManager;

    private void Awake()
    {
        deckManager = GameObject.Find("CardManager").GetComponent<DeckManager>();
        fieldManager = GameObject.Find("CardManager").GetComponent<FieldManager>();
        deckManager.enemyDeckBuffer.Clear();
        deckManager.enemyHandCard.Clear();
        deckManager.enemyHandBuffer.Clear();
        deckManager.enemyCardGrave.Clear();
        fieldManager.enemyFieldCard.Clear();
        deckManager.enemyDeckBuffer = deckBuffer;
    }

    public void InitEnemy()
    {
        Instantiate(gameObject);
    }

    public void DestroyEnemy()
    {
        Destroy(GameObject.FindWithTag("EnemyMain"));
    }
}
