using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Student: Shani levi 
//Id: 302383619

public class SC_GameLogic : MonoBehaviour
{
    private Dictionary<string, GameObject> unityObjects;
    private SC_EnumGlobal.SlotState curState;
    private SC_EnumGlobal.TurnState curTurn, lefted;
    private SC_Board curBoard;

    private string nextTurn;
    private bool gameStarted = false;
    private float curTime  = 0;
    private bool flagOnce = true;
    private bool modeChange, OneTime = false;



    #region Events
    private void OnEnable() { 
        SC_Slots.OnSlotClicked += OnSlotClicked; 
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
        Listener.OnGameStopped += OnGameStopped;
        Listener.OnUserLeftRoom += OnUserLeftRoom;
    }
    private void OnDisable() { 
        SC_Slots.OnSlotClicked -= OnSlotClicked;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
        Listener.OnGameStopped -= OnGameStopped;
        Listener.OnUserLeftRoom -= OnUserLeftRoom;
    }


    #endregion

    #region MonoBehaviour
    void Awake() { Init(); }
    private void Update()
    {
        //handel SinglePlayer first click btn_SinglePlayer
        if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer && !OneTime)
        {
            OneTime = true;
            SinglePlayerStart();
            unityObjects["Txt_Timer"].GetComponent<Text>().text = "";
        }
        //handel MultiyPlayer : timming & player leaving .
        if (!gameStarted)
        {
            if (modeChange) unityObjects["Txt_AiPlaying"].GetComponent<Text>().text = "Ai Playing..";
            unityObjects["Txt_Timer"].GetComponent<Text>().text = "";
        }
        if (gameStarted)
        {
            unityObjects["Txt_AiPlaying"].GetComponent<Text>().text = "";
            int _calcTime = SC_GlobalVariables.maxTurnTime - (int)(Time.time - curTime);
            int curDisplay = SC_GlobalVariables.maxTurnTime - (int)(Time.time - curTime) - 5;
            if (modeChange)
            {
                //when one user leave room then do playing faster
                SC_GlobalVariables.curType = SC_EnumGlobal.GameType.SinglePlayer;

                unityObjects["Txt_Timer"].GetComponent<Text>().text = curDisplay.ToString();
                if (_calcTime == 5 && flagOnce)
                {
                    flagOnce = false;  //makes sure it call one time 
                    int _Index = curBoard.GetRandomSlot();  //get random index from the list of curr options
                    SetSlotDataMultyPlayer(_Index);
                    gameStarted = false;
                }
                if (_calcTime == 4) gameStarted = false;
                if (_calcTime <= 10 && _calcTime >= 9) unityObjects["Txt_Timer"].GetComponent<Text>().text = "player left";
            }
            else
            {
                //when time is up - do rendom move for player.
                if (_calcTime > 0) unityObjects["Txt_Timer"].GetComponent<Text>().text = curDisplay.ToString();
                //I chose to give 5 seconds because I saw that if I give a second or two sometimes no result comes back. 5 seconds is a safe range to complete the reading of functions.
                //So I say a queue in a game has 10 seconds and the server switches a queue in 15 seconds.In the 5 second range it does a random.
                if (_calcTime == 5 && flagOnce)
                {
                    flagOnce = false;                       //makes sure it call one time 
                    int _Index = curBoard.GetRandomSlot();  //get random index from the list of curr options
                    SetSlotDataMultyPlayer(_Index);         //when no time and dosent clicket then set rendom index to api
                }
                if (_calcTime <= 4 && _calcTime >= 0) unityObjects["Txt_Timer"].GetComponent<Text>().text = "0";
                if (_calcTime == 15) flagOnce = true;
            }
        }
    }
    #endregion

    #region Logic
    private void Init()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("UnityGameObjects");
        foreach (GameObject g in _objects)
            unityObjects.Add(g.name, g);
        InitGame();
    }
    private void InitGame()
    {
        curBoard = new SC_Board();
        for (int i = 0; i < SC_GlobalVariables.slotAmount; i++)
        {
            if (i == 14 || i == 21) { unityObjects["Slot" + i].GetComponent<SC_Slots>().ChangeSlotState(SC_EnumGlobal.SlotState.Black); }
            else if (i == 15 || i == 20) { unityObjects["Slot" + i].GetComponent<SC_Slots>().ChangeSlotState(SC_EnumGlobal.SlotState.White); }
            else { unityObjects["Slot" + i].GetComponent<SC_Slots>().ChangeSlotState(SC_EnumGlobal.SlotState.Empty); }
            //set all to fals , onClick will only in the optional places 
            unityObjects["Slot" + i].GetComponent<Button>().interactable = false;
        }
        unityObjects["PopUp_GameOver"].SetActive(false);
        unityObjects["Txt_Amount_Player"].GetComponent<Text>().text = "2";
        unityObjects["Txt_Amount_Ai"].GetComponent<Text>().text = "2";

        curState = SC_EnumGlobal.SlotState.White;
        ChangeInteractableOptions(SC_EnumGlobal.SlotState.White); //The one who starts is white
        unityObjects["Img_CurrentState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");

        SinglePlayerStart();
    }
    private void SinglePlayerStart()
    {
        if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                //AI
                unityObjects["Img_PlayereState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
                unityObjects["Img_AiState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
                curBoard.SetAiColor(SC_EnumGlobal.SlotState.Black);
                StartCoroutine(AILogic());
            }
            else
            {
                curTurn = SC_EnumGlobal.TurnState.Player;
                unityObjects["Img_PlayereState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
                unityObjects["Img_AiState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
                curBoard.SetAiColor(SC_EnumGlobal.SlotState.White);
            }
        }
    }
    private void PassTurn()
    {
        if (curState == SC_EnumGlobal.SlotState.Black)
        {
            curState = SC_EnumGlobal.SlotState.White;
            ChangeInteractableOptions(SC_EnumGlobal.SlotState.White); //set new options 
            unityObjects["Img_CurrentState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
        }
        else if (curState == SC_EnumGlobal.SlotState.White)
        {
            curState = SC_EnumGlobal.SlotState.Black;
            ChangeInteractableOptions(SC_EnumGlobal.SlotState.Black); 
            unityObjects["Img_CurrentState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
        }
    }
    private void ChangeInteractableStatus(bool _IsActive)
    {
        for (int i = 0; i < SC_GlobalVariables.slotAmount; i++)
            unityObjects["Slot" + i].GetComponent<Button>().interactable = _IsActive;
    }
    private void ChangeInteractableOptions(SC_EnumGlobal.SlotState slotState)
    {
        //Makes options appear and be clickable
        List<List<int>> _listOptions = curBoard.SetCurrListOptions(slotState);
        if (_listOptions.Count > 0)
        {
            foreach (List<int> items in _listOptions)
            {
                curBoard.SetSlotValue(items[items.Count - 1], SC_EnumGlobal.SlotState.Optional);
                unityObjects["Slot" + items[items.Count - 1]].GetComponent<SC_Slots>().ChangeSlotState(SC_EnumGlobal.SlotState.Optional);
                unityObjects["Slot" + items[items.Count - 1]].GetComponent<Button>().interactable = true;
            }
        }

        else
        { //No Optins on the board for the palyer
            Debug.Log("game over no options ");
            Placement(-2);  //-2 meens no moves for Player
        }
    }
    private IEnumerator AILogic()
    { 
        curTurn = SC_EnumGlobal.TurnState.Opponent;
        //wait for _rend time
        int _rand = UnityEngine.Random.Range(2, 4);
        yield return new WaitForSeconds(_rand);

        int _idx = curBoard.GetRandomSlot();  // _idx=-1 meens no move for ai -game is over
        Placement(_idx);
        curTurn = SC_EnumGlobal.TurnState.Player;
    }
    private SC_EnumGlobal.MatchState Placement(int _Index)
    {
        SC_EnumGlobal.MatchState _endState = SC_EnumGlobal.MatchState.NoWinner;
        List<List<int>> _listOptions = curBoard.GetListOfOptions(); 
        List<int> backANDwhitAmount = curBoard.GetAmountSlots_BW();

        if (_listOptions.Count == 0 || _Index < 0) //no options for the players or board is filled = Game over
        {
            Debug.Log("No moves for One Player, _listOptions= " + _listOptions.Count +" i= " + _Index);

            WarpClient.GetInstance().stopGame();
            _endState = curBoard.IsMatchOver();

            if (_endState != SC_EnumGlobal.MatchState.NoWinner)
            {
                ChangeInteractableStatus(false);
                unityObjects["PopUp_GameOver"].SetActive(true);

                //popUp setting when display for each player - who win.
                if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.Black)
                {
                    if (_endState == SC_EnumGlobal.MatchState.WinnerBlack) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " Openent Win!";
                    if (_endState == SC_EnumGlobal.MatchState.WinnerWhite) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " You Win! ";
                }
                else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.White)
                {
                    if (_endState == SC_EnumGlobal.MatchState.WinnerWhite) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " Openent Win! ";
                    if (_endState == SC_EnumGlobal.MatchState.WinnerBlack) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " You Win! ";
                }
                else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
                {
                    if (_endState == SC_EnumGlobal.MatchState.WinnerWhite && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.Black) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " Openent Win! ";
                    if (_endState == SC_EnumGlobal.MatchState.WinnerBlack && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.Black) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " You Win! ";
                    if (_endState == SC_EnumGlobal.MatchState.WinnerWhite && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.White) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " You Win! ";
                    if (_endState == SC_EnumGlobal.MatchState.WinnerBlack && curBoard.GetAiColor() == SC_EnumGlobal.SlotState.White) unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = " Openent Win! ";
                }
                //...end

                if (_endState == SC_EnumGlobal.MatchState.WinnerBlack)
                {
                    unityObjects["Img_GameOver_WinnerSign"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
                    unityObjects["Txt_GameOver_Vs"].GetComponent<Text>().text = backANDwhitAmount[0].ToString() + " VS " + backANDwhitAmount[1].ToString();
                }
                else if (_endState == SC_EnumGlobal.MatchState.WinnerWhite)
                {
                    unityObjects["Img_GameOver_WinnerSign"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
                    unityObjects["Txt_GameOver_Vs"].GetComponent<Text>().text = backANDwhitAmount[1].ToString() + " VS " + backANDwhitAmount[0].ToString();
                }
                else
                {
                    unityObjects["Txt_GameOver_Winner"].GetComponent<Text>().text = "Game Tie ";
                    unityObjects["Img_GameOver_WinnerSign"].GetComponent<Image>().enabled = false;
                    unityObjects["Txt_GameOver_Vs"].GetComponent<Text>().text = backANDwhitAmount[1].ToString() + " Vs " + backANDwhitAmount[0].ToString();
                }
            }
        }
        else if (_Index != -1 || _Index != -2) //Game continues // -1 meens no moves for AI //-2 meens no moves for Player
        { 
            if (curBoard.GetSlotValue(_Index) == SC_EnumGlobal.SlotState.Optional)
            {
                ChangeInterfaceSlots(_Index, curState); // Updates the list of Slots who are clicked and also changes the UI
                ClearOptions(_Index);                   // set optinal places to empty and clear list

                //chang num of black&white amounts display
                List<int> amount = curBoard.GetAmountSlots_BW();
                if (SC_EnumGlobal.SlotState.Black == curBoard.GetAiColor())
                {
                    if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer )   
                    {
                        unityObjects["Txt_Amount_Player"].GetComponent<Text>().text = amount[1].ToString();
                        unityObjects["Txt_Amount_Ai"].GetComponent<Text>().text = amount[0].ToString();
                    }
                    else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
                    {
                        unityObjects["Txt_Amount_Player"].GetComponent<Text>().text = amount[0].ToString();
                        unityObjects["Txt_Amount_Ai"].GetComponent<Text>().text = amount[1].ToString();
                    }
                }
                else if (SC_EnumGlobal.SlotState.White == curBoard.GetAiColor())
                {
                    if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer ) 
                    {
                        unityObjects["Txt_Amount_Player"].GetComponent<Text>().text = amount[0].ToString();
                        unityObjects["Txt_Amount_Ai"].GetComponent<Text>().text = amount[1].ToString();
                    }
                    else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
                    {
                        unityObjects["Txt_Amount_Player"].GetComponent<Text>().text = amount[1].ToString();
                        unityObjects["Txt_Amount_Ai"].GetComponent<Text>().text = amount[0].ToString();
                    }
               
                }
                PassTurn();
            }
        }
        return _endState;
    }
    public void ClearOptions(int clicked)
    {
        List<List<int>> _listOptions = curBoard.GetListOfOptions();
        foreach (List<int> items in _listOptions)
        {
            int i = items[items.Count - 1];
            if (i != clicked)
            {
                //curBoard.SetClicedSlots(items, curState);
                curBoard.SetSlotValue(i, SC_EnumGlobal.SlotState.Empty);
                unityObjects["Slot" + i].GetComponent<SC_Slots>().ChangeSlotState(SC_EnumGlobal.SlotState.Empty);
            }
            unityObjects["Slot" + i].GetComponent<Button>().interactable = false;
        }
        curBoard.ClearOptions();
    }
    public void ChangeInterfaceSlots(int _index, SC_EnumGlobal.SlotState curState)
    {
        //Handles the display of the color change after clicking Index
        List<List<int>> _listOptions = curBoard.GetListOfOptions();
        foreach (List<int> items in _listOptions)
        {
            if (_index == items[items.Count - 1])
            {
                curBoard.SetClicedSlots(items, curState);  //Handles white and black array updates
                for (int i = 0; i < items.Count; i++)
                {
                    curBoard.SetSlotValue(items[i], curState);
                    unityObjects["Slot" + items[i]].GetComponent<SC_Slots>().ChangeSlotState(curState);
                    unityObjects["Slot" + items[i]].GetComponent<Button>().interactable = false;
                }
            }
        }      
    }
    public void SetSlotDataMultyPlayer(int _Index)
    {
        if (curTurn == SC_EnumGlobal.TurnState.Player)
        {
            if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer)
            {
                //send move
                Dictionary<string, object> _toSer = new Dictionary<string, object>();
                _toSer.Add("Index", _Index);
                string _toSend = Json.Serialize(_toSer);
                //Debug.Log("_toSend + " + _toSend);
                WarpClient.GetInstance().sendMove(_toSend);  //save move and call OnMoveCompleted()
            }
            SC_EnumGlobal.MatchState _matchState = Placement(_Index);
            if (_matchState == SC_EnumGlobal.MatchState.NoWinner &&
                SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
                StartCoroutine(AILogic());
        }
    }
    #endregion

    #region Callbacks
    private void OnSlotClicked(int _Index)
    {
        if (modeChange)
            gameStarted = false;
        SetSlotDataMultyPlayer(_Index);
    }
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        SC_GlobalVariables.curType = SC_EnumGlobal.GameType.Multiplayer;
        InitGame();

        gameStarted = true;
        modeChange = false;
        nextTurn = _NextTurn;
        curTime = Time.time;

        if (nextTurn == SC_GlobalVariables.userId)
        {
            curTurn = SC_EnumGlobal.TurnState.Player;
            unityObjects["Img_PlayereState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
            unityObjects["Img_AiState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
            curBoard.SetAiColor(SC_EnumGlobal.SlotState.Black);
        }
        else
        {
            curTurn = SC_EnumGlobal.TurnState.Opponent;
            unityObjects["Img_PlayereState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("Black");
            unityObjects["Img_AiState"].GetComponent<Image>().sprite = SC_GlobalVariables.Instance.GetSprite("White");
            curBoard.SetAiColor(SC_EnumGlobal.SlotState.White);
        }
    }
    private void OnMoveCompleted(MoveEvent _Move)
    {
        if (_Move.getSender() != SC_GlobalVariables.userId && SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer)
        {
            // get index to display to anuther player
            Dictionary<string, object> _data = (Dictionary<string, object>)Json.Deserialize(_Move.getMoveData());
            int _index = int.Parse(_data["Index"].ToString());

            SC_EnumGlobal.MatchState _curState = Placement(_index);
            if (_curState != SC_EnumGlobal.MatchState.NoWinner)
                WarpClient.GetInstance().stopGame();
        }
        curTime = Time.time;

        if (_Move.getNextTurn() == SC_GlobalVariables.userId)
            curTurn = SC_EnumGlobal.TurnState.Player;
        else
            curTurn = SC_EnumGlobal.TurnState.Opponent;
    }
    private void OnGameStopped(string _Sender, string _RoomId)
    {
        gameStarted = false;
    }
    private void OnUserLeftRoom(RoomData eventObj, string _UserName)
    {
        SC_GlobalVariables.userLeftId = _UserName;
        if (_UserName != SC_GlobalVariables.userId && curTurn == SC_EnumGlobal.TurnState.Player)
        {
            //Debug.Log("Wait for Player move and then do AI move");
            modeChange = true;
            SC_GlobalVariables.curType = SC_EnumGlobal.GameType.SinglePlayer;
        }
        else if (curTurn == SC_EnumGlobal.TurnState.Opponent)
        {
            //Debug.Log("Do Ai move only");
            gameStarted = false;
            SC_GlobalVariables.curType = SC_EnumGlobal.GameType.SinglePlayer;
            unityObjects["Txt_AiPlaying"].GetComponent<Text>().text = "Ai Playing..";
            StartCoroutine(AILogic());
        }
    }
    #endregion

    #region Controller
    public void Btn_RestartGame()
    {
        if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.SinglePlayer)
            InitGame();
        else if (SC_GlobalVariables.curType == SC_EnumGlobal.GameType.Multiplayer)
            WarpClient.GetInstance().startGame();
    }
    #endregion
}
