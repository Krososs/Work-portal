using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;


public class WorkersScene : MonoBehaviour
{
    //web
    public static bool newRequest;
    public static Web.REQUEST _requestType;
    public static string requestMessage;

    //user options panel
    public Button addWorkerButton;
    public Button removeWorkerButton;
    public Button editWorkerButton;
    public Button createDepartmentButton;
    public Button setHeadButton;
    public Button deleteDepartmentButton;
    public Button createCompanyButton;
    public Button setCompanyOwnerButton;
    public Button deleteCompanyButton;

    //new worker panel
    public InputField firstName;
    public InputField surname;
    public InputField email;
    public InputField password;
    public GameObject newWorkerPanel;
    public Button newWorkerButton;
    public Dropdown departmentsDropdown;
    public Dropdown roleDropdown;

    //edit worker panel
    public InputField _firstName;
    public InputField _surname;
    public InputField _email;
    public GameObject editWorkerPanel;
    public Dropdown usersDropdown;

    //remove worker panel
    public GameObject removeWorkerPanel;
    public Dropdown _usersDropdown;
    public Button _removeWorkerButton;

    //new department panel
    public GameObject newDepartmentPanel;
    public Dropdown companiesDropdown;
    public InputField departmentName;

    //department head panel
    public GameObject departmentHeadPanel;
    public Dropdown _departmentsDropdown;
    public Dropdown __usersDropdown;

    //delete department panel
    public GameObject deleteDepartmentPanel;
    public Dropdown __departmentsDropdown;

    //create company panel
    public GameObject createCompanyPanel;
    public InputField companyName;

    //company owenr panel
    public GameObject companyOwnerPanel;
    public Dropdown _companiesDropdown;
    public Dropdown ___usersDropdown;

    //delete company panel
    public GameObject deleteCompanyPanel;
    public Dropdown __companiesDropdown;

   //global
    Dictionary<int, string> companies = new Dictionary<int, string>();
    Dictionary<int, string> companyDepartments = new Dictionary<int, string>();
    Dictionary<int, string> workers = new Dictionary<int, string>();
    Dictionary<int, string> roles= new Dictionary<int, string>()
    {
        [1] = "Company owner",
        [2] = "Head of department" ,
        [3] = "Worker"
    };

    [Serializable]   
    public class User
    {
        public string firstName=null;
        public string surname=null; 
        public string email=null; 
        public string password=null;
        public string language="Polish"; 

    }
    [Serializable]
    public class Department
    {
        public string companyId;
        public string name;
    }

    void Start()
    {       
        hideAll();
        showButtons();
        getCompanyDepartments();      
    }
 
    void Update()
    {      
        if(newRequest){
            newRequest=false;
            ProcessNewRequest(_requestType,requestMessage);
        }
        checkNewUserForm();    
    }

    public static void setRequestInfo(Web.REQUEST type, string rawRespone){
        newRequest=true;
        requestMessage=rawRespone;
        _requestType=type;
    }
    
    private void hideAll(){
        //panels
        newWorkerPanel.gameObject.SetActive(false);
        editWorkerPanel.gameObject.SetActive(false);
        removeWorkerPanel.gameObject.SetActive(false);
        newDepartmentPanel.gameObject.SetActive(false);
        departmentHeadPanel.gameObject.SetActive(false);
        deleteDepartmentPanel.gameObject.SetActive(false);
        createCompanyPanel.gameObject.SetActive(false);
        companyOwnerPanel.gameObject.SetActive(false);
        deleteCompanyPanel.gameObject.SetActive(false);
        //buttons
        addWorkerButton.gameObject.SetActive(false);
        removeWorkerButton.gameObject.SetActive(false);
        editWorkerButton.gameObject.SetActive(false);

        createDepartmentButton.gameObject.SetActive(false);
        setHeadButton.gameObject.SetActive(false);
        deleteDepartmentButton.gameObject.SetActive(false);

        createCompanyButton.gameObject.SetActive(false);
        setCompanyOwnerButton.gameObject.SetActive(false);
        deleteCompanyButton.gameObject.SetActive(false);

    }

