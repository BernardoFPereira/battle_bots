using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RobotFrameTypes
{
    SCOUT,
    WARRIOR,
    JUGGERNAUT,
}

[CreateAssetMenu(fileName = "New Robot Frame", menuName = "Robot Frame")]
public class RobotFrame : RobotPart
{
    // public new string name;
    public RobotFrameTypes robot_frame;

    // public int armor;
    // public int integrity = 100;

    public void Start()
    {
        switch (robot_frame)
        {
            case RobotFrameTypes.SCOUT:
                armor = 25;
                integrity = 75;
                break;
            case RobotFrameTypes.WARRIOR:
                armor = 50;
                integrity = 100;
                break;
            case RobotFrameTypes.JUGGERNAUT:
                armor = 75;
                integrity = 125;
                break;
            default:
                armor = 50;
                integrity = 100;
                break;
        }
    }
}
