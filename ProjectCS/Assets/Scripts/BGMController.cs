using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    [System.Serializable]
    public struct BgmType
    {
        public string bgmName;
        public AudioClip audio;
    }
    public BgmType[] BGMList;

    private AudioSource BGM;
    private string NowBGMName = "";

    public bool isMainBgm;

    void Start()
    {
        BGM = gameObject.AddComponent<AudioSource>();
        BGM.loop = false;
        if (isMainBgm)
        {
            if (BGMList.Length > 0)
                PlayBGM(BGMList[0].bgmName);
            BGM.volume = 0.1f;
            StartCoroutine(BGMChanger());
        }
        else
            BGM.volume = 0.4f;
    }

    IEnumerator BGMChanger()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (!BGM.isPlaying)
            {
                PlayBGM(NowBGMName);
            }
        }
    }

    public void PlayBGM(string name)
    {
        Debug.Log("PlayBGM 함수 실행");
        
        for(int i = 0; i < BGMList.Length; i++)
        {
            if (BGMList[i].bgmName.Equals(name))
            {
                if(i == 2)
                {
                    BGM.clip = BGMList[0].audio;
                    BGM.Play();
                    NowBGMName = BGMList[0].bgmName;
                    break;
                }
                BGM.clip = BGMList[i+1].audio;
                BGM.Play();
                NowBGMName = BGMList[i + 1].bgmName;
            }
        }
    }

    public void Card_Attack()
    {
        SearchSound("CardAttack");
    }

    public void Character_Attack()
    {
        SearchSound("CharacterAttack");
    }

    public void Game_Pause()
    {
        SearchSound("Pause");
    }

    public void Game_Victory()
    {
        SearchSound("Victory");
    }

    public void Game_Defeat()
    {
        SearchSound("Defeat");
    }

    public void ButtonClick()
    {
        SearchSound("Button");
    }

    private void SearchSound(string bgmName)
    {
        for (int i = 0; i < BGMList.Length; i++)
        {
            if (BGMList[i].bgmName == "" + bgmName + "")
            {
                BGM.clip = BGMList[i].audio;
                BGM.Play();
                break;
            }
        }
    }
}
