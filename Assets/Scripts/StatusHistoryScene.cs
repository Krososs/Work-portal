using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;
using System.Text;
using UnityEngine.SceneManagement;

public class StatusHistoryScene : MonoBehaviour
{

    public GameObject callendar;
    public GameObject day;
    public GameObject week;

    public GameObject exportPanel;

    public Button nextMotnButton; 
    public Button previousMonthButton; 
    public Button submitExportButton; 

    public GameObject monthName;
    public GameObject dayNumber;

    public GameObject DayPanel;
    public GameObject DayDetailsPanel;
    public GameObject dateText;
    public GameObject statussesPanel;
    public GameObject statussesContainer;
    public GameObject statusDetailsText;

    Vector2 initialStatussContainerSize;

    private int currentMonth;
    public static int currentDay;
    private int currentYear;

    private int exportYear;
    private int exportMonth;

    public static bool showDetails;

    public static bool newRequest;
    public static Web.REQUEST requestType;
    public static string requestMessage;

    private class Status
    {
        public int month { get; set; }
        public int day{ get; set; }
        public int hour{ get; set; }
        public int minute{ get; set; }
        public int type{ get; set; }
    }

    List<Status> statuses = new List<Status>();
   
    void Start()
    {   
        exportPanel.gameObject.SetActive(false);
        showDetails=false;
        initialStatussContainerSize =new Vector2(statussesContainer.GetComponent<RectTransform>().rect.width,statussesContainer.GetComponent<RectTransform>().rect.height);
        getCurrentDate();
        exportMonth=1;
        exportYear=currentYear;
        getStatusHistory();             
    }

    void Update()
    { 
        if(showDetails){
            showDetails=false;
            showDayDetails();
        }
        if(newRequest){
            newRequest=false;
            ProcessNewRequest(requestType,requestMessage);
        }    
    }

    public static void setRequestInfo(Web.REQUEST type, string rawRespone){
        newRequest=true;
        requestMessage=rawRespone;
        requestType=type;
    }

    private void ProcessNewRequest(Web.REQUEST type, string rawRespone){

        switch(type){
            case Web.REQUEST.GET_STATUS_HISTORY:
                ProcessStatusHistory(rawRespone);
                break;
            case Web.REQUEST.EXPORT:
                ProcessExport(rawRespone);
                break;
            default:
                Debug.LogError("Wrong request type!");
                break; 
        }

    }

    private void getStatusHistory(){
        StartCoroutine(Web.GetRequest(Web.GET_STATUS_HISTORY+MainScene.token, Web.REQUEST.GET_STATUS_HISTORY));
    }
 
    private void ProcessExport(string rawRespone){

        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        string path =@"";
        path+= Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        path+=@"/export.xls";
        File.WriteAllBytes(path, Convert.FromBase64String(data["result"]));

    }

