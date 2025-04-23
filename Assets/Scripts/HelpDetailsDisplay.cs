using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpDetailsDisplay : MonoBehaviour
{

    [SerializeField] private TMP_Text topic; 
    [SerializeField] private TMP_Text requester; 
    [SerializeField] private TMP_Text description; 

    public void updateInfo(string topicText, string requesterText, string descriptionText){
      topic.text = topicText;
      requester.text = "Requested by " + requesterText;
      description.text = descriptionText;
    }
}
