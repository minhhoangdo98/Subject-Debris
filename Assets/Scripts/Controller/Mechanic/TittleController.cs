using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioConfig;

public class TittleController : MonoBehaviour
{
    EventSystem system;
    public GameObject fadeIn, blackScreen, tittleScreenWindow, loadingPanel, music, rainSound, buttonClickSound, windowRain;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("FirstTimeOpen") == 0)
        {
            BgmManager.SetBgmVolume(10);
            SoundManager.SetSoundVolume(10);
            QualitySettings.SetQualityLevel(2, true);
            PlayerPrefs.SetInt("Quality", 2);
            PlayerPrefs.SetInt("FirstTimeOpen", 1);
        }
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"), true);
        SoundManager.SetSoundVolumeToObject(rainSound);
        SoundManager.SetSoundVolumeToObject(buttonClickSound);
        BgmManager.SetBgmVolumeToObject(music);
        windowRain.SetActive(true);
        return;
    }

    void Start()
    {
        system = EventSystem.current;
        tittleScreenWindow.SetActive(false);
        blackScreen.SetActive(true);
        StartCoroutine(FadeInScreenblack());
    }

    private IEnumerator FadeInScreenblack()//lam man hinh sang len va hien title screen
    {
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fadeIn.SetActive(false);
        tittleScreenWindow.SetActive(true);
        yield break;
    }

    void Update()
    {
        //Tab key to next input field
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));  

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public void Thoat()
    {
        Application.Quit();
    }

    public void ButtonLogin()
    {
        StartGame();
    }

    public void ButtonStartGame()
    {
        PlayerPrefs.SetInt("story", 0);
        StartGame();
    }

    public void ButtonContinue()
    {
        StartGame();
    }

    private void StartGame()
    {
        ChangeScene(2);
    }

    public void ChangeScene(int sceneIndex)
    {
        StartCoroutine(ChangingScene(sceneIndex));
    }

    IEnumerator ChangingScene(int sceneIndex)
    {
        loadingPanel.SetActive(true);
        loadingPanel.GetComponent<Animation>().Play("LoadingStart");
        loadingPanel.GetComponentInChildren<Slider>().value = 0;
        yield return new WaitForSeconds(1f);
        if (PlayerPrefs.GetInt("story") == 0)
            SceneManager.LoadScene(3, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
}
