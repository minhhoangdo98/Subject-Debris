using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject colider;
    [SerializeField]
    private float hp;
    [SerializeField]
    private bool invisible = false, object3d = false;
    [SerializeField]
    private UnityEvent eventWhenDestroy;

    private void Start()
    {
        if (object3d)
            colider = GameObject.Find("Collider");
        else
            colider = gameObject;
    }

    #region NhanSatThuong
    public void TakeDamage(float damageAmount)//Nhan sat thuong
    {
        if (!invisible && hp > 0)
        {
            hp -= damageAmount;
        }
        if (hp <= 0)
        {
            if (object3d)
                colider.GetComponent<Collider>().enabled = false;
            else
                colider.GetComponent<Collider2D>().enabled = false;
            gameObject.tag = "Death";
            eventWhenDestroy.Invoke();
        }
    }
    #endregion
}
