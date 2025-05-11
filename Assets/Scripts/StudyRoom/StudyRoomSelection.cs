using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class StudyRoomSelection : MonoBehaviour
{
    [SerializeField] private GameObject joinRoomObject;
    [SerializeField] private GameObject createRoomObject;
    [SerializeField] private  List<OpenRoom> allRooms = new List<OpenRoom>();
    [SerializeField] private TMP_InputField joinPrivateRoomInput;
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private GameObject publicRoomBtnPrefab;
    [SerializeField] private Transform roomListContent;
    private bool createRoomPrivate;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: get allRooms from database
        // TODO: load buttons based on allRooms
        foreach(OpenRoom curRoom in allRooms){
            AddRoomToScrollView(curRoom);
        }
    }

    public void AddRoomToScrollView(OpenRoom newRoom)
    {
        if(newRoom.isPublic){
            GameObject newRoomObject = Instantiate(publicRoomBtnPrefab, roomListContent);
            PublicRoomSelectButton roomInfo = newRoomObject.GetComponent<PublicRoomSelectButton>();
            roomInfo.displayRoomName(newRoom.roomName);
            roomInfo.setRoomId(newRoom.roomID);
            newRoomObject.transform.localScale = Vector3.one; 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectJoinRoomMode(){
        if (joinRoomObject != null && !joinRoomObject.activeSelf) joinRoomObject.SetActive(true);
        if (createRoomObject != null && createRoomObject.activeSelf) createRoomObject.SetActive(false);
    }

    public void selectCreateRoomMode(){
        if (joinRoomObject != null && joinRoomObject.activeSelf) joinRoomObject.SetActive(false);
        if (createRoomObject != null && !createRoomObject.activeSelf) createRoomObject.SetActive(true);
    }

    public void createRoom(){
        // note: assumes allRooms is stored in order of roomID
        // TODO: generate random id instead of just using next id
        // TODO: input validation
        if (createRoomNameInput.text == ""){
            return;
        }
        int newRoomId = ((allRooms.Count - 1) >= 0) ? allRooms[allRooms.Count - 1].roomID + 1 : 0;
        OpenRoom newRoom = new OpenRoom(createRoomPrivate, createRoomNameInput.text, newRoomId);
        allRooms.Add(newRoom);
        AddRoomToScrollView(newRoom);
        // TODO: change user to that room
        // TODO: if private, let them know what study room ID is once they enter
    }

    public void setRoomCreationTypePublic(bool newType){
        createRoomPrivate = newType;
        // TODO: change color
    }

    public bool getCreateRoomPrivate(){
        return createRoomPrivate;
    }

    public List<OpenRoom> getAllRooms(){
        return allRooms;
    }

    public void clearAllRoom(){
        allRooms = new List<OpenRoom>();
    }

    public void setCreateRoomNameInput(string text){
        createRoomNameInput.text = text;
    }

    public void closeStudyRoomSelection(){
        gameObject.SetActive(false);
    }
}

[System.Serializable] 
public class OpenRoom {
    public bool isPublic;
    public string roomName;
    public int roomID;

    public OpenRoom(bool isPublic, string roomName, int roomID)
    {
        this.isPublic = isPublic;
        this.roomName = roomName;
        this.roomID = roomID;
    }
}