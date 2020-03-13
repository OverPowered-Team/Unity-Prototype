using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFunctions
{
    static public void ApplyBurnOnHit(GameObject enemy)
    {
        //TODO: Add particle
        Debug.Log("Here we code to burn the enemy that we will receive as parameter");
        EnemyGhoul g = enemy.GetComponent<EnemyGhoul>();
        if (g)
        {
            g.OnFire();
            return;
        }
        EnemyRanger r = enemy.GetComponent<EnemyRanger>();
        if (r)
        {
            r.OnFire();
            return;
        }
        EnemyMutant m = enemy.GetComponent<EnemyMutant>();
        if (m)
        {
            m.OnFire();
        }
    }
}
