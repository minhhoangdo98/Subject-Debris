using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBall : MonoBehaviour
{
    [SerializeField]
    private Canvas canvasEffect;
    private DealDamageTrigger dDTrigger;
    public float timeToStart = 1, timeEffect = 1, speed = 1f, strikePosX = 0f, strikePosY = -2.5f;
    public bool moveable = false;
    public Transform targetMove;

    private void Start()
    {
        dDTrigger = gameObject.GetComponent<DealDamageTrigger>();
        StartCoroutine(ThunderStrike());
    }

    private void Update()
    {
        if (moveable)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetMove.position, step);
        }       
    }

    IEnumerator ThunderStrike()
    {
        GameObject mainCam = Camera.main.gameObject;
        yield return new WaitForSeconds(timeToStart);
        canvasEffect.gameObject.SetActive(true);
        GameObject thunder = Instantiate(Resources.Load<GameObject>("Prefabs/Effect/Thunder 2"), gameObject.transform.position, Quaternion.identity);
        SoundManager.SetSoundVolumeToObject(thunder);
        dDTrigger.CopyValueTo(thunder.GetComponent<DealDamageTrigger>());
        thunder.transform.localEulerAngles = gameObject.transform.localEulerAngles;
        thunder.transform.localPosition = new Vector3(thunder.transform.localPosition.x + strikePosX, thunder.transform.localPosition.y + strikePosY, thunder.transform.localPosition.z);
        iTween.ShakePosition(mainCam, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
        yield return new WaitForSeconds(0.05f);
        canvasEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeEffect);
        Destroy(thunder);
        Destroy(gameObject);
    }
}
