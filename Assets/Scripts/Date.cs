using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Date : MonoBehaviour
{


    public static string dateTimeToString(string date){
        DateTime Date = System.DateTime.Parse(date,System.Globalization.CultureInfo.GetCultureInfo("en-us"));
        Debug.Log("SUper date");
        Debug.Log(date);
        Debug.Log(Date.Month);
        Debug.Log(Date.Year);
        Debug.Log(Date.Day);

        string d=Date.Year+"-";
        d+= Date.Month < 10 ? "0"+Date.Month.ToString() : Date.Month.ToString();
        d+="-"+Date.Day;

        return d;
    }

    public static string getMonth(int month){

        switch(month){
            case 1:
                return "January";
            case 2:
                return "February";
            case 3:
                return "March";
            case 4:
                return "April";
            case 5:
                return "May";
            case 6:
                return "June";
            case 7:
                return "July";
            case 8:
                return "August";
            case 9:
                return "September";
            case 10:
                return "October";
            case 11:
                return "November";
            case 12:
                return "December";
            default:
                return "NULL";
        }
    }

    public static int getDayInWeek(string day){
         switch(day){

            case "Monday":
                return 1;
            case "Tuesday":
                return 2;
            case "Wednesday":
                return 3;
            case "Thursday":
                return 4;
            case "Friday":
                return 5;
            case "Saturday":
                return 6;
            case "Sunday":
                return 7;
            default:
                return -1;
        }
    }

    
 
}
