using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System.Text;
using UnityEngine.Networking;

public class VacationsScene : MonoBehaviour
{

    public GameObject requestsContainer;
    public GameObject requestPanel;
    public GameObject workerRequestPanel;
    public GameObject text;
    public GameObject requestIdText;
    public GameObject requestData;

    public GameObject newRequestPanel;
    public GameObject error;


    public Dropdown start_day;
    public Dropdown start_month;
    public Dropdown start_year;

    public Dropdown end_day;
    public Dropdown end_month;
    public Dropdown end_year;

    public Button workersRequestsButton;
    public Button acceptButton;
    public Button discardButton;

    public GameObject acceptPanel;
    public GameObject discardPanel;

    Vector2 initialRequestContainerSize;

    [Serializable]
    public class Request
    {
        public int type;
        public string startDate;
        public string endDate;
    }

    public static bool newRequest;
    public static Web.REQUEST _requestType;
    public static string requestMessage;

    private int requestType;
    private bool myRequests;

    private int startYear;
    private int startMonth;
    private int startDay;

    private int endDay;
    private int endMonth;
    private int endYear;

    void Start()
    {
        newRequestPanel.gameObject.SetActive(false);
        error.gameObject.SetActive(false);  
        requestType=1; //default   
        myRequests=true; //my requests
        // MainScene.token="00ca68f8f5874f8a9740ff7184cd8f59";
        // MainScene.userRole=1;
        // MainScene.isAdmin=true;
        initialRequestContainerSize=new Vector2(requestsContainer.GetComponent<RectTransform>().rect.width,requestsContainer.GetComponent<RectTransform>().rect.height);
        getUserRequests();     
    }

    void Update()
    {
        if(newRequest){
            newRequest=false;
            ProcessNewRequest(_requestType,requestMessage);
        }

        if(MainScene.userRole==2 || MainScene.userRole==1 || MainScene.isAdmin){
            workersRequestsButton.gameObject.SetActive(true);
        }else{
            workersRequestsButton.gameObject.SetActive(false);
        }         
    }

    public static void setRequestInfo(Web.REQUEST type, string rawRespone){
        newRequest=true;
        requestMessage=rawRespone;
        _requestType=type;
    }

    private void ProcessNewRequest(Web.REQUEST type, string rawRespone){

        switch(type){
            case Web.REQUEST.GET_USER_REQUESTS:
                ProcessRequestsResponse(rawRespone);
                break;
            case Web.REQUEST.CREATE_REQUEST:
                ProcessRequestResponse(rawRespone);
                break;
            case Web.REQUEST.GET_REQUESTS_FOR_APPROVE:
                ProcessRequestsToApproveResponse(rawRespone);
                break;
            case Web.REQUEST.ACCEPT_REQUEST:
                 ProcessAcceptRequestResponse(rawRespone);
                 break;
            case Web.REQUEST.DISCARD_REQUEST:
                ProcessDiscardRequestResponse(rawRespone);
                break;
            default:
                Debug.LogError("Wrong request type!");
                break; 
        }

    }

    private void ProcessAcceptRequestResponse(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        StartCoroutine(Web.GetRequest(Web.GET_REQUESTS_FOR_APPROVE+MainScene.token,Web.REQUEST.GET_REQUESTS_FOR_APPROVE));     
    }

    private void ProcessDiscardRequestResponse(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        StartCoroutine(Web.GetRequest(Web.GET_REQUESTS_FOR_APPROVE+MainScene.token,Web.REQUEST.GET_REQUESTS_FOR_APPROVE));
    }

    private void ProcessRequestsToApproveResponse(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        Debug.Log("REQUESTS TO APPROVE");
        Debug.Log(data);
        showWorkersRequests(data);

    }

    private void ProcessRequestResponse(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        getUserRequests();
    }

    private void ProcessRequestsResponse(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        showUserRequests(data);
    }

    private void getUserRequests(){
        StartCoroutine(Web.GetRequest(Web.VACATION+MainScene.token,Web.REQUEST.GET_USER_REQUESTS));
    }
    
