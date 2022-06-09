using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class AcceptButton : MonoBehaviour
{
    public void acceptRequest(){

        GameObject  child= this.transform.GetChild (1).gameObject;
        string requestId = child.GetComponent<Text>().text;
        StartCoroutine(Web.PostRequest(Web.acceptRequest(MainScene.token,requestId),"e",Web.REQUEST.ACCEPT_REQUEST));
    }
}
