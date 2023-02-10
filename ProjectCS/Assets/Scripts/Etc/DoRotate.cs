using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoRotate : MonoBehaviour
{
    public float rotSpeed = 100f;
    public GameObject sunpoongiNeck;
    public Ease ease;
    /*public void StartRoatate()
    {
        StartCoroutine(RotateWing());
        //transform.DORotate(new Vector3(180, 0, 0), 1);
        //transform.DORotate(new Vector3(360, 0, 0), 1);
        
    }

    public void StopRotate()
    {
        StopCoroutine(RotateWing());
    }

    IEnumerator RotateWing()
    {
        while (true)
        {
            transform.DORotate(new Vector3(180, 0, 0), 1);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, 0), 1);
        }
    }*/

    public void Btn_RotateNeck()
    {
        //모가지 회전
        StartCoroutine(StartRotateNeck());
    }

    private void Update()
    {
        transform.Rotate(new Vector3(rotSpeed * Time.deltaTime, 0, 0));
    }

    IEnumerator StartRotateNeck()
    {
        while (true)
        {
            sunpoongiNeck.transform.DORotate(new Vector3(0, -45, 0), 4f).SetEase(ease);
            yield return new WaitForSeconds(4.5f);
            sunpoongiNeck.transform.DORotate(new Vector3(0, 45, 0), 4f).SetEase(ease);
            yield return new WaitForSeconds(4.5f);
            /*sunpoongiNeck.transform.Rotate(new Vector3(0, -neckSpeed * Time.deltaTime, 0));
            yield return new WaitForSeconds(1.5f);
            sunpoongiNeck.transform.Rotate(new Vector3(0, neckSpeed * Time.deltaTime, 0));
            yield return new WaitForSeconds(3f);*/
        }
    }
}
