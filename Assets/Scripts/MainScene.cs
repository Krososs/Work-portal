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


    public static string email;
    public static string password;
    public static string token;
    public static string userId;
    private static int currentStatus;

    public Button workButton;
    public Button breakButton;
    public Button quitButton;

   
    void Start(){
        username.text=email;  
        getUserInfo();
        getLastStatus();
    }

    void Update(){
        if(currentStatus==3 || currentStatus==2)
            breakButton.interactable=false;
        else
            breakButton.interactable=true;

        if(currentStatus==1)
            workButton.interactable=false;
        else
            workButton.interactable=true;       
    }

    private void getUserInfo(){
         StartCoroutine(Web.GetRequest(Web.GET_USER_INFO+token,Web.REQUEST.GET_USER_INFO));
    }
 
    public static void ProcessUserInfo(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        userId=data["result"]["id"];
    }

    public static void ProcessLasStatus(string rawRespone){
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);   
        currentStatus=data["result"]["type"];
    }

    public static void ProcessSentStatus(string rawRespone){
       
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
