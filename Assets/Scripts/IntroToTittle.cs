using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroToTittle : MonoBehaviour
{   
    void Start()
    {
        StartCoroutine(IntroToTittleScreen());
    }

    IEnumerator IntroToTittleScreen()
    {
        yield return new WaitForSeconds(4.3f);
        SceneManager.LoadScene(1);
    }
}
