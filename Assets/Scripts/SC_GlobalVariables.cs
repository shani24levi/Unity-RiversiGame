using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GlobalVariables : MonoBehaviour
{
    #region Variables
    //Defining variables to support future matrix resizing
    public static int matrixLength = 6; //matrix is n*n
    public static int slotAmount = matrixLength * matrixLength; //6*6=36

    public static string _inRoomId;
    public static string userId;
    public static SC_EnumGlobal.GameType curType = SC_EnumGlobal.GameType.SinglePlayer;
    public static int maxTurnTime = 15;
    public static string userLeftId;
    public static bool endRoom = false;


    #endregion

    private Dictionary<string, Sprite> unitySprites;
    private static SC_GlobalVariables instance;
    public static SC_GlobalVariables Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_GlobalVariables").GetComponent<SC_GlobalVariables>();

            return instance;
        }
    }
    void Awake() { Init(); }
    private void Init()
    {
        unitySprites = new Dictionary<string, Sprite>();
        unitySprites.Add("White", Resources.Load<Sprite>("Textures/White"));
        unitySprites.Add("Black", Resources.Load<Sprite>("Textures/Black"));
        unitySprites.Add("Green", Resources.Load<Sprite>("Textures/Green"));
    }
    public Sprite GetSprite(string _SpriteName) { return unitySprites[_SpriteName];}
}
