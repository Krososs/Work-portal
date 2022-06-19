using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;

public class MainScene : MonoBehaviour
{
    
    public Text username;
    public GameObject Slider;
    public GameObject workersButton;

    public Text stats1;
    public Text stats2;
    public Text stats3;


    public static string email;
    public static string password;
    public static string token;
    public static string userId;
    public static int userRole;
    public static int companyId;
    public static int departamentId;
    public static bool isAdmin;
    private static int currentStatus;
    private static JSONNode statistics;
    private static bool statisticsReady=false;
    private static bool updateStatistics=false;

    public Button workButton;
    public Button breakButton;
    public Button quitButton;

   
    void Start(){
        Debug.Log(token);
        username.text=email;  
        getUserInfo();
        getLastStatus();
    }

    void Update(){
        if(currentStatus==3){
            quitButton.interactable=false;
        }
        else{
            quitButton.interactable=true;
        }
        if(currentStatus==3 || currentStatus==2)
            breakButton.interactable=false;
        else
            breakButton.interactable=true;

        if(currentStatus==1)
            workButton.interactable=false;
        else
            workButton.interactable=true;

        if(userRole==3 || userRole == 2){
            workersButton.gameObject.SetActive(false);
        }

        if(statisticsReady){
            statisticsReady=false;
            setStatistics();
        }    

        if(updateStatistics){
            updateStatistics=false;
            StartCoroutine(Web.GetRequest(Web.getStatistics(token,"2022","6"),Web.REQUEST.GET_STATISTICS));
            
        }   
    }

    private void setStatistics(){
         
        double h = statistics["result"]["sumOfWorkTime"];
        double min = statistics["result"]["sumOfWorkTime"] %1.0;

        stats1.text= "Work time: " + Math.Floor(h)+" hours " + Math.Floor(min*60.0) +" mins"; // /100 *60
        stats2.text= "Working days: " + statistics["result"]["workingDays"];
        stats3.text= "Vacation days: " + statistics["result"]["vacationDays"];
    }

    

    private void getUserInfo(){
        StartCoroutine(Web.GetRequest(Web.GET_USER_INFO+token,Web.REQUEST.GET_USER_INFO));
        StartCoroutine(Web.GetRequest(Web.GET_USER_ROLE+token,Web.REQUEST.GET_USER_ROLE));
        StartCoroutine(Web.GetRequest(Web.getStatistics(token,"2022","6"),Web.REQUEST.GET_STATISTICS));
    }
 
    public static void ProcessUserInfo(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        userId=data["result"]["id"];
        Debug.Log("USER_INFO");
        Debug.Log(data);
    }

    public static void ProcessStatistics(string rawRespone){

        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        statistics=data;
        statisticsReady=true;
        Debug.Log("STATISTICS");
        


    }

    public static void ProcessRoleInfo(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        Debug.Log("ROLE_INFO");
        Debug.Log(data);
        userRole=data["result"]["role"];
        companyId=data["result"]["companyId"];
        departamentId=data["result"]["departamentId"];
        isAdmin=data["result"]["isAdmin"];

        

    }


    public static void ProcessLasStatus(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        Debug.Log("STATUS");
        Debug.Log(data["result"]["type"]);   
        currentStatus=data["result"]["type"];
        
        if(currentStatus==null)
            currentStatus=3;
    }

    public static void ProcessSentStatus(string rawRespone){
        updateStatistics=true;     
    }

    public static void ProcessLogoutResponse(string rawRespone){
       SceneManager.LoadScene(0);     
    }

    public void ShowSlider(){

        if(Slider !=null){
            Animator animator = Slider.GetComponent<Animator>();
            if( animator.runtimeAnimatorController!=null){
                bool isOpen= animator.GetBool("show");
                animator.SetBool("show", !isOpen);
            }
        }
    }

    void getLastStatus(){
         StartCoroutine(Web.GetRequest(Web.GET_LAST_STATUS+token,Web.REQUEST.GET_LAST_STATUS));
    }

    public void SendWorkStatus(){
        currentStatus=1;
        StartCoroutine(Web.PutRequest(Web.setStatuss(token, "1"),"e",Web.REQUEST.SEND_USER_STATUS));
    }

    public void SendBreakStatus(){
        currentStatus=2;
        StartCoroutine(Web.PutRequest(Web.setStatuss(token, "2"),"e",Web.REQUEST.SEND_USER_STATUS));
    }

    public void SendOutOfOfficeStatus(){
        currentStatus=3;
        StartCoroutine(Web.PutRequest(Web.setStatuss(token, "3"),"e",Web.REQUEST.SEND_USER_STATUS));
    }

    public void WorkersScene(){
        SceneManager.LoadScene(2);
    }
    public void VacationsScene(){
        SceneManager.LoadScene(3);
    }
    public void StatusHistoryScene(){
        SceneManager.LoadScene(4);
    }
    public void ChatScene(){
        SceneManager.LoadScene(5);
    }
    public void Logout(){
        StartCoroutine(Web.PostRequest(Web.LOGOUT_ADDRESS+token, "e",Web.REQUEST.LOGOUT));
    }
    public void Exit(){
        Application.Quit();
    }
  

}