    private void showUserRequests(JSONNode data){
        clearUserRequests();
        int requestCounter=0;
        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){
            requestCounter++;
            GameObject panel = Instantiate(requestPanel, new Vector3(0,0,0), Quaternion.identity);
            GameObject text1 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);
            GameObject text2 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);
            GameObject text3 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);
            GameObject text4 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);

            text1.GetComponent<Text>().text =entry.Value["id"]+". "+vacationTypeToString(entry.Value["type"]); //7. Quarantine
            text2.GetComponent<Text>().text ="Start: "+Date.dateTimeToString(entry.Value["startDate"]); //Start: 2022.05.19   End: 2022.05.19
            text3.GetComponent<Text>().text ="End: "+ Date.dateTimeToString(entry.Value["endDate"]); //Start: 2022.05.19   End: 2022.05.19
            text4.GetComponent<Text>().text ="Status: "+ statusToString(entry.Value["state"]); //Status: pending

            text1.transform.SetParent(panel.transform,false);
            text2.transform.SetParent(panel.transform,false);
            text3.transform.SetParent(panel.transform,false);
            text4.transform.SetParent(panel.transform,false);

            panel.transform.SetParent(requestsContainer.transform,false);
        }

        if(requestCounter>3){
            requestCounter-=3;
            float width = requestsContainer.GetComponent<RectTransform>().rect.width;
            float height = requestsContainer.GetComponent<RectTransform>().rect.height;
            requestsContainer.GetComponent<RectTransform>().sizeDelta=new Vector2(width, height+(requestCounter*200)+(requestCounter*12));
            requestData.GetComponentInChildren<Scrollbar>().value=1;
        }
    }

    public void getWorkersRequests(){

        if(myRequests){
            //button text = My requests
            myRequests=false;
            workersRequestsButton.GetComponentInChildren<Text>().text ="My requests";
            StartCoroutine(Web.GetRequest(Web.GET_REQUESTS_FOR_APPROVE+MainScene.token,Web.REQUEST.GET_REQUESTS_FOR_APPROVE));
        }
        else{
            myRequests=true;
            //button text = Workers requests
            workersRequestsButton.GetComponentInChildren<Text>().text ="Workers requests";
            StartCoroutine(Web.GetRequest(Web.VACATION+MainScene.token,Web.REQUEST.GET_USER_REQUESTS));
        }
    }

    private void showWorkersRequests(JSONNode data){

        clearUserRequests();
        int requestCounter=0;
        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){

            if(entry.Value["state"]==1){
                requestCounter++;
                GameObject panel = Instantiate(workerRequestPanel, new Vector3(0,0,0), Quaternion.identity);
                GameObject text1 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);
                GameObject text2 = Instantiate(text, new Vector3(0,0,0), Quaternion.identity);

                GameObject requestId = Instantiate(requestIdText, new Vector3(0,0,0), Quaternion.identity);
                GameObject _requestId = Instantiate(requestIdText, new Vector3(0,0,0), Quaternion.identity);

                GameObject accept = Instantiate(acceptPanel, new Vector3(0,0,0), Quaternion.identity);
                GameObject discard = Instantiate(discardPanel, new Vector3(0,0,0), Quaternion.identity);


                text1.GetComponent<Text>().text =entry.Value["id"]+". "+vacationTypeToString(entry.Value["type"]) +" ("+entry.Value["firstName"]+" "+entry.Value["lastName"]+")";
                text2.GetComponent<Text>().text ="Start: "+Date.dateTimeToString(entry.Value["startDate"]) +" - "+"End: "+ Date.dateTimeToString(entry.Value["endDate"]); //Start: 2022.05.19   End: 2022.05.19
                requestId.GetComponent<Text>().text = entry.Value["id"];
                _requestId.GetComponent<Text>().text = entry.Value["id"];

                text1.transform.SetParent(panel.transform,false);
                text2.transform.SetParent(panel.transform,false);

                requestId.transform.SetParent(accept.transform,false);
                _requestId.transform.SetParent(discard.transform,false);

                accept.transform.SetParent(panel.transform,false);
                discard.transform.SetParent(panel.transform,false);

                panel.transform.SetParent(requestsContainer.transform,false);
            }
        }

        if(requestCounter>3){
            requestCounter-=3;
            float width = requestsContainer.GetComponent<RectTransform>().rect.width;
            float height = requestsContainer.GetComponent<RectTransform>().rect.height;
            requestsContainer.GetComponent<RectTransform>().sizeDelta=new Vector2(width, height+(requestCounter*200)+(requestCounter*12));
            requestData.GetComponentInChildren<Scrollbar>().value=1;
        }
    }

    private static string vacationTypeToString(int type){

        switch(type){
            case 1:
                return "Quarantine";
            case 2:
                return "Sick leave";
            case 3:
                return "Unpaid leave";
            case 4:
                return "Parential leave";
            case 5:
                return "Maternity leave";
            case 6:
                return "Vacation leave";
            case 7:
                return "Occasional leave";
            case 8:
                return "Vacation on demand leave";
            case 9:
                return "Blood donation leave";
            default:
                return "Null";
        }
    }

    private static string statusToString(int status){

        switch(status){
            case 1:
                return "Pending";
            case 2:
                return "Accepted";
            case 3:
                return "Rejected";
            default:
                return "Null";
        }
    }

    private void setYears(int year, Dropdown dropdown){
        List<string> years = new List<string>();

        for(int i=0; i<5; i++){
            years.Add(year.ToString());
            year++;
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(years);
    }

    private void setMonths(int month, Dropdown dropdown){
        List<string> months = new List<string>();

        while(month<=12){
            months.Add(month.ToString());
            month++;
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(months);
    }

    private void setDays(int day, int daysInMonth,Dropdown dropdown){
        List<string> days = new List<string>();

        while(day<=daysInMonth){
            days.Add(day.ToString());
            day++;
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(days);
    }

    public void showRequestPanel(){
        newRequestPanel.gameObject.SetActive(true);

        System.DateTime localDate =  System.DateTime.Now;

        startYear=endYear=localDate.Year; 
        startMonth=endMonth=localDate.Month;
        startDay=endDay=localDate.Day;
 
        setYears(startYear,start_year);
        setMonths(startMonth,start_month);
        setDays(startDay,System.DateTime.DaysInMonth(startYear, startMonth),start_day);

        setYears(startYear, end_year);
        setMonths(startMonth, end_month);
        setDays(startDay,System.DateTime.DaysInMonth(startYear, startMonth),end_day);

    }

    private void clearUserRequests(){
        for(int i=requestsContainer.transform.childCount-1; i>=0; i--){
            DestroyImmediate(requestsContainer.transform.GetChild(i).gameObject);
        }
        requestsContainer.GetComponent<RectTransform>().sizeDelta=initialRequestContainerSize;
    }

    public void submitRequest(){
        DateTime sDate = new System.DateTime(startYear, startMonth, startDay);
        DateTime eDate = new System.DateTime(endYear, endMonth, endDay);

        if(sDate>eDate){
            error.gameObject.SetActive(true);
        }
        else{
            error.gameObject.SetActive(false);
            Request request = new Request();
            request.type=requestType;
            request.startDate=sDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            request.endDate=eDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

            StartCoroutine(Web.PutRequest(Web.CREATE_REQUEST+MainScene.token, JsonUtility.ToJson(request), Web.REQUEST.CREATE_REQUEST));
        }
    }

    public void hideRequestPanel(){
        newRequestPanel.gameObject.SetActive(false);
    }

    public void setRequestType(int type){
        requestType=type+1;
    }

    public void SetStartYear(int value){

        startYear=Int32.Parse(start_year.options[start_year.value].text);

        if(startYear!=System.DateTime.Now.Year){
            startMonth=1;
            startDay=1;
        }else{
            startMonth=System.DateTime.Now.Month;
            startDay=System.DateTime.Now.Day;
        }
        setMonths(startMonth,start_month);
        setDays(startDay,System.DateTime.DaysInMonth(startYear, startMonth),start_day);   
    }

    public void SetEndYear(int value){
        endYear=Int32.Parse(end_year.options[end_year.value].text);

        if(endYear!=System.DateTime.Now.Year){
            endMonth=1;
            endDay=1;
        }else{
            endMonth=System.DateTime.Now.Month;
            endDay=System.DateTime.Now.Day;
        }
        setMonths(endMonth,end_month);
        setDays(endDay,System.DateTime.DaysInMonth(endYear, endMonth),end_day);
    }

    public void setStartMonth(int value){

        startMonth = Int32.Parse(start_month.options[start_month.value].text);

        if(startMonth!=System.DateTime.Now.Month){
            startDay=1;
        }else{
            startDay=System.DateTime.Now.Day;
        }
        setDays(startDay,System.DateTime.DaysInMonth(startYear, startMonth),start_day);
    }

     public void setEndMonth(int value){

        endMonth = Int32.Parse(end_month.options[end_month.value].text);

        if(endMonth!=System.DateTime.Now.Month){
            endDay=1;
        }
        else{
            endDay=System.DateTime.Now.Day;
        }
        setDays(endDay,System.DateTime.DaysInMonth(endYear, endMonth),end_day);
    }

    public void setStartDay(int value){
        startDay= Int32.Parse(start_day.options[start_day.value].text);
    }

    public void setEndDay(int value){
        endDay= Int32.Parse(end_day.options[end_day.value].text);
    }

    public void Back(){

        SceneManager.LoadScene(1);

    }
}
