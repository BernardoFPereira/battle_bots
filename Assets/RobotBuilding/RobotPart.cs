using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    FRAME,
    HEAD,
    ARM,
    LOCOMOTION,
}

public class RobotPart : ScriptableObject
{
    public new string name;
    public PartType part_type;
    public int armor;
    public int integrity;
}
