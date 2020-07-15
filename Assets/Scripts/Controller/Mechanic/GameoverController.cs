using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverController : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonHome, loadingPanel;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Story") <= 1)
            buttonHome.SetActive(false);
    }

    public void ButtonTryAgain()
    {
        StartCoroutine(TryAgainAction());
    }

    public void ButtonHome()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void ButtonTittle()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    IEnumerator TryAgainAction()
    {
        loadingPanel.SetActive(true);
        loadingPanel.GetComponent<Animation>().Play("LoadingStart");
        loadingPanel.GetComponentInChildren<Slider>().value = 0;
        yield return new WaitForSeconds(1f);
        int lastScene = PlayerPrefs.GetInt("LastScene");
        PlayerPrefs.SetInt("TryAgain", 1);
        SceneManager.LoadScene(lastScene, LoadSceneMode.Single);
    }
}
