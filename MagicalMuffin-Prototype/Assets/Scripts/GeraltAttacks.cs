using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeraltAttacks : MonoBehaviour
{
    public AttackList attacks;

    private Animator anim;
    private List<Attack> startingCombos;

    private Attack currAttack = null;
    private float lastAttackStartTime = 0f;
    [Tooltip("In seconds")]
    public float extraInputWindow;
    private string nextInput = "";

    private playerController _playerController;
    [HideInInspector] public float lastAttackFinishTime;

    Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string> buttonString;

    public GameObject particleScale;
    public GameObject swordScaleCollider;//Used for scaling the collider
    public GameObject swordCollider;//Used for activating and desactivating

    private AudioSource audioSource;

    public GameObject horizontalLayoutPrefab;
    public GameObject combosPanel;
    public GameObject currentComboUI;
    public GameObject[] button_images;

    //The most hardcoded thing ever because Unity doen't let us acess the state's in runtime...
    //We pasted here the exact values found in the Animator window
    private Dictionary<string, float> animationSpeeds;

    private void Start()
    {
        animationSpeeds = new Dictionary<string, float>();
        animationSpeeds["x"] = 2f;
        animationSpeeds["xx"] = 2f;
        animationSpeeds["xxy"] = 2f;
        animationSpeeds["xxyx"] = 2f;
        animationSpeeds["xxx"] = 2f;
        animationSpeeds["xxxx"] = 2f;
        animationSpeeds["xxxxx"] = 4f;
        animationSpeeds["xxxy"] = 2f;
        animationSpeeds["xxxyy"] = 2f;
        animationSpeeds["xy"] = 3f;
        animationSpeeds["xyx"] = 1.25f;
        animationSpeeds["y"] = 2f;
        animationSpeeds["yy"] = 2f;
        animationSpeeds["yyy"] = 2f;
        animationSpeeds["yyyy"] = 2f;
        animationSpeeds["yyyyy"] = 1.5f;

        anim = GetComponent<Animator>();
        _playerController = GetComponent<playerController>();
        
        buttonString = new Dictionary<UnityEngine.InputSystem.Controls.ButtonControl, string>();
        buttonString.Add(_playerController.gamepad.buttonSouth, "a");
        buttonString.Add(_playerController.gamepad.buttonWest,  "x");
        buttonString.Add(_playerController.gamepad.buttonNorth, "y");
        buttonString.Add(_playerController.gamepad.buttonEast,  "b");

        startingCombos = new List<Attack>();
        startingCombos.Add(attacks.attacks.Find(attack => attack.name == "x"));
        startingCombos.Add(attacks.attacks.Find(attack => attack.name == "y"));

        lastAttackStartTime = Time.time;

        audioSource = GetComponent<AudioSource>();

        CreateCombosUI();
    }

    void CreateCombosUI()
    {
        //Find the last attacks
        List<string> attack_pool = GetFinalAttacks();
        foreach (string attack in attack_pool)
        {
            GameObject comboUI = Instantiate(horizontalLayoutPrefab, combosPanel.transform);
            foreach (char attackButton in attack)
            {
                GameObject new_button;
                if (attackButton == 'x')
                {
                    new_button = Instantiate(button_images[0], comboUI.transform);
                }
                else if (attackButton == 'y')
                {
                    new_button = Instantiate(button_images[1], comboUI.transform);
                }
            }
        }
    }

    float GetAnimatorStateSpeed(string name)
    {
        //AnimatorOverrideController ac = anim.runtimeAnimatorController as AnimatorOverrideController;
        //RuntimeAnimatorController ac = anim.runtimeAnimatorController as RuntimeAnimatorController;

        //UnityEditor.Animations.AnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        //UnityEditor.Animations.ChildAnimatorState[] states = ac.layers[0].stateMachine.states;
        //foreach (UnityEditor.Animations.ChildAnimatorState state in states)
        //{
        //    if (state.state.name == name)
        //    {
        //        return state.state.speed;
        //    }
        //}
        //return 1f;

        if (animationSpeeds.ContainsKey(name))
        {
            return animationSpeeds[name];
        }
        else
        {
            return 1f;
        }
    }

    private bool FinishedAttack()
    {
        if (currAttack != null)
        {
            return Time.time - lastAttackStartTime >= GetAnimationClip(currAttack.animation_id).length / GetAnimatorStateSpeed(currAttack.name);
            //return Time.time - lastAttackStartTime >= GetAnimationClip(currAttack.animation_id).length / anim.GetCurrentAnimatorStateInfo(0).speed;
        }
        else
        {
            return true;
        }  
    }

    public void CancelCombo()
    {
        currAttack = null;
    }

    private float GetAttackLength(Attack attack)
    {
        if (attack == null)
        {
            return 0f;
        }
        else
        {
            return GetAnimationClip(attack.animation_id).length / GetAnimatorStateSpeed(attack.name);
        }
    }

    public void UpdateAttack()
    {
        if (Time.time - (lastAttackStartTime + GetAttackLength(currAttack)) > extraInputWindow)
        {
            currAttack = null;
        }
        ActDesactCollider(
            currAttack != null
            && Time.time > lastAttackStartTime + 0.02
            && Time.time < lastAttackStartTime + GetAttackLength(currAttack) - 0.02);
        ResizeCollider(currAttack);

        RegisterNewInput(_playerController.gamepad.buttonWest);
        RegisterNewInput(_playerController.gamepad.buttonNorth);
        PlayNextCombo();
    }

    public void PlayNextCombo()
    {
        //If the combo has finished
        if (FinishedAttack())
        {
            lastAttackFinishTime = Time.time;
            if (nextInput != "")
            {
                audioSource.Play();
                //INFO: Check if the input given matches any of the inputs of the next attacks
                Attack nextAttack = FindNextAttack(currAttack, nextInput);
                if (nextAttack != null)
                {
                    currAttack = nextAttack;
                }
                else
                {
                    currAttack = attacks.attacks.Find(attack => attack.name == nextInput);
                }
                anim.Play(currAttack.animation_id);
                lastAttackStartTime = Time.time;
                nextInput = "";
            }
            else
            {
                Vector2 move = _playerController.gamepad.leftStick.ReadValue();
                if (move == Vector2.zero)
                {
                    anim.CrossFade("idle", extraInputWindow);
                }
                else
                {
                    anim.CrossFade("Movement", extraInputWindow);
                }
                _playerController.currState = PlayerState.ATTACK_RETURN;
            }
        }
    }

    public void RegisterNewInput(UnityEngine.InputSystem.Controls.ButtonControl button)
    {
        if (button.wasPressedThisFrame)
        {
            nextInput = buttonString[button];
            UpdateCurrentComboUI();
        }
    }

    private void UpdateCurrentComboUI()
    {
        foreach (Transform child in currentComboUI.transform)
        {
            Destroy(child.gameObject);
        }

        Attack nextAttack = FindNextAttack(currAttack, nextInput);
        if (nextAttack == null)
        {
            //Simply show the 1 button
            if (nextInput[0] == 'x')
            {
                Instantiate(button_images[0], currentComboUI.transform);
            }
            else if (nextInput[0] == 'y')
            {
                Instantiate(button_images[1], currentComboUI.transform);
            }
        }
        else
        {
            //Show the full combo that you're going to do
            foreach (char attackButton in nextAttack.name)
            {
                if (attackButton == 'x')
                {
                    Instantiate(button_images[0], currentComboUI.transform);
                }
                else if (attackButton == 'y')
                {
                    Instantiate(button_images[1], currentComboUI.transform);
                }
            }
        }
    }

    //INFO: Returns null if there isn't an attack that follows with the given input
    private Attack FindNextAttack(Attack currAttack, string input)
    {
        string attackName;

        if (currAttack == null)
        {
            attackName = input;
        }
        else
        {
            attackName = currAttack.name + input;
        }

        return attacks.attacks.Find(attack => attack.name == attackName);
    }

    private AnimationClip GetAnimationClip(string clipName)
    {
        foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    private bool IsLastAttack(Attack currAttack)
    {
        foreach (Attack attack in attacks.attacks)
        {
            if (attack.name.Length > currAttack.name.Length)
            {
                for (int i = 0; i < currAttack.name.Length + 1; ++i)
                {
                    if (i < currAttack.name.Length)
                    {
                        if (currAttack.name[i] != attack.name[i])
                        {
                            break;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void OnAddAttackEffect(string attack_name)
    {
        Attack affected_attack = attacks.attacks.Find(attack => attack.name == attack_name);

        //Should probably make a list of stats and iterate through them
        affected_attack.base_damage.CalculateStat(_playerController.effects);
        affected_attack.base_range.CalculateStat(_playerController.effects);
    }

    public void OnHit(GameObject enemy)
    {
        foreach (AttackEffect effect in _playerController.effects)
        {
            if(effect.GetAttackIdentifier() == currAttack.name)
            {
                effect.on_hit_delegate(enemy);
                break;
            }
        }
    }

    public List<string> GetFinalAttacks()
    {
        List<string> final_attacks = new List<string>();

        foreach (Attack attack in attacks.attacks)
        {
            if (IsLastAttack(attack))
                final_attacks.Add(attack.name);
        }

        return final_attacks;
    }
    private void ActDesactCollider(bool active)
    {
        swordCollider.SetActive(active);
    }

    private void ResizeCollider(Attack attack)
    {
        if (attack != null)
        {
            swordScaleCollider.transform.localScale = new Vector3(1f, 1f, attack.base_range.GetValue());
            //particleScale.transform.localScale = new Vector3(1f, 1f, attack.base_range.GetValue() + 3);//INFO: + 3 to place the particle on the tip of the sword
        }
    }

    public Attack GetCurrentAttack()
    {
        return currAttack;
    }


    //INFO: Repeatable 1 attack combos
    //- Unity doesn't let us play the same animation after one has finished
    //- If for example, _x didn't have a combo, and we were to press x after the animation has ended and it's in the inputWindowTime, it wouldn't play it again
    //- To work around this, you can add a transition from that single repetable combo to the idle state
}
