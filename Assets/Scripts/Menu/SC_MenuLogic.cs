using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_MenuLogic : MonoBehaviour
{
    private Dictionary<string, GameObject> unityObjects;
    private Stack<SC_EnumGlobal.Screens> pathScreens;  //save the path when click so we can back to correct srceen
    private SC_EnumGlobal.Screens currentScreen;
    private SC_EnumGlobal.Screens prevScreen;

    private string apiKey = "008bd2b07290d1c0f68346890df6e7a78a5828b79e3bd3490d674de48bcfc6ff";
    private string secretKey = "2ae718750c60e630c25c7f75281ec23b0b42611eb4b29e531ed0a412e64761a3";
    private Listener listner;
    private Dictionary<string, object> passedParms;
    private List<string> roomIds;
    private string roomId;
    private int roomIndex = 0;

    #region Singleton
    static SC_MenuLogic instance;
    public static SC_MenuLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_MenuLogic").GetComponent<SC_MenuLogic>();
            return instance;
        }
    }
    #endregion

    #region MonoBehaviour
    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnGameStarted += OnGameStarted; 
        Listener.OnUserLeftRoom += OnUserLeftRoom;
        Listener.OnRoomDestroyed += OnRoomDestroyed;
    }
    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnUserLeftRoom += OnUserLeftRoom;
        Listener.OnRoomDestroyed -= OnRoomDestroyed;
    }

    private void Awake() { Init(); }
    private void Start()
    {
        unityObjects["Screen_Game"].SetActive(false);
    }
    #endregion

    #region SC_Controller
    public void Btn_SinglePlayerLogic(){
        SC_GlobalVariables.curType = SC_EnumGlobal.GameType.SinglePlayer;
        ChangeScreen(SC_EnumGlobal.Screens.Game);
    }
    public void Btn_MultyPlayerLogic(){ChangeScreen(SC_EnumGlobal.Screens.MultyPlayer);}
    public void Btn_StudentInfoLogic(){ChangeScreen(SC_EnumGlobal.Screens.StudentInfo);}
    public void Btn_OptionsLogic(){ChangeScreen(SC_EnumGlobal.Screens.Options);}
    public void Btn_SoundLogic()
    {
        //for get camera componnent //for sound btn
        GameObject Cam = GameObject.Find("Main Camera");
        //Debug.Log(Cam.GetComponent<AudioSource>().volume);

        if (Cam.GetComponent<AudioSource>().volume > 0)
        {
            Cam.GetComponent<AudioSource>().volume = 0;            
            unityObjects["Img_Sound"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("NonSound");
        }
        else
        {
            Cam.GetComponent<AudioSource>().volume = 0.2f;
            unityObjects["Img_Sound"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Sound");
        }
    }
    public void Btn_backLogic()
    {
        currentScreen = pathScreens.Pop();
        prevScreen = pathScreens.Pop();
        pathScreens.Push(prevScreen);

        unityObjects["Screen_" + currentScreen].SetActive(false);
        unityObjects["Screen_"+ prevScreen].SetActive(true);
    }
    public void Btn_LeaveRoomLogic()
    {
        if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer && SC_GlobalVariables.userLeftId == null)
            Btn_backLogic();
        //when one user allrady left the room
        else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer && SC_GlobalVariables.userLeftId != null)
        {
            //close the room
            //Debug.Log("bey bey all usrs and room");
            Btn_backLogic();
            unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
            WarpClient.GetInstance().LeaveRoom(roomId);         //user leave room
            WarpClient.GetInstance().UnsubscribeRoom(roomId);  //Unsubscribe user from room
            WarpClient.GetInstance().DeleteRoom(roomId);
            SC_GlobalVariables.userLeftId = null;
        }
        else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer)
        {
            //Debug.Log("bey bey one user");
            Btn_backLogic();
            //SC_GlobalVariables.userLeftId = thisUserId;
            unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
            WarpClient.GetInstance().LeaveRoom(roomId);         //user leave room
            WarpClient.GetInstance().UnsubscribeRoom(roomId);  //Unsubscribe user from room
        }
    }
    public void Btn_MultyPlayerStartLogic(){ChangeScreen(SC_EnumGlobal.Screens.Loading);}
    public void Btn_LinkLogic() {Application.OpenURL("https://www.linkedin.com/in/shani-levi-244455209/");}
    public void Btn_Play()
    {
        SC_GlobalVariables.curType = SC_EnumGlobal.GameType.Multiplayer;
        unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
        UpdateStatus("Searching..");
    }
    public void Slider_MultyPlayerLogic()
    {unityObjects["Txt_value"].GetComponent<Text>().text = unityObjects["Slider_MultyPlayer"].GetComponent<Slider>().value.ToString() + "$";}
    public void Slider_MusicLogic()
    {unityObjects["Text_Music"].GetComponent<Text>().text = unityObjects["Slider_Music"].GetComponent<Slider>().value.ToString();}
    public void Slider_SfxLogic()
    {unityObjects["Text_Sfx"].GetComponent<Text>().text = unityObjects["Slider_Sfx"].GetComponent<Slider>().value.ToString();}
    #endregion

    #region Logic
    private void Init()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _objs = GameObject.FindGameObjectsWithTag("unityObjects");
        foreach (GameObject g in _objs)
            unityObjects.Add(g.name, g);

        passedParms = new Dictionary<string, object>();
        passedParms.Add("Password", unityObjects["Txt_value"].GetComponent<Text>().text);
        
        unityObjects["Btn_Play"].GetComponent<Button>().interactable = false;

        //conect to api
        if (listner == null)
            listner = new Listener();

        WarpClient.initialize(apiKey, secretKey);
        //list of calls i want to listien to .
        WarpClient.GetInstance().AddConnectionRequestListener(listner);
        WarpClient.GetInstance().AddChatRequestListener(listner);
        WarpClient.GetInstance().AddUpdateRequestListener(listner);
        WarpClient.GetInstance().AddLobbyRequestListener(listner);
        WarpClient.GetInstance().AddNotificationListener(listner);
        WarpClient.GetInstance().AddRoomRequestListener(listner);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listner);
        WarpClient.GetInstance().AddZoneRequestListener(listner);

        SC_GlobalVariables.userId = System.DateTime.Now.Ticks.ToString();
        WarpClient.GetInstance().Connect(SC_GlobalVariables.userId);
        UpdateStatus("Connecting...");
        //end conect to api

        currentScreen = SC_EnumGlobal.Screens.MainMenu;
        prevScreen = SC_EnumGlobal.Screens.MainMenu;

        pathScreens = new Stack<SC_EnumGlobal.Screens>();
        pathScreens.Push(SC_EnumGlobal.Screens.MainMenu); //set first screen in the stuck

        unityObjects["Screen_Loading"].SetActive(false);
        unityObjects["Screen_MultyPlayer"].SetActive(false);
        unityObjects["Screen_StudentInfo"].SetActive(false);
        unityObjects["Screen_Options"].SetActive(false);
    }
    private void ChangeScreen(SC_EnumGlobal.Screens _NewScreen)
    {
        prevScreen = pathScreens.Pop();
        pathScreens.Push(prevScreen);
        pathScreens.Push(_NewScreen);
        currentScreen = _NewScreen;

        unityObjects["Screen_" + prevScreen].SetActive(false);
        unityObjects["Screen_" + currentScreen].SetActive(true);
    }
    private void UpdateStatus(string _NewStatus)
    {
        unityObjects["Txt_Status"].GetComponent<Text>().text = _NewStatus;
    }
    private void DoRoomsSearchLogic()
    {
        if (roomIndex < roomIds.Count)
        {
            UpdateStatus("Getting room: " + roomIds[roomIndex]);
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIndex]);
        }
        else
        {
            UpdateStatus("Creating Room....");
            WarpClient.GetInstance().CreateTurnRoom("Test" + System.DateTime.Now.Ticks.ToString(), SC_GlobalVariables.userId, 2, passedParms, SC_GlobalVariables.maxTurnTime);
        }
    }
    #endregion

    #region CallBack
    private void OnConnect(bool _IsSuccess)
    {
        if (_IsSuccess)
        {
            unityObjects["Btn_Play"].GetComponent<Button>().interactable = true;
            UpdateStatus("Connected");
        }
        else UpdateStatus("Failed to connect.");
    }
    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log("OnRoomsInRange " + _IsSuccess + " " + eventObj.getRoomsData().Length);
        if (_IsSuccess)
        {
            UpdateStatus("Parsing Rooms");
            roomIds = new List<string>();
            foreach (var RoomData in eventObj.getRoomsData())
            {
                //Debug.Log("RoomId " + RoomData.getId());
                //Debug.Log("RoomOwner " + RoomData.getRoomOwner());
                roomIds.Add(RoomData.getId());
            }
        }
        roomIndex = 0;
        DoRoomsSearchLogic();
    }
    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        //Debug.Log("OnCreateRoom " + _IsSuccess + ", RoomId: " + _RoomId);
        if (_IsSuccess)
        {
            roomId = _RoomId;
            UpdateStatus("Room created : " + _RoomId);
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else UpdateStatus("Failed to create a room.");
    }
    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
        {
            UpdateStatus("Joined room: " + _RoomId + ", Waiting for an opponent...");
        }
        else
        {
            WarpClient.GetInstance().UnsubscribeRoom(roomId);
            UpdateStatus("Failed to join the " + _RoomId + "  room.");
            roomIndex++;
            DoRoomsSearchLogic();
        }
    }
    private void OnUserJoinRoom(RoomData eventObj, string _UserName)
    {
        UpdateStatus("User: " + _UserName + " has joined the room");
        if (eventObj.getRoomOwner() == SC_GlobalVariables.userId && SC_GlobalVariables.userId != _UserName)
        {
            UpdateStatus("Starting Game...");
            WarpClient.GetInstance().startGame(); //set game to start
        }
    }
    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        //Debug.Log("OnGetLiveRoomInfo " + eventObj.getProperties());
        if (eventObj.getProperties() != null && eventObj.getProperties().ContainsKey("Password") &&
            eventObj.getProperties()["Password"].ToString() == passedParms["Password"].ToString())
        {
            roomId = eventObj.getData().getId();
            UpdateStatus("Matching, joining room... (" + roomId + ")");
            WarpClient.GetInstance().JoinRoom(roomId);
            WarpClient.GetInstance().SubscribeRoom(roomId);
        }
        else
        {
            UpdateStatus("No matched Password");
            roomIndex++;
            DoRoomsSearchLogic();
        }
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        ChangeScreen(SC_EnumGlobal.Screens.Game);
        UpdateStatus("The game have started, Turn: " + _NextTurn);
    }

    private void OnUserLeftRoom(RoomData eventObj, string _UserName)
    {
        Debug.Log("onclicked leave " + _UserName);
    }

    private void OnRoomDestroyed(RoomData eventObj)
    {
        Debug.Log("room Destroyed");

    }
    #endregion

}
