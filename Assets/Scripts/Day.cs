using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Day : MonoBehaviour
{
    public void setDayDetails(){
        GameObject  child= this.transform.GetChild (0).gameObject;
        string dayNumber = child.GetComponent<Text>().text;
        StatusHistoryScene.currentDay=Int32.Parse(dayNumber);
        //this.GetComponent<Image>().color= new Color(0.9f,0.0f,0.80f,0.392f);
        StatusHistoryScene.showDetails=true;
    }
}
