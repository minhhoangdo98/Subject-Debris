using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    [SerializeField]
    private UnityEvent eventWhenEnter;
    [SerializeField]
    private bool disableAfterEvent, delay = false;
    [SerializeField]
    float delayTime = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player3d") && !delay)
        {
            StartCoroutine(InvokeAction());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider") && !delay)
        {
            StartCoroutine(InvokeAction());
        }
    }

    IEnumerator InvokeAction()
    {
        delay = true;
        eventWhenEnter.Invoke();
        gameObject.SetActive(!disableAfterEvent);
        yield return new WaitForSeconds(delayTime);
        delay = false;
    }
}
