using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using SimpleJSON;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Security.Cryptography;

public class ChatScene : MonoBehaviour
{

    public GameObject messageInput;
    public InputField _messageInput;

    public GameObject search;
    public GameObject searchPanel;
    public GameObject searchContainer;
    public InputField _searchInput;
    private bool fristSearch;

    //chat
    public GameObject chatContainer;
    public GameObject messageContent;
    public GameObject messageContainer;
    public GameObject chatPanel;
    public GameObject invisiblePanel;

    private JSONNode userChats;
    private JSONNode messages;


    //users
    public GameObject usersMainPanel;
    public GameObject usersContainer;
    public GameObject usersPanel;
    public GameObject usernameContent;
    public GameObject companyContent;
    public GameObject userId;
    private int currentId;
    Dictionary<int, string> users = new Dictionary<int, string>(); // id / firstname + surname
    Dictionary<string, JSONNode> _messages = new Dictionary<string, JSONNode>();

    //chat
    Vector2 initialChatSize;
    Vector2 initialSearchSize;
    Vector2 initialUsersSize;

    public static bool reloadChat;

    public static bool newRequest;
    public static string requestMessage;
    public static Web.REQUEST requestType;

    public static string choosenChatId;
    public static string choosenUserId;
    public static string choosenChatName;

    private string currentMessage;
    private string chatId;
    private string startUUID;
    private string endUUID;
    private float timeToRefresh;
    private bool usersLoaded=false;

    [Serializable]
    public class historyMessage
    {
        public string chatId;
        public string userId;
        public string content;
        public string uuid;
    }
    [Serializable]
    public class Message
    {
        public string chatId;
        public string userId;
        public string content;
    }
    [Serializable]
    public class Status
    {
        public string chatId;
        public string UUID;
        public string token;
    }
    [Serializable]
    public class Chat
    {
        public string firstUserId;
        public string secondUserId;
    }

    void Start()
    {
        chatId = "";
        search.gameObject.SetActive(false);
        choosenUserId = "";
        choosenChatId = "";
        // MainScene.token="5634992f9e354cb489fd5e9728be555b";
        // MainScene.userId="13"; 
        fristSearch=true;
        timeToRefresh = 2;
        startUUID = null;
        endUUID = null;
        reloadChat = false;
        newRequest = false;
        initialChatSize = new Vector2(chatContainer.GetComponent<RectTransform>().rect.width, chatContainer.GetComponent<RectTransform>().rect.height);
        initialSearchSize = new Vector2(searchContainer.GetComponent<RectTransform>().rect.width, searchContainer.GetComponent<RectTransform>().rect.height);
        initialUsersSize = new Vector2(usersContainer.GetComponent<RectTransform>().rect.width, usersContainer.GetComponent<RectTransform>().rect.height);
        StartCoroutine(Web.GetRequest(Web.GET_USER_CHATS + MainScene.token, Web.REQUEST.GET_USER_CHATS));
    }

    void Update()
    {
        if (newRequest)
        {
            newRequest = false;
            ProcessNewRequest(requestType, requestMessage);
        }

        if (reloadChat)
        {
            reloadChat = false;

            if (choosenChatId.Length > 0 || chatExists())
            {
                saveMessagesHistory();
                startUUID = null;
                endUUID = null;

                if (choosenChatId.Length > 0) chatId = choosenChatId;

                loadMessages();
            }
            else
                createNewChat();

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Message message = new Message();
            message.chatId = chatId;
            message.userId = MainScene.userId;
            message.content = currentMessage;
            StartCoroutine(Web.PutRequest(Web.SEND_MESSAGE + MainScene.token, JsonUtility.ToJson(message), Web.REQUEST.SEND_MESSAGE));
        }

        if (timeToRefresh > 0)
            timeToRefresh -= Time.deltaTime;
        else
        {
            getStatus();
            saveMessagesHistory();
            timeToRefresh = 2;
        }

        if (_searchInput.isFocused){
            search.gameObject.SetActive(true);
            if(fristSearch){
                //puste zapytanie
                StartCoroutine(Web.GetRequest(Web.findUser(_searchInput.text.ToString(), MainScene.token, null, null), Web.REQUEST.GET_SEARCH_RESULT));
                fristSearch=false;
            }

        }else{
            fristSearch=true;
        }

        // if(usersLoaded){
        //     usersLoaded=false;
        //     
        // }

    }

