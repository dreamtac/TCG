using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestScripts : MonoBehaviour
{
    public TextMeshProUGUI testText;
    public void TestButton()
    {
        testText.text = "Button Ŭ�� ��";
        StartCoroutine(TestText());
        testText.text = "�Լ� ����";
    }

    IEnumerator TestText()
    {
        yield return new WaitForSeconds(2f);
        testText.text = "TestText �ڷ�ƾ ����";
    }
}
