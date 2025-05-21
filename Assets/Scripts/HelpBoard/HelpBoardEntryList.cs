using System.Collections.Generic;
using UnityEngine;
public class HelpBoardEntryList : MonoBehaviour
{
    [SerializeField] private List<HelpDetailsInfo> allHelpItems = new List<HelpDetailsInfo>();

    public List<HelpDetailsInfo> getAllHelpItems()
    {
        return allHelpItems;
    }

    // TODO: set/edit functions
}