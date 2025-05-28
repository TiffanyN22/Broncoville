using System.Collections;
using System.Collections.Generic;
using System.Text; 
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using TMPro;
using UnityEngine;

public class HelpDetailsDisplay : MonoBehaviour
{
  [SerializeField] private TMP_Text topic; 
  [SerializeField] private TMP_Text requester; 
  [SerializeField] private TMP_Text description; 
  [SerializeField] private GameObject helpDetails;
  [SerializeField] private GameObject helpList;
  private bool waitingForDescription = false;
  private string[] descriptionParts = null;
  private int numDescriptionReceived = 0;

  public void updateInfo(HelpDetailsInfo info)
  {
    topic.text = info.topic; ;
    requester.text = "Requested by " + info.requester;

    // Send a request to get description
    EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
    Entity getHelpDescriptionRequest = clientManager.CreateEntity(typeof(GetHelpDescriptionRpc), typeof(SendRpcCommandRequest));
    clientManager.SetComponentData(getHelpDescriptionRequest, new GetHelpDescriptionRpc { id = info.guid.ToString() });

    waitingForDescription = true;
  }

  void Update()
  {
    if (!waitingForDescription) return;

    EntityManager entities = FindFirstObjectByType<ClientManager>().GetEntityManager();
    EntityQuery entries = entities.CreateEntityQuery(ComponentType.ReadOnly<HelpBoardEntryDescriptionRpc>());

    foreach (Entity entity in entries.ToEntityArray(Allocator.Temp))
    {
      HelpBoardEntryDescriptionRpc response = entities.GetComponentData<HelpBoardEntryDescriptionRpc>(entity);

      if (descriptionParts == null)
      {
        descriptionParts = new string[response.descriptionNumPackets];
        numDescriptionReceived = 0;
      }
      descriptionParts[response.index] = response.description.ToString();
      ++numDescriptionReceived;
      entities.DestroyEntity(entity);
    }

    if (descriptionParts != null && numDescriptionReceived == descriptionParts.Length)
    {
      StringBuilder s = new StringBuilder(descriptionParts.Length * 125 + 1);
      foreach (string curString in descriptionParts)
      {
        s.Append(curString);
      }
      description.text = s.ToString();
      waitingForDescription = false;
      descriptionParts = null;
    }

  }

  public void CloseDetailsDisplay()
  {
    if (helpDetails.activeSelf) helpDetails.SetActive(false);
    if (!helpList.activeSelf) helpList.SetActive(true);
  }
}
