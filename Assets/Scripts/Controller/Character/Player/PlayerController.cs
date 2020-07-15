using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterObject))]
public class PlayerController : MonoBehaviour
{
    public bool dieuKhien = true, canChangeView = false, canOpenBag = true;
    public CharacterObject charObj;
    public ItemManager bag;
    private Image playerImage, hpBar, energyBar;

    void Start()
    {
        charObj = gameObject.GetComponent<CharacterObject>();
        charObj.SetValuesStart();
        //Khoi tao cac bien ma chi co o player
        PlayerValueInit();
    }

    void Update()
    {
        charObj.SetAnimatiorAndValuesUpdate();
        CapNhatBar();
    }

    private void PlayerValueInit()
    {
        charObj.isPlayer = true;
        hpBar = GameObject.FindGameObjectWithTag("HpBar").GetComponent<Image>();
        energyBar = GameObject.FindGameObjectWithTag("EnergyBar").GetComponent<Image>();
        playerImage = GameObject.FindGameObjectWithTag("PlayerImage").GetComponent<Image>();
    }

    private void CapNhatBar()
    {
        hpBar.fillAmount = charObj.charStat.hp / charObj.charStat.maxHp;
        energyBar.fillAmount = charObj.charStat.energy / charObj.charStat.maxEnergy;
    }

    private void Defeated()
    {
        StartCoroutine(ChangeToGameover());
    }

    IEnumerator ChangeToGameover()
    {
        yield return new WaitForSeconds(2f);
        charObj.gc.eve.hoiThoaiPanel.SetActive(false);
        StartCoroutine(charObj.gc.FadeOutScreenBlack());
        PlayerPrefs.SetInt("LastScene", SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Gameover", LoadSceneMode.Single);
    }
}
