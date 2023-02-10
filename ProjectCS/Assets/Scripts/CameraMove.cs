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
        //메인 메뉴 화면 카메라 이동
        gameObject.transform.DOMove(menuCameraPosition, 2f);
        gameObject.transform.DORotate(menuCameraRotation, 2f);
    }

    public void MoveCamera_InGame()
    {
        //인게임 화면 카메라 이동
        gameObject.transform.DOMove(gameCameraPosition, 2f);
        gameObject.transform.DORotate(gameCameraRotation, 2f);
    }

    public void ShakeCamera()
    {
        //카메라 흔들기
        Camera.main.DOShakePosition(shakeDuration, shakeStrength);
    }
}
