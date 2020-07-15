using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSide : MonoBehaviour
{
    enum SideName {BackSide, FrontSide, MiddleSide };
    [SerializeField]
    SideName side;
    private GameObject chara;

    private void Start()
    {
        chara = gameObject.transform.parent.transform.parent.gameObject;
    }

    #region OnTriggerStaySide
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (side)
        {
            case SideName.BackSide:
                BackSideStayAction(collision);
                break;
            case SideName.FrontSide:
                FrontStayAction(collision);
                break;
            case SideName.MiddleSide:
                MiddleStayAction(collision);
                break;
        }
    }

    private void BackSideStayAction(Collider2D collision)
    {
        if (chara.GetComponent<CharacterObject>().diChuyen && chara.GetComponent<CharacterObject>().canMove)
        {
            if (collision.CompareTag(chara.GetComponent<CharacterObject>().target1Tag))
            {
                chara.GetComponent<CharacterObject>().Flip();
            }
        }
        if (collision.CompareTag("Tuong") || collision.CompareTag("BlockEnemy") || collision.CompareTag("Dat"))
        {
            chara.GetComponent<BossController>().canRoll = false;
        }
    }

    private void FrontStayAction(Collider2D collision)
    {

    }

    private void MiddleStayAction(Collider2D collision)
    {
        if (collision.CompareTag(chara.GetComponent<CharacterObject>().target1Tag))
            chara.SendMessage("RollBackward");
    }
    #endregion

    #region OnTriggerExitSide
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (side)
        {
            case SideName.BackSide:
                BackSideExitAction(collision);
                break;
            case SideName.FrontSide:
                FrontExitAction(collision);
                break;
        }
    }

    private void BackSideExitAction(Collider2D collision)
    {
        if (collision.CompareTag("Tuong") || collision.CompareTag("BlockEnemy") || collision.CompareTag("Dat"))
        {
            chara.GetComponent<BossController>().canRoll = true;
        }
    }

    private void FrontExitAction(Collider2D collision)
    {
        
    }
    #endregion
}
