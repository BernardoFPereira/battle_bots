using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmsType
{
    SHIELD,
    CANNON,
    MACHINE_GUN,
    SWORD,
}

public enum WeaponRange
{
    NONE,
    MELEE,
    SHORT,
    MEDIUM,
    LONG,
}

[CreateAssetMenu(fileName = "New Robot Arm", menuName = "Robot Arm")]
public class RobotPartArm : RobotPart
{
    public int damage;
    public WeaponRange weapon_range;
}