    private void ProcessStatusHistory(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);

        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){
            System.DateTime date = System.DateTime.Parse(entry.Value["timestamp"],System.Globalization.CultureInfo.GetCultureInfo("en-us"));
            statuses.Add(new Status(){month=date.Month, day=date.Day, hour=date.Hour, minute=date.Minute, type=entry.Value["type"]});       
        }
        loadMonth(currentYear, currentMonth);
    }

    private void getCurrentDate(){
        System.DateTime localDate =  System.DateTime.Now;
        currentDay=localDate.Day;
        currentMonth=localDate.Month;
        currentYear=localDate.Year;
    }

    private bool dayHasStatus(int _day){
        foreach(Status s in statuses)
            if(s.month == currentMonth && s.day ==_day) return true;         
        return false;
    }

    private void showDayDetails(){

        clearStatussesPanel();
        DayPanel.GetComponent<Image>().color= new Color(1.0f,1.0f,1.00f,0.392f);
        DayDetailsPanel.GetComponent<Image>().color= new Color(0.7568628f,0.7568628f,0.7568628f,1.0f);
        dateText.GetComponent<Text>().text=currentDay+"."+currentMonth+"."+currentYear;
        int statusCounter=0;
        foreach(Status s in statuses){

            if(s.month == currentMonth && s.day ==currentDay){
                statusCounter++;
                GameObject details = Instantiate(statusDetailsText, new Vector3(0,0,0), Quaternion.identity);
                details.GetComponent<Text>().text=getStatusDetails(s);
                details.transform.SetParent(statussesContainer.transform,false);

            } 
        }

        if(statusCounter>6){
            statussesPanel.GetComponentInChildren<Scrollbar>().value=100;
            statusCounter-=6;
            float width = statussesContainer.GetComponent<RectTransform>().rect.width;
            float height = statussesContainer.GetComponent<RectTransform>().rect.height;
            statussesContainer.GetComponent<RectTransform>().sizeDelta=new Vector2(width, height+(statusCounter*100));   
        }
    }

    private void addBlankDay(GameObject w){
        GameObject d = Instantiate(day, new Vector3(0,0,0), Quaternion.identity);
        d.transform.SetParent(w.transform,false);
        d.GetComponent<Image>().color= new Color(0.847f,0.839f,0.839f,0.41f);
    }

    private void addAvailableDay(GameObject w, int _day){
        GameObject d = Instantiate(day, new Vector3(0,0,0), Quaternion.identity);

        GameObject number = Instantiate(dayNumber, new Vector3(0,0,0), Quaternion.identity);
        number.GetComponent<Text>().text =_day.ToString();
        number.transform.SetParent(d.transform,false);

        d.transform.SetParent(w.transform,false);

        if(dayHasStatus(_day)){
            d.GetComponent<Image>().color= new Color(0.2156f,0.788f,0.0745f,1.0f);
        }
    }

    private void clearCallendar(){

        for(int i=callendar.transform.childCount-1; i>=0; i--){
            DestroyImmediate(callendar.transform.GetChild(i).gameObject);
        }
    }
    
    private void clearStatussesPanel(){
        for(int i=statussesContainer.transform.childCount-1; i>=0; i--){
            DestroyImmediate(statussesContainer.transform.GetChild(i).gameObject);
        }
        statussesContainer.GetComponent<RectTransform>().sizeDelta=initialStatussContainerSize;
    }

    private string getStatusDetails(Status s){

        string status = s.hour.ToString()+":"+s.minute.ToString()+" - ";
        switch(s.type){
            case 1:
                status+="Start of work";
                break;
            case 2:
                status+="Break";
                break;
            case 3:
                status+="End of work";
                break;
            default:
                status+="Undefined status";
                break;
        }
        return status;
    }

    private void loadMonth(int year, int month){

        clearCallendar();

        monthName.GetComponent<Text>().text=Date.getMonth(month);
        int dayCounter=0;
        System.DateTime date = new System.DateTime(year, month, 1);
        int firstDayInMonth= Date.getDayInWeek(date.DayOfWeek.ToString());
        int daysInMonth=System.DateTime.DaysInMonth(year, month);
        
        bool printEnabled=false;

        for(int i=0; i<6; i++){
            GameObject w = Instantiate(week, new Vector3(0,0,0), Quaternion.identity);
            w.transform.SetParent(callendar.transform,false);

            for(int j=0; j<7; j++){

                
                if(j==firstDayInMonth-1){ 
                    printEnabled=true;
                    firstDayInMonth=-1;
                }

                if(printEnabled){
                    dayCounter++;               
                    addAvailableDay(w, dayCounter);                 
                }
                else addBlankDay(w);
                       
                if(dayCounter==daysInMonth) printEnabled=false;
                    
            }
        }

    }

    public void showExportPanel(){
        exportPanel.gameObject.SetActive(true);
    }

    public void closeExportPanel(){
        exportPanel.gameObject.SetActive(false);
    }

    public void setSelectedYear(int val){
        exportYear=currentYear-val;
    }

    public void setSelectedMonth(int val){
        exportMonth= val+1;   
    }

    public void Submit(){
        StartCoroutine(Web.GetRequest(Web.export(MainScene.token, exportMonth, exportYear,null), Web.REQUEST.EXPORT));            
    }

    public void loadPreviousMonth(){
        currentMonth-=1;
        nextMotnButton.interactable=true;
        if(currentMonth-1<1) previousMonthButton.interactable=false;
        loadMonth(currentYear, currentMonth);  
    }

    public void loadNextMonth(){
        currentMonth+=1;
        previousMonthButton.interactable=true;
        if(currentMonth+1>12) nextMotnButton.interactable=false;
        loadMonth(currentYear, currentMonth);
    }

    public void Back(){
        SceneManager.LoadScene(1);
    }
}
