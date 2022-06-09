using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DiscardButton : MonoBehaviour
{
    public void discardRequest(){

        GameObject  child= this.transform.GetChild (1).gameObject;
        string requestId = child.GetComponent<Text>().text;
        StartCoroutine(Web.PostRequest(Web.discardRequest(MainScene.token,requestId),"e",Web.REQUEST.DISCARD_REQUEST));
    }  
}
