using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public class StudyRoomSelection : MonoBehaviour
{
    [SerializeField] private GameObject joinRoomObject;
    [SerializeField] private GameObject createRoomObject;
    [SerializeField] private  List<OpenRoom> allRooms = null;
    [SerializeField] private TMP_InputField joinPrivateRoomInput;
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private GameObject publicRoomBtnPrefab;
    [SerializeField] private Transform roomListContent;
    private bool createRoomPrivate;

    // Start is called before the first frame update
    void Start()
    {
        allRooms = new List<OpenRoom>();
        // TODO: get allRooms from database
        refreshRoomListFromServer();
        // TODO: load buttons based on allRooms
        
    }

    public void AddRoomToScrollView(OpenRoom newRoom)
    {
        if(newRoom.isPublic){
            GameObject newRoomObject = Instantiate(publicRoomBtnPrefab, roomListContent);
            PublicRoomSelectButton roomInfo = newRoomObject.GetComponent<PublicRoomSelectButton>();
            roomInfo.displayRoomName(newRoom.roomName);
            roomInfo.setRoomId(newRoom.room4NumID);
            newRoomObject.transform.localScale = Vector3.one; 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for entities containing the StudyRoomSelectEntryRpc.
        EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
        EntityQuery entries = entities.CreateEntityQuery(ComponentType.ReadOnly<StudyRoomSelectEntryRpc>());

        foreach (Entity entity in entries.ToEntityArray(Allocator.Temp))
        {
            StudyRoomSelectEntryRpc response = entities.GetComponentData<StudyRoomSelectEntryRpc>(entity);


            bool isPublic = response.isPublic;
            string roomName = response.roomName.ToString();
            int room4NumID = response.room4NumID;
            Guid roomGuid = Guid.Parse(response.roomGuid.ToString());

            (allRooms ??= new List<OpenRoom>()).Add(new OpenRoom(isPublic, roomName, room4NumID, roomGuid));

            refreshRoomList();
            entities.DestroyEntity(entity);
        }
    }
    
    public void refreshRoomListFromServer()
    {
        allRooms = null;
        refreshRoomList(); 

        // Send a get study room select request to the server.
        EntityManager clientManager = GameObject.FindFirstObjectByType<ClientManager>().GetEntityManager();
        Entity getHelp = clientManager.CreateEntity(typeof(GetStudyRoomSelectRpc), typeof(SendRpcCommandRequest));
    }

    private void refreshRoomList()
    {
        // delete previous items
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // add rooms from list
        if (allRooms == null) return;
        foreach (OpenRoom curRoom in allRooms)
        {
            AddRoomToScrollView(curRoom);
        }
    }

    public void selectJoinRoomMode()
    {
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
        int newRoomId = ((allRooms.Count - 1) >= 0) ? allRooms[allRooms.Count - 1].room4NumID + 1 : 0;
        OpenRoom newRoom = new OpenRoom(createRoomPrivate, createRoomNameInput.text, newRoomId, Guid.NewGuid());
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
    public int room4NumID;
    public Guid roomGuid;

    public OpenRoom(bool isPublic, string roomName, int roomID, Guid guid)
    {
        this.isPublic = isPublic;
        this.roomName = roomName;
        this.room4NumID = roomID;
        this.roomGuid = guid;
    }
}