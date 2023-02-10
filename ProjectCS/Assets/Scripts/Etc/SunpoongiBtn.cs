using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SunpoongiBtn : MonoBehaviour
{
    public GameObject btn_Stop, btn_Power01, btn_Power02, btn_Power03, btn_RotateNeck;
    public float reduceSpeed = 100f;
    [SerializeField]private DoRotate doRotate;


    private void OnMouseOver()
    {
        if(gameObject == btn_Power01)
        {
            Debug.Log("πÃ«≥");
        }
        else if (gameObject == btn_Power02)
        {
            Debug.Log("æ‡«≥");
        }
        else if (gameObject == btn_Power03)
        {
            Debug.Log("∞≠«≥");
        }
        else
        {
            Debug.Log("πˆ∆∞ ø¿πˆ");
        }
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (!EventSystem.current.IsPointerOverGameObject())
    //        {
    //            RaycastHit hit;
    //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //            Physics.Raycast(ray, out hit);

    //            if (hit.collider.name == "BT_02")
    //            {
    //                Debug.Log("πÃ«≥ º±≈√");
    //                doRotate.rotSpeed = 800f;
    //                btn_Power02.transform.DOLocalMove(new Vector3(0.01462345f, -0.001193928f, 0.0125f), 0.5f);
    //                btn_Power03.transform.DOLocalMove(new Vector3(0.004623454f, -0.001193928f, 0.0375f), 0.5f);
    //                btn_Power01.transform.DOLocalMove(new Vector3(0.0146233f, -0.006f, -0.01249981f), 0.5f);
    //            }
    //            else if (hit.collider.name == "BT_03")
    //            {
    //                Debug.Log("æ‡«≥ º±≈√");
    //                doRotate.rotSpeed = 1000f;
    //                btn_Power01.transform.DOLocalMove(new Vector3(0.0146233f, -0.001193926f, -0.01249981f), 0.5f);
    //                btn_Power03.transform.DOLocalMove(new Vector3(0.004623454f, -0.001193928f, 0.0375f), 0.5f);
    //                btn_Power02.transform.DOLocalMove(new Vector3(0.01462345f, -0.006f, 0.0125f), 0.5f);
    //            }
    //            else if (hit.collider.name == "BT_04")
    //            {
    //                Debug.Log("∞≠«≥ º±≈√");
    //                doRotate.rotSpeed = 1500f;
    //                btn_Power01.transform.DOLocalMove(new Vector3(0.0146233f, -0.001193926f, -0.01249981f), 0.5f);
    //                btn_Power02.transform.DOLocalMove(new Vector3(0.01462345f, -0.001193928f, 0.0125f), 0.5f);
    //                btn_Power03.transform.DOLocalMove(new Vector3(0.004623454f, -0.006f, 0.0375f), 0.5f);
    //            }
    //            else if (hit.collider.name == "BT_01")
    //            {
    //                Debug.Log("¡§¡ˆ º±≈√");
    //                //doRotate.rotSpeed = 0;
    //                btn_Power01.transform.DOLocalMove(new Vector3(0.0146233f, -0.001193926f, -0.01249981f), 0.5f);
    //                btn_Power02.transform.DOLocalMove(new Vector3(0.01462345f, -0.001193928f, 0.0125f), 0.5f);
    //                btn_Power03.transform.DOLocalMove(new Vector3(0.004623454f, -0.001193928f, 0.0375f), 0.5f);
    //                while (true)
    //                {
    //                    doRotate.rotSpeed -= reduceSpeed;
    //                    if (doRotate.rotSpeed <= 0)
    //                    {
    //                        doRotate.rotSpeed = 0;
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
