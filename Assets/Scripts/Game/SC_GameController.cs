using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameController : MonoBehaviour
{
    public SC_GameLogic sc_GameLogic;
    public void Btn_RestartGame()
    {
        if (sc_GameLogic != null)
            sc_GameLogic.Btn_RestartGame();
    }
}
