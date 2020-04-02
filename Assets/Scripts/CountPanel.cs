using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountPanel : MonoBehaviour
{
    [SerializeField]
    private float waitTime = 3, countTime = 3, waitToDisable = 0.5f;
    [SerializeField]
    private Text countText;
    [SerializeField]
    private string startText = "Mission Start!";
    [SerializeField]
    UnityEvent eventWhenEnable, eventWhenCountEnd;

    private void OnEnable()
    {
        eventWhenEnable.Invoke();
        StartCoroutine(CountToStart());
    }

    IEnumerator CountToStart()
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < countTime; i++)
        {
            countText.text = ((int)countTime - i).ToString();
            yield return new WaitForSeconds(0.5f);
        }
        countText.text = startText;
        yield return new WaitForSeconds(waitToDisable);
        eventWhenCountEnd.Invoke();
        gameObject.SetActive(false);
    }
}
