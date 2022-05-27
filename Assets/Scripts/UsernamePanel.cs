using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using System.Text;
using System.Linq;

public class UsernamePanel : MonoBehaviour
{

    public void SetChat(){

        GameObject _userid = this.transform.GetChild (5).gameObject;
        GameObject  child= this.transform.GetChild (4).gameObject;
        GameObject  chatName= this.transform.GetChild (1).gameObject;

        string chatID = child.GetComponent<Text>().text;
        string userId = _userid.GetComponent<Text>().text;
        string name = chatName.GetComponent<Text>().text; 

        ChatScene.choosenChatId=chatID;
        ChatScene.choosenUserId=userId;
        ChatScene.choosenChatName=name;
        ChatScene.reloadChat=true;

    }
   
}
