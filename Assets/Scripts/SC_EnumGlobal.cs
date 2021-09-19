using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SC_EnumGlobal 
{
    public enum Screens { MainMenu, Loading, MultyPlayer,StudentInfo,Options, Game };
    public enum SlotState { Empty, Optional, White, Black };
    public enum Direction { Up= -6, Down=6, Left=-1, Right=1, LeftUp=-7, LeftDown=5, RightUp=-5, RightDown=7 }; //Values ​​for checking the direction in a matrix of 6 * 6 only!
    public enum Boundary { Top=0, Bottom=35, Left=6, Right=5 }; //if Top<0 , Bottom>35(Size of matrix), Left%6 ,Right+1%6  // OUT OF MATRIX
    public enum MatchState { NoWinner, Winner, Tie, WinnerBlack, WinnerWhite };
    public enum TurnState { Opponent, Player };
    public enum GameType { SinglePlayer, Multiplayer };
}
