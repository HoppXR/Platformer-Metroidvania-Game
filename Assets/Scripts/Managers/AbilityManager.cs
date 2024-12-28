using System;
using Platformer;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    
    [HideInInspector] public Ability abilityToGive;
    public enum Ability
    {
        DoubleJump,
        Slide,
        Dash,
        Swing
    }
    
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Sliding slideAbility;
    [SerializeField] private Dashing dashAbility;
    [SerializeField] private Swinging swingAbility;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void EnableAbility(Ability ability)
    {
        if (ability == Ability.DoubleJump)
        {
            Debug.Log("Double Jump Enabled");
        }
        else if (ability == Ability.Slide)
        {
            Debug.Log("Slide Enabled");
        }
        else if (ability == Ability.Dash)
        {
            Debug.Log("Dash Enabled");
        }
        else if (ability == Ability.Swing)
        {
            Debug.Log("Swing Enabled");
        }
    }
}
