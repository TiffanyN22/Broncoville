using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpDetailsDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text topic; 
    [SerializeField] private TMP_Text requester; 
    [SerializeField] private TMP_Text description; 
    [SerializeField] private GameObject helpDetails;
    [SerializeField] private GameObject helpList;

    public void updateInfo(string topicText, string requesterText, string descriptionText){
      topic.text = topicText;
      requester.text = "Requested by " + requesterText;
      description.text = descriptionText;
    }

    public void CloseDetailsDisplay(){
      if (helpDetails.activeSelf) helpDetails.SetActive(false);
      if (!helpList.activeSelf) helpList.SetActive(true);
    }
}
