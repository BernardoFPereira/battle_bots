using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SensorType
{
    RADAR,
    SONIC,
    VISUAL,
    SEISMIC,
}

[CreateAssetMenu(fileName = "New Robot Head", menuName = "Robot Head")]
public class RobotPartHead : RobotPart
{
    // public new string name;
    // public int armor;
    // public int integrity;
    public SensorType[] sensors;
}
