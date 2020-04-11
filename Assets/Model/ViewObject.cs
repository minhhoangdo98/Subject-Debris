using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewObject : MonoBehaviour
{
    public GameObject camera2D, fpsPlayer, player2d, touchControll2DPanel, touchControll3DPanel, currentActiveCamera;
    public GameObject[] eventCamera;

    public void ChangeView()
    {
        StartCoroutine(PerformChangeView());
    }

    private IEnumerator PerformChangeView()
    {
        if (player2d.activeInHierarchy)//kiem tra dang o goc nhin nao, neu = true thi la goc nhin 2d
        {
            if (!player2d.GetComponent<CharacterObject>().gc.touchButton.button3DView.GetComponent<Button>().interactable)
                yield break;
            fpsPlayer.transform.position = new Vector3(player2d.transform.position.x, player2d.transform.position.y, player2d.transform.position.z);//lay vi tri 2d chuyen sang 3d 
            fpsPlayer.transform.eulerAngles = new Vector3(fpsPlayer.transform.eulerAngles.x, 90f * player2d.GetComponent<CharacterObject>().faceRight, fpsPlayer.transform.eulerAngles.z);
        }
        else//nguoc lai la 3d
        {
            player2d.transform.position = new Vector3(fpsPlayer.transform.position.x, player2d.transform.position.y, player2d.transform.position.z);//lay thong tin vi tri 3d chuyen sang 2d khong thay doi truc z va y
        }
        yield return new WaitForSeconds(0.1f);
        camera2D.SetActive(!camera2D.activeInHierarchy);
        fpsPlayer.SetActive(!fpsPlayer.activeInHierarchy);
        player2d.SetActive(!player2d.activeInHierarchy);
        touchControll2DPanel.SetActive(!touchControll2DPanel.activeInHierarchy);
        touchControll3DPanel.SetActive(!touchControll3DPanel.activeInHierarchy);
        if (player2d.activeInHierarchy)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().CapNhatColliderIgnore();
            player2d.GetComponent<CharacterObject>().enableFootSound = false;
#if UNITY_STANDALONE || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
#endif
        }
#if UNITY_STANDALONE || UNITY_EDITOR
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }       
#endif
        fpsPlayer.GetComponent<FirstPersonController>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        fpsPlayer.GetComponent<FirstPersonController>().enabled = true;
        currentActiveCamera = Camera.main.gameObject;
    }
}
