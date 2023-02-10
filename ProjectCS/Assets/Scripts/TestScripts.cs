using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestScripts : MonoBehaviour
{
    public TextMeshProUGUI testText;
    public void TestButton()
    {
        testText.text = "Button 클릭 됨";
        StartCoroutine(TestText());
        testText.text = "함수 종료";
    }

    IEnumerator TestText()
    {
        yield return new WaitForSeconds(2f);
        testText.text = "TestText 코루틴 진입";
    }
}
