using System.Collections;
using System.Collections.Generic;
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

    public void updateInfo(HelpDetailsInfo info){
      topic.text = info.topic;;
      requester.text = "Requested by " + info.requester;

      // Send a request to get description
      EntityManager clientManager = FindFirstObjectByType<ClientManager>().GetEntityManager();
      Entity getHelpDescriptionRequest = clientManager.CreateEntity(typeof(GetHelpDescriptionRpc), typeof(SendRpcCommandRequest));
      clientManager.SetComponentData(getHelpDescriptionRequest, new GetHelpDescriptionRpc{id = info.guid});

      description.text = info.description;
    }

    public void CloseDetailsDisplay(){
      if (helpDetails.activeSelf) helpDetails.SetActive(false);
      if (!helpList.activeSelf) helpList.SetActive(true);
    }
}
