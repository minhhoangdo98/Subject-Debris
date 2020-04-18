using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FIMSpace.FLook;

public class AllyController : MonoBehaviour
{
    public CharacterObject charObj;

    void Start()
    {
        charObj = gameObject.GetComponent<CharacterObject>();
        charObj.SetValuesStart();
    }

    void Update()
    {
        charObj.SetAnimatiorAndValuesUpdate();
    }
}
