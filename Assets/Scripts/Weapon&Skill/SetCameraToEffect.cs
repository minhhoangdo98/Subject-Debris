using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraToEffect : MonoBehaviour
{
    [SerializeField]
    private Canvas effectCanvas;

    private void Awake()
    {
        if (effectCanvas == null)
            effectCanvas = gameObject.transform.Find("Canvas").gameObject.GetComponent<Canvas>();
        effectCanvas.worldCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
    }
}
