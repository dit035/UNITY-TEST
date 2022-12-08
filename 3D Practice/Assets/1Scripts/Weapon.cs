using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponEnum { Melee, Range };
    public WeaponEnum WeaponType;
    public int WeaponDamage;
    public BoxCollider CollisionBox;
    public TrailRenderer WeaponEffect;

    public void UseWeapon()
    {
        if (WeaponType == WeaponEnum.Melee)
            StartCoroutine(HammerAttackDelay());
    }

    private IEnumerator HammerAttackDelay()
    {
        yield return new WaitForSeconds(0.1f);
        CollisionBox.enabled = true;
        WeaponEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        CollisionBox.enabled = false;

        yield return new WaitForSeconds(0.3f);
        WeaponEffect.enabled = false;
    }
}