    private void showButtons(){

        if(MainScene.isAdmin){
            showAllButtons();              
        }
        if(MainScene.userRole==1){

            addWorkerButton.gameObject.SetActive(true);
            removeWorkerButton.gameObject.SetActive(true);
            editWorkerButton.gameObject.SetActive(true);

            createDepartmentButton.gameObject.SetActive(true);
            setHeadButton.gameObject.SetActive(true);
            deleteDepartmentButton.gameObject.SetActive(true);
        }        
    }

    private void showAllButtons(){
        addWorkerButton.gameObject.SetActive(true);
        removeWorkerButton.gameObject.SetActive(true);
        editWorkerButton.gameObject.SetActive(true);

        createDepartmentButton.gameObject.SetActive(true);
        setHeadButton.gameObject.SetActive(true);
        deleteDepartmentButton.gameObject.SetActive(true);

        createCompanyButton.gameObject.SetActive(true);
        setCompanyOwnerButton.gameObject.SetActive(true);
        deleteCompanyButton.gameObject.SetActive(true);
    }

    private void getCompanyDepartments(){
        StartCoroutine(Web.GetRequest(Web.getCompanyDepartments(MainScene.token, MainScene.companyId.ToString()),Web.REQUEST.GET_COMPANY_DEPARTMENTS));
       
    }

    private int getDepartmentId(string departmentName){

        foreach (KeyValuePair<int, string> entry in companyDepartments){
            
            if(entry.Value==departmentName)
                return entry.Key;
        }
        return 0;
    }

    private int getRoleId(string role){

        foreach (KeyValuePair<int, string> entry in roles){
            
            if(entry.Value==role)
                return entry.Key;
        }
        return 0;
    }

    private int getUserId(string fullName){

        foreach (KeyValuePair<int, string> entry in workers){
            
            if(entry.Value==fullName)
                return entry.Key;
        }
        return 0;
    }

    private int getCompanyId(string name){
        foreach (KeyValuePair<int, string> entry in companies){
            
            if(entry.Value==name)
                return entry.Key;
        }
        return 0;
    }

    private void ProcessNewRequest(Web.REQUEST type, string rawRespone){

        switch(type){
            case Web.REQUEST.CREATE_USER:
                ProcessNewUserResponse(rawRespone);
                break;
            case Web.REQUEST.CREATE_DEPARTMENT:
                ProcessNewDepartmentResponse(rawRespone);
                break;
            case Web.REQUEST.GET_COMPANY_DEPARTMENTS:
                ProcessCompanyDepartments(rawRespone);
                break;
            case Web.REQUEST.GET_USERS:
                ProcessUsersResponse(rawRespone);
                break;
            case Web.REQUEST.GET_GIVEN_USER_INFO:
                ProcessUserInfo(rawRespone);
                break;
            case Web.REQUEST.EDIT_USER:
                ProcessEditedUser(rawRespone);
                break;
            case Web.REQUEST.DELETE_USER:
                ProcessDeletedUser(rawRespone);
                break;
            case Web.REQUEST.GET_COMPANIES:
                ProcessCompanies(rawRespone);
                break;
            case Web.REQUEST.SET_HEAD_OF_DEPARTMENT:
                ProcessHeadOfDepartment(rawRespone);
                break;
            case Web.REQUEST.DELETE_DEPARTMENT:
                ProcessDeletedDepartment(rawRespone);
                break;
            case Web.REQUEST.CREATE_COMPANY:
                ProcessNewCompany(rawRespone);
                break;
            case Web.REQUEST.SET_COMPANY_OWNER:
                ProcessCompanyOwner(rawRespone);
                break;
            case Web.REQUEST.DELETE_COMPANY:
                ProcessDeletedCompany(rawRespone);
                break;
            default:
                Debug.LogError("Wrong request type!");
                break; 
        }

    }