    public static void setRequestInfo(Web.REQUEST type, string rawRespone){
        newRequest=true;
        requestMessage=rawRespone;
        requestType=type;
    }

    private void ProcessNewRequest(Web.REQUEST type, string rawRespone)
    {

        switch (type)
        {
            case Web.REQUEST.GET_USER_CHATS:
                ProcessChatsResponse(rawRespone);
                break;
            case Web.REQUEST.GET_MESSAGES:
                ProcessMessagesResponse(rawRespone);
                break;
            case Web.REQUEST.GET_UNREAD_MESSAGES:
                ProccessUnreadMessagesResponse(rawRespone);
                break;
            case Web.REQUEST.GET_STATUS:
                ProcessStatusRespone(rawRespone);
                break;
            case Web.REQUEST.GET_SEARCH_RESULT:
                ProcessSearchResultResponse(rawRespone);
                break;
            case Web.REQUEST.SEND_MESSAGE:
                ProcessNewMessageResponse(rawRespone);
                break;
            case Web.REQUEST.SEND_CHAT_STATUS:
                ProcessSentStatusResponse(rawRespone);
                break;
            case Web.REQUEST.CREATE_NEW_CHAT:
                ProcessNewChatResponse(rawRespone);
                break;
            default:
                Debug.LogError("Wrong request type!");
                break;
        }

    }

