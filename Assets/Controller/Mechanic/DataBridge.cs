using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using Firebase.Unity.Editor;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class DataBridge : MonoBehaviour
{
    private PlayerDataOnline data;
    private string DATA_URL = "https://subject-debris.firebaseio.com/";
    private DatabaseReference databaseReference;
    private bool enableLoad = false;
    private IDictionary dictUser;

    private void Start()//Khoi tao firebase
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Luu du lieu vao firebase
    public void SaveData(int strenght, int vita, int intlligent, int eGem, string tenNV, int storyNum, string emails)
    {
        //data de luu
        data = new PlayerDataOnline(strenght, vita, intlligent, eGem, tenNV, storyNum, emails);

        string jsonData = JsonUtility.ToJson(data);
        //bo dau "." trong email thi moi luu duoc
        string s = "";
        for (int i = 0; i < emails.Length; i++)
        {
            if (emails[i] != '.')
                s = s + emails[i];

        }
        databaseReference.Child(s).SetRawJsonValueAsync(jsonData);
    }

    public void LoadData(string email)//Load du lieu theo email
    {
        FirebaseDatabase.DefaultInstance.GetReferenceFromUrl(DATA_URL).GetValueAsync()
            .ContinueWith((task =>
            {
                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    return;
                }

                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    return;
                }

                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.HasChild(email))
                    {
                        dictUser = (IDictionary)snapshot.Child(email).Value;//xac dinh email can load
                        enableLoad = true;//Bat dau load
                        return;
                    }
                }
            }));
    }

    private void Update()
    {
        //kiem tra de bat su kien load
        if (enableLoad)
        {
            GetData();
            enableLoad = false;
        }
    }

    public void GetData()//load data tu firebase ve cuc bo
    {
        PlayerPrefs.SetInt("str", int.Parse(dictUser["str"].ToString()));
        PlayerPrefs.SetInt("intl", int.Parse(dictUser["intl"].ToString()));
        PlayerPrefs.SetInt("vit", int.Parse(dictUser["vit"].ToString()));
        PlayerPrefs.SetInt("EGem", int.Parse(dictUser["gem"].ToString()));
        PlayerPrefs.SetString("tenCurrent", dictUser["ten"].ToString());
        PlayerPrefs.SetInt("story", int.Parse(dictUser["story"].ToString()));
        gameObject.GetComponent<TittleController>().ButtonLogin();
    }

    public void PushData()//Dua data tu cuc bo len firebase
    {
        if (gameObject.GetComponent<GameController>().connection)
        {
            //khoi tao cac bien chua data tam
            int strength = PlayerPrefs.GetInt("str");
            int vita = PlayerPrefs.GetInt("vit");
            int intlligent = PlayerPrefs.GetInt("intl");
            int eGem = PlayerPrefs.GetInt("EGem");
            string ten = PlayerPrefs.GetString("tenCurrent");
            int story = PlayerPrefs.GetInt("story");
            string email = PlayerPrefs.GetString("emailCurrent");
            //Luu du lieu len firebase thong qua cac bien tren
            SaveData(strength, vita, intlligent, eGem, ten, story, email);
            gameObject.GetComponent<GameController>().HienThongBao("Save successful");
        }
        else
        {
            gameObject.GetComponent<GameController>().HienThongBao("No internet!");
        }
    }
}
