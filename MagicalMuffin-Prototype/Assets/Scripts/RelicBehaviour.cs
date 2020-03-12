using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Relic_Type
{
    BASE,
    ATTACK,
    DASH,
    COMPANION
}
public enum Relic_Effect
{
   FIRE,
   POISON,
   EARTH,
   HEALING
}
public class RelicBehaviour: MonoBehaviour {

    public string relic_name;
    public string description;
    public Relic_Type relic_type;
    public Relic_Effect[] relic_effects;
    Relic relic;

    public void Start()
    {
        switch(relic_type)
        {
            case Relic_Type.BASE:
                relic = new Relic();
                break;
            case Relic_Type.ATTACK:
                relic = new AttackRelic();
                break;
        }  
        relic.relic_name = relic_name;
        relic.description = description;
        relic.relic_effects = relic_effects;
    }
    public void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            relic.OnPickUp(coll.gameObject.GetComponent<playerController>());
            GameObject.Find("Canvas").GetComponent<UIManager>().CreateRelicPopup((AttackRelic)relic, relic_type);
            Destroy(this.gameObject);
        }
    }
}

public class Relic
{
    public string relic_name;
    public string description;

    public Relic_Effect[] relic_effects;
    public List<Effect> effects;

    public Relic()
    {
        effects = new List<Effect>();
    }

    public virtual void OnPickUp(playerController player)
    {
        player.PickUpRelic(this);
    }
}
public class AttackRelic: Relic
{
    public string attack_name;

    public override void OnPickUp(playerController player)
    {
        List<string> attack_pool = player._playerCombos.GetFinalAttacks();
        int random_index = Random.Range(0, attack_pool.Count);
        attack_name = attack_pool[random_index];

        foreach (Relic_Effect effect in relic_effects)
        {
            AttackEffect test_effect = new AttackEffect();
            test_effect.SetAttackIdentifier(attack_name);

            switch (effect)
            {
                case Relic_Effect.FIRE:    
                    test_effect.on_hit_delegate = EffectFunctions.ApplyBurnOnHit;
                    break;
                case Relic_Effect.POISON:
                    break;
                case Relic_Effect.EARTH:
                    test_effect.AddFlatModifer(0.1f, "Attack_Damage");
                    break;
                case Relic_Effect.HEALING:
                    break;
            }

            effects.Add(test_effect);
        }
        //test_effect.AddFlatModifer(0.5f, "Attack_Range");
        //description = description.Replace("_combo_", attack_name);

        base.OnPickUp(player);
    }
}
