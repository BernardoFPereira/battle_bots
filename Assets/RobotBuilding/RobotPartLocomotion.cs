using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocomotionType
{
    WHEEL,
    TRACKS,
    LEGS,
}

[CreateAssetMenu(fileName = "New Robot Locomotion", menuName = "Robot Locomotion")]
public class RobotPartLocomotion : RobotPart
{
    public int base_speed;
    public LocomotionType locomotion_type;
}
