using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;
using System.Text;

public class Web
{
    public enum REQUEST
    {
        ///GET
        GET_USER_CHATS,
        GET_MESSAGES,
        GET_UNREAD_MESSAGES,
        GET_STATUS,
        GET_SEARCH_RESULT,
        GET_USER_INFO,
        GET_LAST_STATUS, 
        GET_USER_ROLE,
        GET_USERS,
        GET_REQUESTS_FOR_APPROVE,
        GET_STATUS_HISTORY,
        EXPORT,
        GET_USER_REQUESTS,
        GET_COMPANY_DEPARTMENTS,
        GET_GIVEN_USER_INFO,
        GET_GIVEN_USER_INFO_CHAT,
        GET_COMPANIES,
        ////PUT
        SEND_MESSAGE,
        SEND_CHAT_STATUS,
        SEND_USER_STATUS,
        CREATE_REQUEST,
        CREATE_USER,
        SET_HEAD_OF_DEPARTMENT,
        SET_COMPANY_OWNER,
        //PATCH
        EDIT_USER,
        ///POST
        CREATE_NEW_CHAT,
        LOGIN,
        LOGOUT,
        CREATE_DEPARTMENT,
        CREATE_COMPANY,
        ACCEPT_REQUEST,
        DISCARD_REQUEST,
        //DELETE
        DELETE_USER,
        DELETE_DEPARTMENT,
        DELETE_COMPANY
    }



