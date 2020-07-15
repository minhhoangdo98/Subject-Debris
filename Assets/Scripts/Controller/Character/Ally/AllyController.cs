using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FIMSpace.FLook;
using UnityEngine.UI;

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

    private void ChangeText(Text actionText)
    {
        gameObject.transform.Find("TalkObject").SendMessage("ChangeText", actionText.GetComponentInChildren<Text>(), SendMessageOptions.DontRequireReceiver);
    }

    private void ActionPerform()
    {
        gameObject.transform.Find("TalkObject").SendMessage("ActionPerform", SendMessageOptions.DontRequireReceiver);
    }

    private void FixedUpdate()
    {
        //Npc di chuyen den vi tri la positionMove
        if(charObj.movePos && charObj.postionMove!=null && charObj.diChuyen && charObj.canMove)
        {
            //kiem tra diem den la o ben trai hay phai, quay ve huong diem den
            if (charObj.objOnTheRight != charObj.faceRight)
                charObj.Flip();
            charObj.DiChuyenNhanVat(charObj.faceRight);//di chuyen ve phia truoc
            if (Vector2.Distance(transform.position, charObj.postionMove.transform.position) <= 0.5f)//neu den noi thi dung lai
            {
                charObj.movePos = false;
                charObj.r2.velocity = Vector2.zero;
            }
        }
    }
}
