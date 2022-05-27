using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;
using System.Text;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;


public class LoginScene : MonoBehaviour
{

    [Serializable]   
    public class User
    {
        public string email;
        public string password; 

    }
       
    public static string email {get; set;}
    public static string password {get; set;}
   
    
    public void OnClick(){

        string HashedPass= ComputeSha256Hash(password);
        User user = new User();
        user.email=email;
        user.password=password;
        Debug.Log(user);
        Debug.Log(JsonUtility.ToJson(user));
        StartCoroutine(Web.PostRequest(Web.LOGIN_ADDRESS, JsonUtility.ToJson(user),Web.REQUEST.LOGIN)); 
       
    }

    public void Email(string s){
        email=s;      
    }

    public void Password(string s){
        password=s;
    }

    static string ComputeSha256Hash(string rawData)  
    {  
               
        using (SHA256 sha256Hash = SHA256.Create())  
        {              
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
            
            StringBuilder builder = new StringBuilder();  
            for (int i = 0; i < bytes.Length; i++)  
            {  
                builder.Append(bytes[i].ToString("x2"));  
            }  
            return builder.ToString();  
         }  
    } 

    public static void ProcessLoginResponse(string rawRespone){

        JSONNode node = SimpleJSON.JSON.Parse(rawRespone);     
        MainScene.email=email;
        MainScene.token=node["result"];
        SceneManager.LoadScene(1);
    }


    
}
