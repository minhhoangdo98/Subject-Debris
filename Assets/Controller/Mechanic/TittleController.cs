using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioConfig;

public class TittleController : MonoBehaviour
{
    EventSystem system;
    public GameObject fadeIn, blackScreen, titleScreen, loadingPanel, music, rainSound, buttonClickSound;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("FirstTimeOpen") == 0)
        {
            BgmManager.SetBgmVolume(10);
            SoundManager.SetSoundVolume(10);
            PlayerPrefs.SetInt("FirstTimeOpen", 1);
        }
        SoundManager.SetSoundVolumeToObject(rainSound);
        SoundManager.SetSoundVolumeToObject(buttonClickSound);
        BgmManager.SetBgmVolumeToObject(music);
    }

    void Start()
    {
        system = EventSystem.current;
        titleScreen.SetActive(false);
        blackScreen.SetActive(true);
        StartCoroutine(FadeInScreenblack());
        GameObject dialog = null;
        //Neu la nen tang android thi se can nguoi dung cho phep chia se vi tri
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            dialog = new GameObject();
        }
#endif
    }

    private IEnumerator FadeInScreenblack()//lam man hinh sang len va hien title screen
    {
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fadeIn.SetActive(false);
        titleScreen.SetActive(true);
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
