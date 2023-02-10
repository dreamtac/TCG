using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    private Vector3 gameCameraPosition = new Vector3(1.5f, 24.05f, 0);
    private Vector3 gameCameraRotation = new Vector3(90, 0, 0);
    private Vector3 menuCameraPosition = new Vector3(27.5f, 18.5f, -33.5f);
    private Vector3 menuCameraRotation = new Vector3(8.5f, -60, 0);

    public float shakeDuration;
    public float shakeStrength;

    public void MoveCamera_MainMenu()
    {
        //���� �޴� ȭ�� ī�޶� �̵�
        gameObject.transform.DOMove(menuCameraPosition, 2f);
        gameObject.transform.DORotate(menuCameraRotation, 2f);
    }

    public void MoveCamera_InGame()
    {
        //�ΰ��� ȭ�� ī�޶� �̵�
        gameObject.transform.DOMove(gameCameraPosition, 2f);
        gameObject.transform.DORotate(gameCameraRotation, 2f);
    }

    public void ShakeCamera()
    {
        //ī�޶� ����
        Camera.main.DOShakePosition(shakeDuration, shakeStrength);
    }
}
