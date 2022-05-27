using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;
using System.Text;

public class Web {

    public enum REQUEST{
        ///GET
        GET_USER_CHATS,
        GET_MESSAGES,
        GET_UNREAD_MESSAGES,
        GET_STATUS, 
        GET_SEARCH_RESULT, 
        GET_USER_INFO, 
        GET_LAST_STATUS, //MAIN SCENE ?
        //GET_USER_STATUS, //MAIN SCENE ?
        //SET_USER_STATUS, //SET STATUS - //MAIN SCENE
        GET_STATUS_HISTORY, 
        EXPORT,
        GET_USER_REQUESTS,
        ////PUT
        SEND_MESSAGE, 
        SEND_CHAT_STATUS,
        SEND_USER_STATUS,
        CREATE_REQUEST,
        ///POST
        CREATE_NEW_CHAT,
        LOGIN,
        LOGOUT


    }


    public static IEnumerator PostRequest(string uri, string n, REQUEST type){

        byte[] bytes = Encoding.ASCII.GetBytes(n);
        UnityWebRequest www = UnityWebRequest.Post(uri,"");
        UploadHandler uploader = new UploadHandlerRaw(bytes);
        uploader.contentType = "application/json";
        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
           Debug.LogError("Something went wrong " + www.error); 
           Debug.LogError(www.downloadHandler.text);      
        }
        else
        {
            switch(type){
                case REQUEST.CREATE_NEW_CHAT:
                    ChatScene.requestType=REQUEST.CREATE_NEW_CHAT;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.LOGIN:
                    LoginScene.ProcessLoginResponse(www.downloadHandler.text);
                    break;
                case REQUEST.LOGOUT:
                    MainScene.ProcessLogoutResponse(www.downloadHandler.text);
                    break;
                default:
                    Debug.LogError("Wrong request type!");
                    break;   
            }
            
        }

    }

    public static IEnumerator PutRequest(string uri, string messageData, REQUEST type){

        byte[] bytes = Encoding.ASCII.GetBytes(messageData);
        UnityWebRequest www = UnityWebRequest.Put(uri, bytes);
        UploadHandler uploader = new UploadHandlerRaw(bytes);

        uploader.contentType = "application/json";

        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
           Debug.LogError("Something went wrong " + www.error); 
           Debug.LogError(www.downloadHandler.text);      
        }
        else
        {
            switch(type){
                case REQUEST.SEND_MESSAGE:
                    ChatScene.requestType=REQUEST.SEND_MESSAGE;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.SEND_CHAT_STATUS:
                    ChatScene.requestType=REQUEST.SEND_CHAT_STATUS;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;                 
                    break;
                case REQUEST.SEND_USER_STATUS:
                     MainScene.ProcessSentStatus(www.downloadHandler.text);
                     break;
                case REQUEST.CREATE_REQUEST:
                    VacationsScene._requestType=REQUEST.CREATE_REQUEST;
                    VacationsScene.newRequest=true;
                    VacationsScene.requestMessage=www.downloadHandler.text;
                    break;
                default:
                    Debug.LogError("Wrong request type!");
                    break;              
            }
            
        }
    }

    public static IEnumerator GetRequest(string uri, REQUEST type){

        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
           Debug.LogError("Something went wrong " + www.error);
           Debug.LogError(www.downloadHandler.text);       
        }
        else
        {
           
            switch(type){
                case REQUEST.GET_USER_INFO:
                    MainScene.ProcessUserInfo(www.downloadHandler.text);
                    break;
                case REQUEST.GET_LAST_STATUS:
                    MainScene.ProcessLasStatus(www.downloadHandler.text);
                    break;
                case REQUEST.GET_USER_CHATS:
                    ChatScene.requestType=REQUEST.GET_USER_CHATS;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_MESSAGES:              
                    ChatScene.requestType=REQUEST.GET_MESSAGES;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_UNREAD_MESSAGES:
                    ChatScene.requestType=REQUEST.GET_UNREAD_MESSAGES;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_STATUS:
                    ChatScene.requestType=REQUEST.GET_STATUS;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_SEARCH_RESULT:
                    ChatScene.requestType=REQUEST.GET_SEARCH_RESULT;
                    ChatScene.newRequest=true;
                    ChatScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_STATUS_HISTORY:
                    StatusHistoryScene.requestType=REQUEST.GET_STATUS_HISTORY;
                    StatusHistoryScene.newRequest=true;
                    StatusHistoryScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.EXPORT:
                    StatusHistoryScene.requestType=REQUEST.EXPORT;
                    StatusHistoryScene.newRequest=true;
                    StatusHistoryScene.requestMessage=www.downloadHandler.text;
                    break;
                case REQUEST.GET_USER_REQUESTS:
                    VacationsScene._requestType=REQUEST.GET_USER_REQUESTS;
                    VacationsScene.newRequest=true;
                    VacationsScene.requestMessage=www.downloadHandler.text;
                    break;
                default:
                    Debug.LogError("Wrong request type!");
                    break; 
            }                        
        }

    }

    public static string LOGIN_ADDRESS=  "http://workportal-api.damol.pl/api/Auth/login";  
    public static string REGISTER_ADDRESS=  "http://workportal-api.damol.pl/api/Auth/register";  
    public static string LOGOUT_ADDRESS=  "http://workportal-api.damol.pl/api/Auth/logout?token=";
    public static string GET_USER_INFO ="http://workportal-api.damol.pl/api/User/DEBUG/myUserInfo?token=";

    /// CHAT
    public static string CREATE_PRIVATE_CHAT=  "http://workportal-api.damol.pl/api/Chat/createPrivateChat?token=";
    public static string GET_STATUS=  "http://workportal-api.damol.pl/api/Chat/getStatus?token=";
    public static string SEND_MESSAGE="http://workportal-api.damol.pl/api/Chat/addMessage?token=";
    public static string GET_USER_CHATS="http://workportal-api.damol.pl/api/Chat/getChatsForUser?token=";

    public static string getMessages(string chatId, string token, string startUUID, string endUUID){
        string address="http://workportal-api.damol.pl/api/Chat/getMessages?chatId="+chatId;
        address+="&token="+token;

        if(startUUID!=null)
            address+="&startUUID="+startUUID;
        if(endUUID!=null)
            address+="&endUUID="+endUUID;

        address+="&n=20";
        return address;

    }
    public static string setStatus(string chatId, string uuid, string token){
        
        string address="http://workportal-api.damol.pl/api/Chat/setStatus?chatId=";
        address+=chatId;
        if(uuid!=null)
            address+="&UUID="+uuid;
        address+="&token="+token;
        return address;
        
     }

    public static string findUser(string username, string token, string companyId, string departmentId){
        string address ="http://workportal-api.damol.pl/api/User/find?token="+token;
        address+="&userName="+username;

        if(companyId!=null)
            address+="&companyId="+companyId;
        if(departmentId!=null)
            address+="&departamentId="+departmentId;

        return address;     

     }
    //todo string builder
   
    //STATUS
    public static string GET_STATUS_HISTORY ="http://workportal-api.damol.pl/api/Status?token=";
    public static string GET_LAST_STATUS ="http://workportal-api.damol.pl/api/Status/last?token=";

    public static string getUserStatus(string token, string userId){

        return "http://workportal-api.damol.pl/api/Status/"+userId+"?token="+token;
    }

    public static string setStatuss(string token, string statusType){
        return "http://workportal-api.damol.pl/api/Status/setStatus?token="+token+"&statusTypeId="+statusType;
    }

    public static string export(string token, int month, int year, string userId){
        string address="http://workportal-api.damol.pl/api/Status/export";

        if(userId!=null)
            address+="/"+userId;
        
        address+="?token="+token+"&month="+month+"&year="+year;

        return address;
    }

    //HOLIDAYS
    public static string CREATE_REQUEST ="http://workportal-api.damol.pl/api/Vacation/createRequest?token="; //http://workportal-api.damol.pl/api/Vacation/createRequest?token=0a1b24b67fa247e185aaf4583c44f894
    public static string ACCEPT_REQUEST="";
    public static string REJECT_REQUEST="";
    public static string VACATION ="http://workportal-api.damol.pl/api/Vacation?token="; //http://workportal-api.damol.pl/api/Vacation?token=0a1b24b67fa247e185aaf4583c44f894  - moje requesty
    public static string PRIVILEGE_BASED="";

}
