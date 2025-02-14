using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public List<WeaponStats> stats;
    public int weaponLevel;

    [HideInInspector]
    public bool statsUpdated;

    public Sprite icon;
    
    public virtual void LevelUp()
    {
        if (weaponLevel < stats.Count - 1)
        {
            weaponLevel++;

            statsUpdated = true;

            if (weaponLevel >= stats.Count - 1)
            {
                Player.instance.fullyLevelledWeapons.Add(this);
                Player.instance.assignedWeapons.Remove(this);
            }
        }
    }
   
}

[System.Serializable]
public class WeaponStats
{
    public float speed, damage, range, timeBetweenAttacks, amount, duration, holderDistance;
    public string upgradeText;

}
