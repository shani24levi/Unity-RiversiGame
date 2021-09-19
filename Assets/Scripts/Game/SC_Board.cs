using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Board 
{
    private List<SC_EnumGlobal.SlotState> curBoard;
    private List<int> clickedWhite;
    private List<int> clickedBlack;
    List<List<int>> _listOptions;
    SC_EnumGlobal.SlotState aiStart;
    public SC_Board()
    {
        curBoard = new List<SC_EnumGlobal.SlotState>();
        clickedWhite = new List<int>();
        clickedBlack = new List<int>();

        for (int i = 0; i < SC_GlobalVariables.slotAmount; i++)
        {
            if (i == 14 || i == 21)
            {
                curBoard.Add(SC_EnumGlobal.SlotState.Black);
                clickedBlack.Add(i);
            }

            else if (i == 15 || i == 20)
            {
                curBoard.Add(SC_EnumGlobal.SlotState.White);
                clickedWhite.Add(i);
            }
            else curBoard.Add(SC_EnumGlobal.SlotState.Empty);
        }
    }
    public SC_EnumGlobal.MatchState IsMatchOver()
    {
        if (CheckGameOver())
        {
            if (clickedWhite.Count == clickedBlack.Count)
                return SC_EnumGlobal.MatchState.Tie;
            if(clickedWhite.Count > clickedBlack.Count)
                return SC_EnumGlobal.MatchState.WinnerWhite;
            if (clickedWhite.Count < clickedBlack.Count)
                return SC_EnumGlobal.MatchState.WinnerBlack;
        }
        return SC_EnumGlobal.MatchState.NoWinner;
    }
    private bool CheckGameOver()
    {
        if (clickedWhite.Count+clickedBlack.Count == SC_GlobalVariables.slotAmount)
            return true;
        else if (_listOptions.Count == 0)
            return true;
        return false;
    }

    public List<int> GetAmountSlots_BW()
    {
        List<int> _list = new List<int>();
        _list.Add(clickedBlack.Count);
        _list.Add(clickedWhite.Count);

        return _list;
    }
    public List<List<int>> SetCurrListOptions(SC_EnumGlobal.SlotState slotState)
    {
        List<int> _options= new List<int>();
        _listOptions = new List<List<int>>();
        List<int> tempArr = new List<int>();

        if (slotState == SC_EnumGlobal.SlotState.White)
            tempArr = clickedWhite;
        else if(slotState == SC_EnumGlobal.SlotState.Black)
            tempArr = clickedBlack;

       // Debug.Log("FOR - " + slotState);

        for (int i = 0; i < tempArr.Count; i++)
            {
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.Up, SC_EnumGlobal.Boundary.Top, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.Down, SC_EnumGlobal.Boundary.Bottom, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.Left, SC_EnumGlobal.Boundary.Left, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.Right, SC_EnumGlobal.Boundary.Right, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.LeftDown, SC_EnumGlobal.Boundary.Left, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.LeftUp, SC_EnumGlobal.Boundary.Left, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.RightDown, SC_EnumGlobal.Boundary.Left, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
            _options = SetOptionByDirection(i, SC_EnumGlobal.Direction.RightUp, SC_EnumGlobal.Boundary.Left, slotState);
            if (_options.Count > 0) _listOptions.Add(_options);
        }
        return _listOptions;
    }

    public bool is_in_Boundaries(int cuurDirection, int prevDirection, int direction, SC_EnumGlobal.Boundary boundary)
    {
        //Checking the cases of the matrix boundaries
        if ((direction == (int)SC_EnumGlobal.Direction.Up && cuurDirection >= (int)boundary)
            || (direction == (int)SC_EnumGlobal.Direction.Down && cuurDirection <= (int)boundary)

            || (direction == (int)SC_EnumGlobal.Direction.Left && (is_Boundary_Right(cuurDirection) && !is_Boundary_Left(prevDirection)) && cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && cuurDirection >= (int)SC_EnumGlobal.Boundary.Top)
            || (direction == (int)SC_EnumGlobal.Direction.Left && (is_Boundary_Left(cuurDirection) || (!is_Boundary_Left(cuurDirection) && !is_Boundary_Right(cuurDirection))) && cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && cuurDirection >= (int)SC_EnumGlobal.Boundary.Top)
            || (direction == (int)SC_EnumGlobal.Direction.LeftDown && (cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && ((is_Boundary_Right(cuurDirection) && !is_Boundary_Left(prevDirection)) || (!is_Boundary_Left(cuurDirection) && !is_Boundary_Right(cuurDirection)))))
            || (direction == (int)SC_EnumGlobal.Direction.LeftUp && (cuurDirection >= (int)SC_EnumGlobal.Boundary.Top && ((is_Boundary_Right(cuurDirection) && !is_Boundary_Left(prevDirection)) || (!is_Boundary_Left(cuurDirection) && !is_Boundary_Right(cuurDirection)))))

            || (direction == (int)SC_EnumGlobal.Direction.Right && (is_Boundary_Left(cuurDirection) && !is_Boundary_Right(prevDirection)) && cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && cuurDirection >= (int)SC_EnumGlobal.Boundary.Top)
            || (direction == (int)SC_EnumGlobal.Direction.Right && (is_Boundary_Right(cuurDirection) || (!is_Boundary_Right(cuurDirection) && !is_Boundary_Left(cuurDirection))) && cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && cuurDirection >= (int)SC_EnumGlobal.Boundary.Top)
            || (direction == (int)SC_EnumGlobal.Direction.RightDown && cuurDirection <= (int)SC_EnumGlobal.Boundary.Bottom && ((is_Boundary_Left(cuurDirection) && !is_Boundary_Right(prevDirection)) || (!is_Boundary_Right(cuurDirection) && !is_Boundary_Left(cuurDirection))))
            || (direction == (int)SC_EnumGlobal.Direction.RightUp && cuurDirection >= (int)SC_EnumGlobal.Boundary.Top && ((is_Boundary_Left(cuurDirection) && !is_Boundary_Right(prevDirection)) || (!is_Boundary_Right(cuurDirection) && !is_Boundary_Left(cuurDirection))))
            )
            return true;
        else return false;
    }
    public List<int> SetOptionByDirection(int index, SC_EnumGlobal.Direction direction, SC_EnumGlobal.Boundary boundary, SC_EnumGlobal.SlotState slotState)
    {
        List<int> _options = new List<int>();
        int cuurDirection = 0;
        int prevDirection = 1;

        if (slotState == SC_EnumGlobal.SlotState.White) {cuurDirection = clickedWhite[index] + (int)direction;}
        else if (slotState == SC_EnumGlobal.SlotState.Black) {cuurDirection = clickedBlack[index] + (int)direction;}

        while (true)
        {
            //Checking the cases of the matrix boundaries
            if (!is_in_Boundaries(cuurDirection, prevDirection, (int)direction, boundary))
            {
               // Debug.Log("========>>>>>OUT>>> " +cuurDirection + "BY>> "+ direction+ "PRAV="+ prevDirection);
                return new List<int>();
            }
            else if (slotState == curBoard[cuurDirection]) return new List<int>();
            else if (curBoard[cuurDirection] == SC_EnumGlobal.SlotState.Empty)
            {
                if (_options.Count > 0)
                {
                    _options.Add(cuurDirection);
                    return _options;
                }
                return _options;
            }
            else if ((slotState == SC_EnumGlobal.SlotState.White && curBoard[cuurDirection] == SC_EnumGlobal.SlotState.Black)
                || (slotState == SC_EnumGlobal.SlotState.Black && curBoard[cuurDirection] == SC_EnumGlobal.SlotState.White))
            {
                _options.Add(cuurDirection);
                prevDirection = cuurDirection;
                cuurDirection = cuurDirection + (int)direction;
            }
        }
    }

    public void SetClicedSlots(List<int> items ,SC_EnumGlobal.SlotState slotState)
    {
        if (slotState == SC_EnumGlobal.SlotState.White)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (curBoard[items[i]] == SC_EnumGlobal.SlotState.Black)
                {
                    clickedBlack.Remove(items[i]); 
                    clickedWhite.Add(items[i]);
                }
                else if (curBoard[items[i]] == SC_EnumGlobal.SlotState.Optional)
                    clickedWhite.Add(items[i]);
            }
        }
        else if (slotState == SC_EnumGlobal.SlotState.Black)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (curBoard[items[i]] == SC_EnumGlobal.SlotState.White)
                {
                    clickedWhite.Remove(items[i]); //Removes them from the White list
                    clickedBlack.Add(items[i]); //Puts them on the black list
                }
                else if (curBoard[items[i]] == SC_EnumGlobal.SlotState.Optional)
                    clickedBlack.Add(items[i]);
            }
        }
    }
    public bool is_Boundary_Left(int n)
    {
        for(int i=0; i <= 30; i = i + 6)
        {
            if (n == i) return true;
        }
        return false;
    }
    public bool is_Boundary_Right(int n)
    {
        for (int i = 5; i <= 35; i = i + 6)
        {
            if (n == i) return true;
        }
        return false;
    }
    public List<List<int>> GetListOfOptions() {return _listOptions;}
    public void ClearOptions() {_listOptions.Clear();}
   
    public int GetRandomSlot()
    {
        List<int> _list = new List<int>();

        if (_listOptions.Count == 0) return -1;

        foreach (List<int> arr in _listOptions)
            foreach (int i in arr)
                if(i == arr[arr.Count-1])
                    _list.Add(i);

        int _retIndx = UnityEngine.Random.Range(0, _list.Count);
        return _list[_retIndx];
    }
    public SC_EnumGlobal.SlotState GetSlotValue(int _Index) { return curBoard[_Index];}
    public void SetSlotValue(int _Index, SC_EnumGlobal.SlotState _NewState) {curBoard[_Index] = _NewState; }

    public void SetAiColor(SC_EnumGlobal.SlotState slot) { aiStart = slot;}
    public SC_EnumGlobal.SlotState GetAiColor() { return aiStart; }
}
