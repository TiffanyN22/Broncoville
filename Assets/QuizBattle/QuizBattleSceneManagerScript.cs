using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizBattleSceneManagerScript : MonoBehaviour
{
    [SerializeField] GameObject TypeGame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTypeGame()
    {
        TypeGame.SetActive(true);
    }
}
