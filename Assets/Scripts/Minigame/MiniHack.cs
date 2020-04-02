using AudioConfig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MiniHack : MonoBehaviour
{
    [SerializeField]
    private int minNum = 20, maxNum = 100, firstNum, lastNum, currentNumChoice = 0;

    private int[] choicedNumber;
    private Button[] numberButton;
    private Text firstNumText, lastNumText;
    private Button[] choicedNumberButton;
    private Image duongNoi1, duongNoi2;

    [SerializeField]
    private UnityEvent eventAfterHack;

    private void OnEnable()
    {
        ComponentInit();
        NumberInit();
    }

    private void ComponentInit()
    {
        GameObject numberButtonParent = gameObject.transform.Find("NumberButton").gameObject;
        numberButton = numberButtonParent.GetComponentsInChildren<Button>();
        firstNumText = gameObject.transform.Find("FirstNum").GetComponentInChildren<Text>();
        lastNumText = gameObject.transform.Find("LastNum").GetComponentInChildren<Text>();
        choicedNumberButton = gameObject.transform.Find("NumChoice").GetComponentsInChildren<Button>();
        duongNoi1 = gameObject.transform.Find("FirstNum").transform.Find("DuongNoi").GetComponent<Image>();
        duongNoi2 = gameObject.transform.Find("LastNum").transform.Find("DuongNoi2").GetComponent<Image>();
        choicedNumber = new int[choicedNumberButton.Length];
        duongNoi1.fillAmount = 0;
        duongNoi2.fillAmount = 0;
    }

    private void NumberInit()
    {
        firstNum = Random.Range(minNum, maxNum / 2);
        lastNum = Random.Range(firstNum, maxNum);
        firstNumText.text = firstNum.ToString();
        lastNumText.text = lastNum.ToString();
        currentNumChoice = 0;
        RandomNumberInButton();
    }

    private void RandomNumberInButton()
    {
        //Random cac button so
        for (int i = 0; i < numberButton.Length; i++)
        {
            numberButton[i].GetComponentInChildren<Text>().text = Random.Range(0, lastNum / 2).ToString();
        }

        int t = firstNum;//bien t chua firstNum
        int[] dd = new int[numberButton.Length];//bien de danh dau button da dung
        //Random cac so dung
        for(int i = 0; i < choicedNumber.Length; i++)
        {
            if (i == choicedNumber.Length - 1)//Neu la so cuoi cung
            {
                choicedNumber[i] = lastNum - t;              
            }
            else
            {
                //Random cho den khi nho hon lastNum
                do
                {
                    choicedNumber[i] = Random.Range(0, ((firstNum + lastNum) / 2) / choicedNumber.Length);
                } while (lastNum < (t + choicedNumber[i]));
                t += choicedNumber[i];
            }
            int r;
            //kiem tra button nay da dung hay chua, neu chua thi moi gan gia tri so dung vao button
            do
            {
                r = Random.Range(0, numberButton.Length);
            } while (dd[r] == 1);
            numberButton[r].GetComponentInChildren<Text>().text = choicedNumber[i].ToString();
            dd[r] = 1;//danh dau button nay da dung
        }
        ResetChoiceNum();
    }

    public void ButtonNumClick(int numIndex)
    {
        int t = int.Parse(numberButton[numIndex].GetComponentInChildren<Text>().text);
        if (currentNumChoice < choicedNumber.Length)
        {
            choicedNumber[currentNumChoice] = t;
            choicedNumberButton[currentNumChoice].GetComponentInChildren<Text>().text = choicedNumber[currentNumChoice].ToString();
            currentNumChoice++;
        }
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        SoundManager.PlaySound(soundObj, Resources.Load<AudioClip>("Audio/SoundEffect/ObjectSound/cardSwipe"));
        Destroy(soundObj, 1f);
    }

    public void ResetChoiceNum()
    {
        currentNumChoice = 0;
        for (int i = 0; i < choicedNumberButton.Length; i++)
        {
            choicedNumberButton[i].GetComponentInChildren<Text>().text = "0";
            choicedNumber[i] = 0;
        }
        duongNoi1.fillAmount = 0;
        duongNoi2.fillAmount = 0;
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        SoundManager.PlaySound(soundObj, Resources.Load<AudioClip>("Audio/SoundEffect/ObjectSound/cardSwipe"));
        Destroy(soundObj, 1f);
    }

    public void ButtonSync()
    {
        StartCoroutine(SyncHacking());
        GameObject soundObj = Instantiate(Resources.Load<GameObject>("Prefabs/EmptySoundObject"), gameObject.transform.position, Quaternion.identity);
        SoundManager.PlaySound(soundObj, Resources.Load<AudioClip>("Audio/SoundEffect/ObjectSound/cardSwipe"));
        Destroy(soundObj, 1f);
    }

    IEnumerator SyncHacking()
    {
        GameObject buttonSync = gameObject.transform.Find("Button").Find("ButtonSync").gameObject;
        buttonSync.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            duongNoi1.fillAmount += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            duongNoi2.fillAmount += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        int tong = firstNum;
        for (int i = 0; i < choicedNumber.Length; i++)
        {
            tong += choicedNumber[i];
        }
        yield return new WaitForSeconds(0.5f);
        if (tong == lastNum)
        {
            eventAfterHack.Invoke();
        }
        else
        {
            ResetChoiceNum();
        }
        buttonSync.SetActive(true);
    }
}
