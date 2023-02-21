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

    private void Start()
    {
        SetResolution();
    }

    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }

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