    void ProcessSearchResultResponse(string rawRespone)
    {
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        Debug.Log("Search result");

        clearSearchResults();

        int resultCounter = 0;
        foreach (KeyValuePair<string, JSONNode> entry in data["result"])
        {

            GameObject username = Instantiate(usernameContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject company = Instantiate(companyContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject department = Instantiate(companyContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject chatid = Instantiate(userId, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject _userid = Instantiate(userId, new Vector3(0, 0, 0), Quaternion.identity); //
            GameObject container = Instantiate(usersPanel, new Vector3(0, 0, 0), Quaternion.identity);
            container.GetComponent<Image>().color = new Color(0.227f, 0.27f, 0.36f, 1.0f);

            username.GetComponent<Text>().text = entry.Value["firstName"] + " " + entry.Value["surname"];
            company.GetComponent<Text>().text = "Company -" + entry.Value["companyName"];
            department.GetComponent<Text>().text = "Department -" + entry.Value["departamentName"];
            _userid.GetComponent<Text>().text = entry.Value["id"];
            chatid.GetComponent<Text>().text = "";

            username.transform.SetParent(container.transform, false);
            company.transform.SetParent(container.transform, false);
            department.transform.SetParent(container.transform, false);
            chatid.transform.SetParent(container.transform, false);
            _userid.transform.SetParent(container.transform, false);

            container.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            container.transform.SetParent(searchContainer.transform, false);

            resultCounter++;
        }
        if (resultCounter > 4)
        {

            resultCounter -= 4;
            float width = searchContainer.GetComponent<RectTransform>().rect.width;
            float height = searchContainer.GetComponent<RectTransform>().rect.height;
            float userPanelHeight = usersPanel.GetComponent<RectTransform>().rect.height;
            searchContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height + (resultCounter * 100)); //initial size + (resultCounter * userPanelHeight) +(resultCounter*10)
            searchPanel.GetComponentInChildren<Scrollbar>().value = 1;
        }

    }

    void ProcessNewChatResponse(string rawRespone)
    {
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        chatId = data["result"]["id"];
        choosenChatId = data["result"]["secondUserId"]; // ??
        clearChat();
        StartCoroutine(Web.GetRequest(Web.GET_USER_CHATS + MainScene.token, Web.REQUEST.GET_USER_CHATS));
    }

    void ProcessChatsResponse(string rawRespone)
    {
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        userChats = SimpleJSON.JSON.Parse(rawRespone)["result"];
        int chatsCounter = 0;
        
        clearUserChats();
        foreach (KeyValuePair<string, JSONNode> entry in data["result"])
        {
            
            Debug.Log("CHAT");
            Debug.Log(entry);
            string chatName = "";
            string companyName = "";
            string departamentName = "";

            int usersCounter = 0;
            foreach (KeyValuePair<string, JSONNode> user in entry.Value["description"]["users"])
            {
                
                if(user.Value["id"].ToString()!=MainScene.userId){
                    Debug.Log("Inny user");
                    Debug.Log(user.Value["id"]);
                    Debug.Log(MainScene.userId);
                    chatName = user.Value["firstName"] + " " + user.Value["surname"] + " ";
                    companyName = user.Value["companyName"];
                    departamentName = user.Value["departmentName"];
                }
                usersCounter++;
            }

            if (usersCounter > 2)
            {
                chatName = "Group chat";
            }

            GameObject username = Instantiate(usernameContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject company = Instantiate(companyContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject department = Instantiate(companyContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject chatid = Instantiate(userId, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject _userid = Instantiate(userId, new Vector3(0, 0, 0), Quaternion.identity); //
            GameObject container = Instantiate(usersPanel, new Vector3(0, 0, 0), Quaternion.identity);

            container.GetComponent<Image>().color = new Color(0.227f, 0.27f, 0.36f, 1.0f);

            username.GetComponent<Text>().text = chatName;
            company.GetComponent<Text>().text = "Company - " + companyName;
            department.GetComponent<Text>().text = "Department - " + departamentName;
            _userid.GetComponent<Text>().text = "";
            chatid.GetComponent<Text>().text = entry.Value["chat"]["id"];

            username.transform.SetParent(container.transform, false);
            company.transform.SetParent(container.transform, false);
            department.transform.SetParent(container.transform, false);
            chatid.transform.SetParent(container.transform, false);
            _userid.transform.SetParent(container.transform, false);
            container.transform.SetParent(usersContainer.transform, false);

            chatsCounter++;
        }
        if (chatsCounter > 8)
        {

            chatsCounter -= 8;
            float width = usersContainer.GetComponent<RectTransform>().rect.width;
            float height = usersContainer.GetComponent<RectTransform>().rect.height;
            float userPanelHeight = usersPanel.GetComponent<RectTransform>().rect.height;
            usersContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height + (chatsCounter * 100) + (chatsCounter * 12));
            usersMainPanel.GetComponentInChildren<Scrollbar>().value = 1;
        }
        hideAllNotfications();
    }

    void ProcessMessagesResponse(string rawRespone)
    {
        Debug.Log("MESSAGES");
        Debug.Log(SimpleJSON.JSON.Parse(rawRespone));

        saveMessages(SimpleJSON.JSON.Parse(rawRespone));
        loadUsersInfo(SimpleJSON.JSON.Parse(rawRespone));
        showMessages();

    }

    void ProccessUnreadMessagesResponse(string rawRespone)
    {
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);

        Dictionary<string, JSONNode> newMessages = new Dictionary<string, JSONNode>();
        Dictionary<string, System.DateTime> timeStamps = new Dictionary<string, System.DateTime>();

        foreach (KeyValuePair<string, JSONNode> entry in data["result"])
        {
            System.DateTime date = System.DateTime.Parse(entry.Value["timestamp"], System.Globalization.CultureInfo.GetCultureInfo("en-us"));
            timeStamps[entry.Value["uuid"]] = date;
        }

        foreach (KeyValuePair<string, System.DateTime> entry in timeStamps.OrderBy(key => key.Value))
        {
            foreach (KeyValuePair<string, JSONNode> entry2 in data["result"])
            {
                System.DateTime date = System.DateTime.Parse(entry2.Value["timestamp"], System.Globalization.CultureInfo.GetCultureInfo("en-us"));
                if (entry.Value == date)
                {
                    newMessages[entry2.Value["uuid"]] = entry2;
                }
            }
        }

        foreach (KeyValuePair<string, JSONNode> entry in newMessages)
        {
            endUUID = entry.Value["uuid"];
            showNewMessage(entry);

        }
        setStatus();
    }

    void ProcessStatusRespone(string rawRespone)
    {

        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        foreach (KeyValuePair<string, JSONNode> entry in data["result"])
        {
            if (!entry.Value["upToDate"])
            {
                if (entry.Key == chatId)
                {
                    StartCoroutine(Web.GetRequest(Web.getMessages(chatId, MainScene.token, endUUID, null), Web.REQUEST.GET_UNREAD_MESSAGES));
                }
                else
                {
                    showNotification(entry.Key.ToString());
                }
            }
        }
    }

    void ProcessNewMessageResponse(string rawRespone)
    {
        JSONNode data = SimpleJSON.JSON.Parse(rawRespone);
        _messageInput.text = "";
        endUUID = data["result"]["uuid"];
        setStatus();
        showNewMessage(data["result"]);

    }

    void ProcessSentStatusResponse(string rawRespone)
    {
    }

    void loadUsersInfo(JSONNode messages){

        foreach (KeyValuePair<string, JSONNode> entry in messages["result"]["description"]["users"]){
            users[entry.Value["id"]]=entry.Value["firstName"]+" "+entry.Value["surname"];
        }
     
    }

    void showMessages()
    {

        clearChat();
        float allMessagesHeight = 0.0F;
        string m;
        int counter = 0;

        foreach (KeyValuePair<string, JSONNode> entry in _messages)
        {

            counter++;
            GameObject message = Instantiate(messageContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject chatName = Instantiate(usernameContent, new Vector3(0, 0, 0), Quaternion.identity);
            GameObject container = Instantiate(messageContainer, new Vector3(0, 0, 0), Quaternion.identity);

            if (entry.Value["userId"].ToString() == MainScene.userId)
            {
                container.GetComponent<Image>().color = new Color(0.329f, 0.339f, 0.358f, 1.0f);
                chatName.GetComponent<Text>().text = users[entry.Value["userId"]];
            }
            else
            {
                container.GetComponent<Image>().color = new Color(0.329f, 0.339f, 0.358f, 1.0f);
                chatName.GetComponent<Text>().text = users[entry.Value["userId"]];
            }

            m = entry.Value["content"];
            message.GetComponent<Text>().text = m;
            setMessageContainerSize(m.Length, container);
            setMessageContainerSize(m.Length, message);

            chatName.transform.SetParent(container.transform, false);
            message.transform.SetParent(container.transform, false);
            container.transform.SetParent(chatContainer.transform, false);

            allMessagesHeight += container.GetComponent<RectTransform>().rect.height;
            endUUID = entry.Key;
        }

        float width = chatContainer.GetComponent<RectTransform>().rect.width;
        chatContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, allMessagesHeight + (counter * 10.00F) + 10.00F); // 100 ->messagesCounter * 10 ? + 10
        chatPanel.GetComponentInChildren<Scrollbar>().value = -5;
        setStatus();
    }

    void saveMessages(JSONNode messages)
    {

        Dictionary<string, JSONNode> tempMessages = new Dictionary<string, JSONNode>();

        foreach (KeyValuePair<string, JSONNode> entry in messages["result"]["messages"])
            tempMessages[entry.Value["uuid"]] = entry;

        Dictionary<string, JSONNode> reversedMessages = new Dictionary<string, JSONNode>();

        var reversed = tempMessages.Reverse();

        foreach (var item in reversed)
            reversedMessages[item.Key] = item.Value;

        tempMessages.Clear();
        tempMessages = reversedMessages;

        foreach (KeyValuePair<string, JSONNode> entry in tempMessages)
            _messages[entry.Value["uuid"]] = entry;
    }

    void showNewMessage(JSONNode _message)
    {

        GameObject message = Instantiate(messageContent, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject chatName = Instantiate(usernameContent, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject container = Instantiate(messageContainer, new Vector3(0, 0, 0), Quaternion.identity);

        if (_message["userId"].ToString() == MainScene.userId)
        {
            container.GetComponent<Image>().color = new Color(0.329f, 0.339f, 0.358f, 1.0f);
            chatName.GetComponent<Text>().text = users[_message["userId"]];
        }
        else
        {
            container.GetComponent<Image>().color = new Color(0.329f, 0.339f, 0.358f, 1.0f);
            chatName.GetComponent<Text>().text = users[_message["userId"]];
        }

        message.GetComponent<Text>().text = _message["content"];
        string[] lines = message.GetComponent<Text>().text.Split('\n');
        setMessageContainerSize(_message["content"].ToString().Length, container);
        setMessageContainerSize(_message["content"].ToString().Length, message);

        chatName.transform.SetParent(container.transform, false);
        message.transform.SetParent(container.transform, false);
        container.transform.SetParent(chatContainer.transform, false);

        float width = chatContainer.GetComponent<RectTransform>().rect.width;
        float height = chatContainer.GetComponent<RectTransform>().rect.height;

        chatContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height + getContainerHeight(container) + 20.00F); 
        chatPanel.GetComponentInChildren<Scrollbar>().value = -5;

    }

    void hideNotification(string chatId)
    {

        for (int i = usersContainer.transform.childCount - 1; i >= 0; i--)
        {
            if (chatId == usersContainer.transform.GetChild(i).GetChild(4).transform.GetComponent<Text>().text)
            {
                usersContainer.transform.GetChild(i).GetChild(0).transform.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f); 

            }
        }

    }

    void hideAllNotfications()
    {
        for (int i = usersContainer.transform.childCount - 1; i >= 0; i--)
        {
            usersContainer.transform.GetChild(i).GetChild(0).transform.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);                        
        }
    }

    void showNotification(string chatId)
    {

        for (int i = usersContainer.transform.childCount - 1; i >= 0; i--)
        {
            if (chatId == usersContainer.transform.GetChild(i).GetChild(4).transform.GetComponent<Text>().text)
            {
                usersContainer.transform.GetChild(i).GetChild(0).transform.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    void clearChat()
    {
        for (int i = chatContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(chatContainer.transform.GetChild(i).gameObject);
        }
        chatContainer.GetComponent<RectTransform>().sizeDelta = initialChatSize;
    }

    void clearSearchResults()
    {
        for (int i = searchContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(searchContainer.transform.GetChild(i).gameObject);
        }
        searchContainer.GetComponent<RectTransform>().sizeDelta = initialSearchSize;
    }

    void clearUserChats()
    {
        for (int i = usersContainer.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(usersContainer.transform.GetChild(i).gameObject);
        }
        usersContainer.GetComponent<RectTransform>().sizeDelta = initialUsersSize;
    }

    float getContainerHeight(GameObject container)
    {
        return container.GetComponent<RectTransform>().rect.height;
    }

    void setMessageContainerSize(int charsCount, GameObject container)
    {
        Vector2 containerSize = container.GetComponent<RectTransform>().sizeDelta;

        float width = container.GetComponent<RectTransform>().rect.width;
        float height = container.GetComponent<RectTransform>().rect.height;

        height = (charsCount * 1.10F) + 40.00F;

        if (height <= 90.00F)
            height = 120.00F;

        container.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    bool chatExists()
    {
        foreach (KeyValuePair<string, JSONNode> entry in userChats)
        {
            if ( 
                (entry.Value["chat"]["firstUserId"].ToString() == MainScene.userId && entry.Value["chat"]["secondUserId"].ToString() == choosenUserId) 
                || (entry.Value["chat"]["firstUserId"].ToString() == choosenUserId && entry.Value["chat"]["secondUserId"].ToString() == MainScene.userId)
            )
            {
                chatId = entry.Value["chat"]["id"].ToString();
                return true;
            }
        }
        return false;
    }

    void createNewChat()
    {
        Chat chat = new Chat();
        chat.firstUserId = MainScene.userId;
        chat.secondUserId = choosenUserId;
        StartCoroutine(Web.PostRequest(Web.CREATE_PRIVATE_CHAT + MainScene.token, JsonUtility.ToJson(chat), Web.REQUEST.CREATE_NEW_CHAT));
    }

    void getStatus()
    {
        StartCoroutine(Web.GetRequest(Web.GET_STATUS + MainScene.token, Web.REQUEST.GET_STATUS));
    }

    void setStatus()
    {
        if (endUUID != null)
        {
            StartCoroutine(Web.PutRequest(Web.setStatus(chatId, endUUID, MainScene.token), "empty", Web.REQUEST.SEND_CHAT_STATUS));
        }
    }

    void loadMessages()
    {
        _messages.Clear();
        hideNotification(chatId);
        loadMessagesHistory(chatId);
        StartCoroutine(Web.GetRequest(Web.getMessages(chatId, MainScene.token, startUUID, null), Web.REQUEST.GET_MESSAGES));
    }

    void loadMessagesHistory(string chatId)
    {

        if (Directory.Exists("Chats") && chatId.Length > 0)
        {
            if (File.Exists(@"Chats/chat_" + chatId.ToString() + ".json"))
            {
                string[] lines = File.ReadAllLines(@"Chats/chat_" + chatId.ToString() + ".json");

                for (int i = 0; i < lines.Length; i++)
                {
                    JSONNode data = SimpleJSON.JSON.Parse(lines[i]);

                    data["chatId"] = Security.DecryptString(data["chatId"]);
                    data["userId"] = Security.DecryptString(data["userId"]);
                    data["content"] = Security.DecryptString(data["content"]);
                    data["uuid"] = Security.DecryptString(data["uuid"]);

                    _messages[data["uuid"]] = data;
                    startUUID = data["uuid"];
                }
            }
        }
    }

    void saveMessagesHistory()
    {

        List<historyMessage> messagesToSave = new List<historyMessage>();

        if (!Directory.Exists("Chats"))
        {
            Directory.CreateDirectory("Chats");
        }

        if (File.Exists(@"Chats/chat_" + chatId.ToString() + ".json"))
        {
            File.WriteAllText(@"Chats/chat_" + chatId.ToString() + ".json", string.Empty);
        }

        foreach (KeyValuePair<string, JSONNode> entry in _messages)
        {

            historyMessage message = new historyMessage();

            message.chatId = Security.EncryptString(entry.Value["chatId"]);
            message.userId = Security.EncryptString(entry.Value["userId"]);
            message.content = Security.EncryptString(entry.Value["content"]);
            message.uuid = Security.EncryptString(entry.Value["uuid"]);

            var q = JsonUtility.ToJson(message);

            File.AppendAllText(@"Chats/chat_" + chatId.ToString() + ".json", q + Environment.NewLine);
        }

        loadMessagesHistory(chatId);
    }

    public void setCurrentMessage()
    {
        currentMessage = _messageInput.text;
    }

    public void Search()
    {

        if (_searchInput.text.Length > 0)
        {
            StartCoroutine(Web.GetRequest(Web.findUser(_searchInput.text.ToString(), MainScene.token, null, null), Web.REQUEST.GET_SEARCH_RESULT));
        }
        else
        {
            clearSearchResults();
        }
    }

    public void closeSearchPanel()
    {
        search.gameObject.SetActive(false);
        clearSearchResults();
    }

    public void Back()
    {
        SceneManager.LoadScene(1);
    }


}
