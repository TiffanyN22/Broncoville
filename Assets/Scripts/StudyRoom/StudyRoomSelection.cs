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
    [SerializeField] private GameObject privateRoomCreatedPopup;
    [SerializeField] private List<OpenRoom> allRooms = null;
    [SerializeField] private TMP_InputField joinPrivateRoomInput;
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private GameObject publicRoomBtnPrefab;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private TMP_Text createdPrivateRoomIdDisplay;
    private bool createRoomPublic;

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
        if (newRoom.isPublic)
        {
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
        refreshRoomListFromServer();
        if (joinRoomObject != null && !joinRoomObject.activeSelf) joinRoomObject.SetActive(true);
        if (createRoomObject != null && createRoomObject.activeSelf) createRoomObject.SetActive(false);
    }

    public void selectCreateRoomMode()
    {
        if (joinRoomObject != null && joinRoomObject.activeSelf) joinRoomObject.SetActive(false);
        if (createRoomObject != null && !createRoomObject.activeSelf) createRoomObject.SetActive(true);
    }

    public void createRoom()
    {
        // input validation
        if (createRoomNameInput.text == "" || createRoomNameInput.text.Length > 125)
        {
            return;
        }

        // get random 4-digit id if private
        int privateRoomId = 0;
        if (!createRoomPublic)
        {
            do
            {
                privateRoomId = UnityEngine.Random.Range(1000, 10000);
            }
            while (allRooms.Exists(curRoom => curRoom.room4NumID == privateRoomId));

            if (!privateRoomCreatedPopup.activeSelf) privateRoomCreatedPopup.SetActive(true);
            createdPrivateRoomIdDisplay.text = privateRoomId.ToString();
        }

        // OpenRoom newRoom = new OpenRoom(createRoomPublic, createRoomNameInput.text, privateRoomId, Guid.NewGuid());

        // Update Server
        EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
        Entity createStudyRoomRequest = clientManager.CreateEntity(typeof(CreateStudyRoomSelectRpc), typeof(SendRpcCommandRequest));
        string newGuid = Guid.NewGuid().ToString();
        clientManager.SetComponentData(createStudyRoomRequest, new CreateStudyRoomSelectRpc { isPublic = createRoomPublic, roomName = createRoomNameInput.text, room4NumID = privateRoomId, roomGuid = newGuid });

        createRoomNameInput.text = "";
    }

    public void setRoomCreationTypePublic(bool newType)
    {
        createRoomPublic = newType;
        // TODO: change color
    }

    public bool getcreateRoomPublic()
    {
        return createRoomPublic;
    }

    public List<OpenRoom> getAllRooms()
    {
        return allRooms;
    }

    public void clearAllRoom()
    {
        allRooms = new List<OpenRoom>();
    }

    public void setCreateRoomNameInput(string text)
    {
        createRoomNameInput.text = text;
    }

    public void closeStudyRoomSelection()
    {
        gameObject.SetActive(false);
    }

    public void closePrivateRoomPopup()
    {
        if (privateRoomCreatedPopup.activeSelf) privateRoomCreatedPopup.SetActive(false);
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