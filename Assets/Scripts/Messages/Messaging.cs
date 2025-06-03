using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
public class Messaging : MonoBehaviour
{
  string username = "Unknown";
  [SerializeField] private TMP_InputField inputField;
  [SerializeField] private  GameObject textItemPrefab;
  [SerializeField] private  GameObject messageDisplayGroup;
  [SerializeField] private  GameObject noMessageDisplayGroup;
  [SerializeField] private  Transform messagesScrollViewContent;

  void Start()
  {
    EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
    EntityQuery query = entities.CreateEntityQuery(typeof(NetworkId));
    NativeArray<NetworkId> accounts = query.ToComponentDataArray<NetworkId>(Allocator.Temp);

    if (accounts.Length == 0)
    {
        Debug.Log("Faile to find NetworkId");
        return;
    }

    int id = accounts[0].Value;

    query = entities.CreateEntityQuery(typeof(GhostOwner));
    NativeArray<Entity> players = query.ToEntityArray(Allocator.Temp);

    for (int i = 0; i < players.Length; ++i)
    {
        GhostOwner ghost = entities.GetComponentData<GhostOwner>(players[i]);

        if (ghost.NetworkId == id)
        {
            AccountData account = entities.GetComponentData<AccountData>(players[i]);
            username = account.name.ToString();
            break;
        }
    }

    inputField.onSubmit.AddListener(SendMessage);
  }

  void SendMessage(string message)
  {
    if (message.Length > 125)
    {
      inputField.textComponent.color = Color.red;
      return;
    }
    string fullMessage = $"[{username}]: {message}";

    // Send message to server
     EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
    Entity sendMessageEntity = clientManager.CreateEntity(typeof(MessagingSendMessageRpc), typeof(SendRpcCommandRequest));
    clientManager.SetComponentData(sendMessageEntity, new MessagingSendMessageRpc { message = fullMessage });


    inputField.textComponent.color = Color.white;
    inputField.text = "";
  }

  public void UpdateMessageDisplay(bool display)
  {
    messageDisplayGroup.SetActive(display);
    noMessageDisplayGroup.SetActive(!display);
  }

  public void AddMessage(string message)
  {
    // Debug.Log("Adding message " + message);
    GameObject newText = Instantiate(textItemPrefab, messagesScrollViewContent);
    newText.GetComponentInChildren<TMP_Text>().text = message;
  }
}
