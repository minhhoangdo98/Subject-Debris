using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaiKhoanController : MonoBehaviour
{
    private int demTK = 0;
    public InputField tenNV, tenDNT, passT, confirmPassT, tenDNL, passL;
    public GameObject MBPanel;
    private bool loginSucess = false, sendMess = false, regisComplete = false, verify = false;
    private string lat, lon;
    private string message = "";

    private void Start()
    {

    }

    //Chuc nang dang ky
    public void DangKy()
    {
        if (tenDNT.text.Equals("") || passT.text.Equals(""))//Neu nguoi choi khong nhap gi thi hien thong bao
        {
            message = "Please enter email and password to register";
            sendMess = true;
            return;
        }

        if (passT.text != confirmPassT.text)//Neu nhap lai password khong giong voi password tren
        {
            message = "Password not match!";
            sendMess = true;
            return;
        }

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(tenDNT.text,
            passT.text).ContinueWith((task =>
            {
                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    AuthError err = (AuthError)e.ErrorCode;
                    message = err.ToString();
                    sendMess = true;
                    return;
                }

                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    AuthError err = (AuthError)e.ErrorCode;
                    message = err.ToString();
                    sendMess = true;
                    return;
                }

                if (task.IsCompleted)
                {
                    //trigger de chay chuc nang verify email
                    verify = true;
                }
            }));
    }

    //Gui link xac nhan vao email nguoi tao tai khoan moi
    private void SendEmailVerify()
    {
        //Lay ra nguoi dung hien tai
        Firebase.Auth.FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    message = "Send Email Verification Async was canceled.";
                    sendMess = true;
                    return;
                }
                if (task.IsFaulted)
                {
                    message = "Send Email Verification Async encountered an error: " + task.Exception;
                    sendMess = true;
                    return;
                }

                //neu khong gap loi nao thi hien thong bao thanh cong
                message = "Successful register, please verify email and login!";
                regisComplete = true;//Thuc hien chuc nang tao tai khoan moi len firebase
                sendMess = true;
            });
        }
    }

    //Chuc nang dang nhap
    public void DangNhap()
    {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(tenDNL.text,
            passL.text).ContinueWith((task => {

                if (task.IsCanceled)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    AuthError err = (AuthError)e.ErrorCode;
                    message = err.ToString();
                    sendMess = true;
                    loginSucess = false;
                    return;
                }

                if (task.IsFaulted)
                {
                    Firebase.FirebaseException e =
                    task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;
                    AuthError err = (AuthError)e.ErrorCode;
                    message = err.ToString();
                    sendMess = true;
                    loginSucess = false;
                    return;
                }

                if (task.IsCompleted)
                {
                    //Trigger de chay thao tac dang nhap
                    loginSucess = true;
                }

            }));
    }

    private void Update()
    {
        //Phat hien cac trigger de chay ham tuong ung
        if (loginSucess)
        {
            //Luu lai email dang dung hien tai
            PlayerPrefs.SetString("emailCurrent", tenDNL.text);
            //Loai bo dau "." trong email de load tu firebase database
            string s = "";
            for (int i = 0; i < tenDNL.text.Length; i++)
            {
                if (tenDNL.text[i] != '.')
                    s = s + tenDNL.text[i];
            }
            //Load du lieu tu database cua firebase
            gameObject.GetComponent<DataBridge>().LoadData(s);
            loginSucess = false;
        }
        //Trigger Gui thong bao
        if (sendMess)
        {
            GetMessage(message);
            sendMess = false;
        }

        if (verify)//Xac thuc email
        {
            SendEmailVerify();
            verify = false;
        }

        if (regisComplete)//Hoan thanh dang ki, luu du lieu len firebase
        {
            gameObject.GetComponent<DataBridge>().SaveData(0, 0, 0, 0, tenNV.text, 0, tenDNT.text);
            regisComplete = false;
        }
    }

    //Ham hien thong bao
    void GetMessage(string mess)
    {
        MBPanel.SetActive(true);
        MBPanel.GetComponent<MessBox>().textThongBao.text = mess;
    }

    //Chuc nang reset password khi nguoi choi bam vao quen mat khau, nhap email sau do an vao nut gui email
    public void ResetPasword(InputField email)
    {
        string emailAddress = email.text;//Lay email ma nguoi choi nhap

        if (emailAddress == "")
        {
            message = "Please input your email!";
            sendMess = true;
            return;
        }

        FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
            if (task.IsCanceled)
            {
                message = "Send Password Reset Email Async was canceled.";
                sendMess = true;
                return;
            }
            if (task.IsFaulted)
            {
                message = "Send Password Reset Email Async encountered an error: " + task.Exception;
                sendMess = true;
                return;
            }

            message = "Password reset email sent successfully.";
            sendMess = true;
        });

    }
}
