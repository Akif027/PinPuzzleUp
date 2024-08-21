using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public GameObject popEffect;
    public AudioClip BackGroundA;
    public AudioClip OnfillpoolA;
    public AudioClip EndTheGameA;
    public AudioClip OnComboA;
    public AudioClip OnButtonPressA;
    public List<Slot> SlotsList;

    public Color BottomFieldOnSelectButtonColor;

}

public enum PatternType
{
    Pyramid = 1,
    Vegas = 2,
    Art = 3
}

public enum SlotType
{
    Alarm,
    Camera,
    Car,
    Girl,
    Lipstick,
    Megaphone,
    Neclace,
    Phone,
    Record,
    Ring,
    Shoe,
    Tv,
    Red
}