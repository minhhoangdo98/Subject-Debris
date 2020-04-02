using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCloseDoor : MonoBehaviour
{
    private DoorObject door;
    [SerializeField]
    private LockObject lockObj, lockObj2d;
    [SerializeField]
    private bool lockAfterClose = false;
    private bool delay = false;

    private void Start()
    {
        door = gameObject.transform.parent.GetComponent<DoorObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCollider") && door.open && !delay)
        {
            StartCoroutine(DelayClose());
        }       
    }

    IEnumerator DelayClose()
    {
        delay = true;
        door.DoorPerform();
        if (door.open)
            yield return new WaitForSeconds(door.openTime);
        else
            yield return new WaitForSeconds(door.closeTime);
        if (lockAfterClose)
        {
            lockObj.locked = true;
            if (lockObj2d != null)
                lockObj2d.locked = true;    
        }    
        delay = false;
    }
}
