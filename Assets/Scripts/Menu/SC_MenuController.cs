using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Student: Shani levi 
//Id: 302383619

public class SC_MenuController : MonoBehaviour
{
    public void Btn_SinglePlayer() { SC_MenuLogic.Instance.Btn_SinglePlayerLogic();}
    public void Btn_MultyPlayer() { SC_MenuLogic.Instance.Btn_MultyPlayerLogic();}
    public void Btn_StudentInfo() { SC_MenuLogic.Instance.Btn_StudentInfoLogic(); }
    public void Btn_Options() { SC_MenuLogic.Instance.Btn_OptionsLogic(); }
    public void Btn_Sound() { SC_MenuLogic.Instance.Btn_SoundLogic(); }
    public void Btn_back() { SC_MenuLogic.Instance.Btn_backLogic();}
    public void Btn_LeaveRoom() { SC_MenuLogic.Instance.Btn_LeaveRoomLogic(); }

    public void Btn_MultyPlayerStart() { SC_MenuLogic.Instance.Btn_MultyPlayerStartLogic(); }
    public void Btn_Link() { SC_MenuLogic.Instance.Btn_LinkLogic(); }

    public void Btn_Play() {SC_MenuLogic.Instance.Btn_Play();}

    public void Slider_MultyPlayer() { SC_MenuLogic.Instance.Slider_MultyPlayerLogic(); }
    public void Slider_Music() { SC_MenuLogic.Instance.Slider_MusicLogic(); }
    public void Slider_Sfx() { SC_MenuLogic.Instance.Slider_SfxLogic(); }
    public void Btn_Back_MainMenu() { SC_MenuLogic.Instance.Btn_Back_MainMenuLogic(); }
}
