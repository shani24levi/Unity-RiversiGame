using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_Slots : MonoBehaviour
{
    public delegate void SlotClickedHandler(int _Index);
    public static event SlotClickedHandler OnSlotClicked;

    public int index = 0;
    public Image slotImage;
    //public Sprite White, Black;

    public void Click()
    {
        if (OnSlotClicked != null)
            OnSlotClicked(index);
    }

    public void ChangeSlotState(SC_EnumGlobal.SlotState _NewState)
    {
        if (slotImage != null)
        {
            switch (_NewState)
            {
                case SC_EnumGlobal.SlotState.Empty: slotImage.enabled = false; break;
                case SC_EnumGlobal.SlotState.Black:
                    slotImage.enabled = true;
                    slotImage.sprite = SC_GlobalVariables.Instance.GetSprite("Black");// SC_GameData.Instance.GetSprite("Black");
                    break;
                case SC_EnumGlobal.SlotState.White:
                    slotImage.enabled = true;
                    slotImage.sprite = SC_GlobalVariables.Instance.GetSprite("White") ; // SC_GameData.Instance.GetSprite("White");
                    break;
                case SC_EnumGlobal.SlotState.Optional:
                    slotImage.enabled = true;
                    slotImage.sprite = SC_GlobalVariables.Instance.GetSprite("Green"); // SC_GameData.Instance.GetSprite("White");
                    break;
            }
        }
    }
}