    public static IEnumerator PostRequest(string uri, string n, REQUEST type)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(n);
        UnityWebRequest www = UnityWebRequest.Post(uri, "");
        UploadHandler uploader = new UploadHandlerRaw(bytes);
        uploader.contentType = "application/json";
        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            switch (type)
            {
                case REQUEST.CREATE_NEW_CHAT:
                    ChatScene.setRequestInfo(REQUEST.CREATE_NEW_CHAT,www.downloadHandler.text);
                    break;
                case REQUEST.CREATE_DEPARTMENT:
                    WorkersScene.setRequestInfo(REQUEST.CREATE_DEPARTMENT,www.downloadHandler.text);
                    break;
                case REQUEST.CREATE_COMPANY:
                    WorkersScene.setRequestInfo(REQUEST.CREATE_COMPANY,www.downloadHandler.text);
                    break;
                case REQUEST.ACCEPT_REQUEST:
                    VacationsScene.setRequestInfo(REQUEST.ACCEPT_REQUEST,www.downloadHandler.text);
                    break;
                case REQUEST.DISCARD_REQUEST:
                    VacationsScene.setRequestInfo(REQUEST.DISCARD_REQUEST,www.downloadHandler.text);
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

    public static IEnumerator PatchRequest(string uri, string messageData, REQUEST type){

        byte[] bytes = Encoding.UTF8.GetBytes(messageData);
        UnityWebRequest www = UnityWebRequest.Put(uri, bytes);
        www.method = "PATCH";
        UploadHandler uploader = new UploadHandlerRaw(bytes);

        uploader.contentType = "application/json";

        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            switch (type)
            {
                case REQUEST.EDIT_USER:
                    WorkersScene.setRequestInfo(REQUEST.EDIT_USER,www.downloadHandler.text);
                    break;
                default:
                    Debug.LogError("Wrong request type PATCH!");
                    break;
            }
        }
    }
    public static IEnumerator DeleteRequest(string uri, string messageData, REQUEST type){

        byte[] bytes = Encoding.UTF8.GetBytes(messageData);
        UnityWebRequest www = UnityWebRequest.Put(uri, bytes);
        www.method = "DELETE";
        UploadHandler uploader = new UploadHandlerRaw(bytes);

        uploader.contentType = "application/json";

        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            switch (type)
            {
                case REQUEST.DELETE_USER:
                    WorkersScene.setRequestInfo(REQUEST.DELETE_USER,www.downloadHandler.text);
                    break;
                case REQUEST.DELETE_DEPARTMENT:
                    WorkersScene.setRequestInfo(REQUEST.DELETE_DEPARTMENT,www.downloadHandler.text);
                    break;
                case REQUEST.DELETE_COMPANY:
                    WorkersScene.setRequestInfo(REQUEST.DELETE_COMPANY,www.downloadHandler.text);
                    break;
                default:
                    Debug.LogError("Wrong request type DELETE!");
                    break;
            }
        }
    }

    public static IEnumerator PutRequest(string uri, string messageData, REQUEST type)
    {

        byte[] bytes = Encoding.UTF8.GetBytes(messageData);
        UnityWebRequest www = UnityWebRequest.Put(uri, bytes);
        UploadHandler uploader = new UploadHandlerRaw(bytes);

        uploader.contentType = "application/json";

        www.uploadHandler = uploader;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {
            switch (type)
            {
                case REQUEST.SEND_MESSAGE:
                    ChatScene.setRequestInfo(REQUEST.SEND_MESSAGE,www.downloadHandler.text);
                    break;
                case REQUEST.SEND_CHAT_STATUS:
                    ChatScene.setRequestInfo(REQUEST.SEND_CHAT_STATUS,www.downloadHandler.text);
                    break;
                case REQUEST.SEND_USER_STATUS:
                    MainScene.ProcessSentStatus(www.downloadHandler.text);
                    break;
                case REQUEST.CREATE_REQUEST:
                    VacationsScene.setRequestInfo(REQUEST.CREATE_REQUEST,www.downloadHandler.text);
                    break;
                case REQUEST.CREATE_USER:
                    WorkersScene.setRequestInfo(REQUEST.CREATE_USER,www.downloadHandler.text);
                    break;
                case REQUEST.EDIT_USER:
                    WorkersScene.setRequestInfo(REQUEST.EDIT_USER,www.downloadHandler.text);
                    break;
                case REQUEST.SET_HEAD_OF_DEPARTMENT:
                    WorkersScene.setRequestInfo(REQUEST.SET_HEAD_OF_DEPARTMENT,www.downloadHandler.text);
                    break;
                case REQUEST.SET_COMPANY_OWNER:
                    WorkersScene.setRequestInfo(REQUEST.SET_COMPANY_OWNER,www.downloadHandler.text);
                    break;              
                default:
                    Debug.LogError("Wrong request type PUT!");
                    break;
            }
        }
    }
    public static IEnumerator GetRequest(string uri, REQUEST type)
    {

        
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Something went wrong " + www.error);
            Debug.LogError(www.downloadHandler.text);
        }
        else
        {

            switch (type)
            {
                case REQUEST.GET_USER_INFO:
                    MainScene.ProcessUserInfo(www.downloadHandler.text);
                    break;
                case REQUEST.GET_LAST_STATUS:
                    MainScene.ProcessLasStatus(www.downloadHandler.text);
                    break;
                case REQUEST.GET_USER_ROLE:
                    MainScene.ProcessRoleInfo(www.downloadHandler.text);
                    break;
                case REQUEST.GET_USER_CHATS:
                    ChatScene.setRequestInfo(REQUEST.GET_USER_CHATS,www.downloadHandler.text);
                    break;
                case REQUEST.GET_MESSAGES:
                    ChatScene.setRequestInfo(REQUEST.GET_MESSAGES,www.downloadHandler.text);
                    break;
                case REQUEST.GET_UNREAD_MESSAGES:
                    ChatScene.setRequestInfo(REQUEST.GET_UNREAD_MESSAGES,www.downloadHandler.text);
                    break;
                case REQUEST.GET_STATUS:
                    ChatScene.setRequestInfo(REQUEST.GET_STATUS,www.downloadHandler.text);
                    break;
                case REQUEST.GET_SEARCH_RESULT:
                    ChatScene.setRequestInfo(REQUEST.GET_SEARCH_RESULT,www.downloadHandler.text);
                    break;
                case REQUEST.GET_STATUS_HISTORY:
                    StatusHistoryScene.setRequestInfo(REQUEST.GET_STATUS_HISTORY,www.downloadHandler.text);
                    break;
                case REQUEST.EXPORT:
                    StatusHistoryScene.setRequestInfo(REQUEST.EXPORT,www.downloadHandler.text);
                    break;
                case REQUEST.GET_USER_REQUESTS:
                    VacationsScene.setRequestInfo(REQUEST.GET_USER_REQUESTS,www.downloadHandler.text);
                    break;
                case REQUEST.GET_COMPANY_DEPARTMENTS:
                    WorkersScene.setRequestInfo(REQUEST.GET_COMPANY_DEPARTMENTS,www.downloadHandler.text);
                    break;
                case REQUEST.GET_USERS:
                    WorkersScene.setRequestInfo(REQUEST.GET_USERS,www.downloadHandler.text);
                    break;
                case REQUEST.GET_GIVEN_USER_INFO:
                    WorkersScene.setRequestInfo(REQUEST.GET_GIVEN_USER_INFO,www.downloadHandler.text);
                    break;
                case REQUEST.GET_GIVEN_USER_INFO_CHAT:{
                    ChatScene.setRequestInfo(REQUEST.GET_GIVEN_USER_INFO_CHAT,www.downloadHandler.text);
                    break;
                }
                case REQUEST.GET_COMPANIES:
                    WorkersScene.setRequestInfo(REQUEST.GET_COMPANIES,www.downloadHandler.text);
                    break;
                case REQUEST.GET_REQUESTS_FOR_APPROVE:
                    VacationsScene.setRequestInfo(REQUEST.GET_REQUESTS_FOR_APPROVE,www.downloadHandler.text);
                    break;
                default:
                    Debug.LogError("Wrong request type!");
                    break;
            }
        }

    }
    public static string HOST_ADDRESS = "http://workportal-api.damol.pl";
    public static string LOGIN_ADDRESS = HOST_ADDRESS + "/api/Auth/login";
    public static string REGISTER_ADDRESS = HOST_ADDRESS + "/api/Auth/register";
    public static string LOGOUT_ADDRESS = HOST_ADDRESS + "/api/Auth/logout?token=";
    public static string GET_USER_INFO = HOST_ADDRESS + "/api/User/myInfo?token="; 
    public static string GET_USER_ROLE = HOST_ADDRESS + "/api/Role?token=";
    /// CHAT
    public static string CREATE_PRIVATE_CHAT = HOST_ADDRESS + "/api/Chat/createPrivateChat?token=";
    public static string GET_STATUS = HOST_ADDRESS + "/api/Chat/getStatus?token=";
    public static string SEND_MESSAGE = HOST_ADDRESS + "/api/Chat/addMessage?token=";
    public static string GET_USER_CHATS = HOST_ADDRESS + "/api/Chat/getChatsForUser?token=";

    public static string getMessages(string chatId, string token, string startUUID, string endUUID)
    {
        string address = HOST_ADDRESS + "/api/Chat/getMessages?chatId=" + chatId;
        address += "&token=" + token;

        if (startUUID != null)
            address += "&startUUID=" + startUUID;
        if (endUUID != null)
            address += "&endUUID=" + endUUID;

        address += "&n=20";
        return address;
    }
    public static string setStatus(string chatId, string uuid, string token)
    {

        string address = HOST_ADDRESS + "/api/Chat/setStatus?chatId=";
        address += chatId;
        if (uuid != null)
            address += "&UUID=" + uuid;
        address += "&token=" + token;
        return address;
    }
    public static string findUser(string username, string token, string companyId, string departmentId)
    {
        string address = HOST_ADDRESS + "/api/User/find?token=" + token;
        address += "&userName=" + username;

        if (companyId != null)
            address += "&companyId=" + companyId;
        if (departmentId != null)
            address += "&departamentId=" + departmentId;

        return address;
    }
    //STATUS
    public static string GET_STATUS_HISTORY = HOST_ADDRESS + "/api/Status?token=";
    public static string GET_LAST_STATUS = HOST_ADDRESS + "/api/Status/last?token=";
    public static string getUserStatus(string token, string userId)
    {
        return HOST_ADDRESS + "/api/Status/" + userId + "?token=" + token;
    }

    public static string setStatuss(string token, string statusType)
    {
        return HOST_ADDRESS + "/api/Status/setStatus?token=" + token + "&statusTypeId=" + statusType;
    }

    public static string export(string token, int month, int year, string userId)
    {
        string address = HOST_ADDRESS + "/api/Status/export";

        if (userId != null)
            address += "/" + userId;

        address += "?token=" + token + "&month=" + month + "&year=" + year;
        return address;
    }

    //HOLIDAYS
    public static string CREATE_REQUEST = HOST_ADDRESS + "/api/Vacation/createRequest?token=";
    public static string VACATION = HOST_ADDRESS + "/api/Vacation?token=";
    public static string GET_REQUESTS_FOR_APPROVE = HOST_ADDRESS + "/api/Vacation/getRequestsForApproverWithUserInfo?token=";

    public static string acceptRequest(string token, string requestId){
        return HOST_ADDRESS +"/api/Vacation/acceptRequest?token=" + token +"&requestId="+requestId;
    }
    public static string discardRequest(string token, string requestId){
        return HOST_ADDRESS+"/api/Vacation/rejectRequest?token="+token+"&requestId="+requestId;
    }

    ///WORKERS 
    //USER
    public static string createUser(string token, string companyId, string departmentId, string roleId){
        return HOST_ADDRESS + "/api/User/create?token="+token+"&companyId="+companyId+"&departamentId="+departmentId+"&roleType="+roleId;
    }

    public static string editUser(string token, string userId){
        return  HOST_ADDRESS + "/api/User/edit/"+userId+"?token="+token;
    }

    public static string deleteUser(string token, string userId){
        return HOST_ADDRESS + "/api/User/delete/"+userId+"?token="+token;
    }

    public static string getCompanyDepartments(string token, string companyId){
        return HOST_ADDRESS + "/api/Department/"+companyId+"?token="+token;
    }

    public static string GET_USERS = HOST_ADDRESS +  "/api/User/info?token="; //users list

    public static string getGivenUserInfo(string token, string userId){
        return  HOST_ADDRESS +"/api/User/info/"+userId+"?token="+token;
    }
    //DEPARTMENT
    public static string GET_COMPANIES = HOST_ADDRESS + "/api/Company/getAll?token=";
    public static string CREATE_DEPARTMENT = HOST_ADDRESS+ "/api/Department/create?token=";

    public static string setHeadOfDepartment(string token, string companyId, string departamentId, string userId){
        return HOST_ADDRESS +"/api/Department/SetHeadOfDepartament?companyId=" +companyId+ "&departamentId="+departamentId+"&userId="+userId+"&token="+token;
    }

    public static string deleteDepartment(string token,string departamentId ){
        return HOST_ADDRESS + "/api/Department/delete?departamentId="+departamentId+"&token="+token;
    }
    //COMPANY
    public static string createCompany(string token, string companyName){
        return HOST_ADDRESS +"/api/Company/create?Name="+companyName+"&token="+token;
    }

    public static string setCompanyOwner(string token, string companyId, string userId){
        return HOST_ADDRESS + "/api/Company/SetCompanyOwner?companyId="+companyId+"&userId="+userId+"&token="+token;
    }

    public static string deleteCompany(string token, string companyId){
        return HOST_ADDRESS + "/api/Company/delete?companyId="+companyId+"&token="+token;
    }
}