    private void ProcessDeletedCompany(string rawResponse){
    }

    private void ProcessCompanyOwner(string rawResponse){
    }

    private void ProcessNewCompany(string rawResponse){
    }

    private void ProcessDeletedDepartment(string rawResponse){
    }
    private void ProcessHeadOfDepartment(string rawResponse){
    }

    private void ProcessNewDepartmentResponse(string rawResponse){
        getCompanyDepartments();
    }

    private void ProcessCompanies(string rawResponse){      
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){
            companies[entry.Value["id"]]=entry.Value["name"];
        }
        setCompanies(companiesDropdown,companies);
        setCompanies(_companiesDropdown,companies);
        setCompanies(__companiesDropdown,companies);
    }

    private void ProcessDeletedUser(string rawResponse){
    }

    private void ProcessEditedUser(string rawResponse){
    }


    private void ProcessUserInfo(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);       
        _firstName.placeholder.GetComponent<Text>().text =data["result"]["firstName"];
        _surname.placeholder.GetComponent<Text>().text =data["result"]["surname"];
        _email.placeholder.GetComponent<Text>().text =data["result"]["email"];
    }

    private void ProcessUsersResponse(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);
        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){
            workers[entry.Value["id"]]=entry.Value["firstName"]+" "+entry.Value["surname"];
        }
        setUsers(usersDropdown, workers);
        setUsers(_usersDropdown, workers);
        setUsers(__usersDropdown, workers);
        setUsers(___usersDropdown, workers);
        setUserPlaceholders(0);
    }
    
    private void ProcessNewUserResponse(string rawResponse){
    }

    private void ProcessCompanyDepartments(string rawResponse){
        JSONNode data = SimpleJSON.JSON.Parse(rawResponse);

        foreach(KeyValuePair<string,JSONNode> entry in data["result"]){
            companyDepartments[entry.Value["id"]]=entry.Value["name"];    
        }
        setDepartments(departmentsDropdown,companyDepartments);
        setDepartments(_departmentsDropdown,companyDepartments);
        setDepartments(__departmentsDropdown,companyDepartments);      
        setRoles(roleDropdown);   
    }

    private void setCompanies(Dropdown dropdown, Dictionary<int, string> _companies){
        List<string> values = new List<string>();

        foreach (KeyValuePair<int, string> entry in _companies){
            values.Add(entry.Value);
        }  
        dropdown.ClearOptions();
        dropdown.AddOptions(values);
    }

    private void setUsers(Dropdown dropdown, Dictionary<int, string> _workers){
        List<string> values = new List<string>();

        foreach (KeyValuePair<int, string> entry in _workers){
            values.Add(entry.Value);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(values);     
    }

    public void setUserPlaceholders(int value){
        string userFullName=usersDropdown.options[usersDropdown.value].text;
        string userId = getUserId(userFullName).ToString();

        StartCoroutine(Web.GetRequest(Web.getGivenUserInfo(MainScene.token,userId),Web.REQUEST.GET_GIVEN_USER_INFO));
    }

    private void setRoles(Dropdown dropdown){
        List<string> values = new List<string>();

        if(MainScene.userRole==1){
            foreach (KeyValuePair<int, string> entry in roles){
                values.Add(entry.Value);
            }
        }else{
            values.Add("Worker");
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(values);

    }

    private void setDepartments(Dropdown dropdown, Dictionary<int, string> departments){

        List<string> values = new List<string>();
        foreach (KeyValuePair<int, string> entry in departments){
            values.Add(entry.Value);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(values);
    }

    private void closePanels(){
        newWorkerPanel.gameObject.SetActive(false);
        editWorkerPanel.gameObject.SetActive(false);
        removeWorkerPanel.gameObject.SetActive(false);
        newDepartmentPanel.gameObject.SetActive(false);
        departmentHeadPanel.gameObject.SetActive(false);
        deleteDepartmentPanel.gameObject.SetActive(false);
        createCompanyPanel.gameObject.SetActive(false);
        companyOwnerPanel.gameObject.SetActive(false);
        deleteCompanyPanel.gameObject.SetActive(false);
    }

    //NewWorkerPanel
    public void showNewWorkerPanel(){
        closePanels();
        newWorkerPanel.gameObject.SetActive(true);

    }
    public void closeNewWorkerPanel(){
        newWorkerPanel.gameObject.SetActive(false);
    }
    private void checkNewUserForm(){
        if(firstName.text.Length>0 && surname.text.Length>0 && email.text.Length>0 && password.text.Length>0)
            newWorkerButton.interactable=true;
        else
            newWorkerButton.interactable=false;
    }
    public void createUser(){
        User user = new User();
        user.firstName=firstName.text;         
        user.surname=surname.text;
        user.email=email.text;
        user.password=password.text;
                            
        string depName=departmentsDropdown.options[departmentsDropdown.value].text;
        string roleName= roleDropdown.options[roleDropdown.value].text;

        StartCoroutine(Web.PutRequest(Web.createUser(MainScene.token, MainScene.companyId.ToString(),getDepartmentId(depName).ToString(),getRoleId(roleName).ToString()), JsonUtility.ToJson(user), Web.REQUEST.CREATE_USER));
    }

    //EditWorkerPanel
    public void showEditWorkerPanel(){
        closePanels();
        editWorkerPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_USERS+MainScene.token,Web.REQUEST.GET_USERS));
    }
    public void closeEditWorkerPanel(){
        editWorkerPanel.gameObject.SetActive(false);
    }
    public void editUser(){

        User user = new User();

        if(_firstName.text.Length>0){
            user.firstName=_firstName.text;
        }
        else{
             user.firstName=_firstName.placeholder.GetComponent<Text>().text;
        }
        if(_surname.text.Length>0){
            user.surname=_surname.text;
        }   
        else{
            user.surname=_surname.placeholder.GetComponent<Text>().text;
        }
        if(_email.text.Length>0){
            user.email=_email.text;
        }
        else{
            user.email=_email.placeholder.GetComponent<Text>().text;
        }

        string userFullName=usersDropdown.options[usersDropdown.value].text;
        string userId = getUserId(userFullName).ToString();

        StartCoroutine(Web.PatchRequest(Web.editUser(MainScene.token, userId), JsonUtility.ToJson(user), Web.REQUEST.EDIT_USER));
    }

    //RemoveWorkerPanel
    public void removeWorker(){
        string userFullName=_usersDropdown.options[_usersDropdown.value].text;
        string userId = getUserId(userFullName).ToString();
        StartCoroutine(Web.DeleteRequest(Web.deleteUser(MainScene.token,userId ), "e", Web.REQUEST.DELETE_USER));
    }
    public void showRemoveWorkerPanel(){
        closePanels();
        removeWorkerPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_USERS+MainScene.token,Web.REQUEST.GET_USERS));
    }
    public void closeRemoveWorkerPanel(){
        removeWorkerPanel.gameObject.SetActive(false);
    }

    // new department panel
    public void createDepartment(){

        string name=companiesDropdown.options[companiesDropdown.value].text;
        string companyId = getCompanyId(name).ToString();

        string depName = departmentName.text;

        Department department = new Department();
        department.companyId=companyId;
        department.name=depName;

        StartCoroutine(Web.PostRequest(Web.CREATE_DEPARTMENT+MainScene.token,JsonUtility.ToJson(department),Web.REQUEST.CREATE_DEPARTMENT));
    }
    public void showNewDepartmentPanel(){
        closePanels();
        newDepartmentPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_COMPANIES+MainScene.token,Web.REQUEST.GET_COMPANIES));
    }
    public void closeNewDepartmentPanel(){
        newDepartmentPanel.gameObject.SetActive(false);      
    }

    // department head panel
    public void setDepartmentHead(){

        string userFullName=_usersDropdown.options[_usersDropdown.value].text;
        string userId = getUserId(userFullName).ToString();

        string depName=_departmentsDropdown.options[_departmentsDropdown.value].text;
        string departamentId=getDepartmentId(depName).ToString();
  
        StartCoroutine(Web.PutRequest(Web.setHeadOfDepartment(MainScene.token,MainScene.companyId.ToString(),departamentId,userId),"e",Web.REQUEST.SET_HEAD_OF_DEPARTMENT));
    }

    public void showDepartmentHeadPanel(){
        closePanels();
        departmentHeadPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_USERS+MainScene.token,Web.REQUEST.GET_USERS));
    }

    public void closeDepartmentHeadPanel(){
        departmentHeadPanel.gameObject.SetActive(false);        
    }

    // delete department panel
    public void deleteDepartment(){
        string depName=__departmentsDropdown.options[__departmentsDropdown.value].text;
        string departamentId=getDepartmentId(depName).ToString();
        StartCoroutine(Web.DeleteRequest(Web.deleteDepartment(MainScene.token,departamentId ), "e", Web.REQUEST.DELETE_DEPARTMENT));
    }
    public void showDeleteDepartmentPanel(){
        closePanels();
        deleteDepartmentPanel.gameObject.SetActive(true);
    }
    public void closeDeleteDepartmentPanel(){
        deleteDepartmentPanel.gameObject.SetActive(false);
    }

    // create company panel
    public void createCompany(){
        string name = companyName.text;
        StartCoroutine(Web.PostRequest(Web.createCompany(MainScene.token,name ), "e", Web.REQUEST.CREATE_COMPANY));    
    }
    public void showCreateCompanyPanel(){
        closePanels();
        createCompanyPanel.gameObject.SetActive(true);
    }
    public void closeCreateCompanyPanel(){
        createCompanyPanel.gameObject.SetActive(false);
    }

    //company owner panel
    public void setCompanyOwner(){
        string name=_companiesDropdown.options[_companiesDropdown.value].text;
        string companyId = getCompanyId(name).ToString();
 
        string userFullName=___usersDropdown.options[___usersDropdown.value].text;
        string userId = getUserId(userFullName).ToString();

        StartCoroutine(Web.PutRequest(Web.setCompanyOwner(MainScene.token,companyId,userId ), "e", Web.REQUEST.SET_COMPANY_OWNER));
    }
    public void showCompanyOwnerPanel(){
        closePanels();
        companyOwnerPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_USERS+MainScene.token,Web.REQUEST.GET_USERS));
        StartCoroutine(Web.GetRequest(Web.GET_COMPANIES+MainScene.token,Web.REQUEST.GET_COMPANIES));
    }
    public void closeCompanyOwnerPanel(){
        companyOwnerPanel.gameObject.SetActive(false);
    }

    //delete company panel
    public void deleteCompany(){
        string name=__companiesDropdown.options[__companiesDropdown.value].text;
        string companyId = getCompanyId(name).ToString();
        StartCoroutine(Web.DeleteRequest(Web.deleteCompany(MainScene.token,companyId ), "e", Web.REQUEST.DELETE_COMPANY));
    }

    public void showDeleteCompanyPanel(){
        closePanels();
        deleteCompanyPanel.gameObject.SetActive(true);
        StartCoroutine(Web.GetRequest(Web.GET_COMPANIES+MainScene.token,Web.REQUEST.GET_COMPANIES));
    }

    public void closeDeleteCompanyPanel(){
        deleteCompanyPanel.gameObject.SetActive(false); 
    }
 
    public void Back(){
        SceneManager.LoadScene(1);
    }
  
}
