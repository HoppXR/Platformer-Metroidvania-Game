using System;
using Platformer;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    
    public enum Abilities
    {
        DoubleJump,
        Slide,
        Dash,
        Swing
    }
    
    public static bool DoubleJumpEnabled;
    public static bool SlideEnabled;
    public static bool DashEnabled;
    public static bool SwingEnabled;
    
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

    public void EnableAbility(Abilities ability)
    {
        if (ability == Abilities.DoubleJump)
        {
            DoubleJumpEnabled = true;
            Debug.Log("Double Jump Enabled");
        }
        else if (ability == Abilities.Slide)
        {
            SlideEnabled = true;
            Debug.Log("Slide Enabled");
        }
        else if (ability == Abilities.Dash)
        {
            DashEnabled = true;
            Debug.Log("Dash Enabled");
        }
        else if (ability == Abilities.Swing)
        {
            SwingEnabled = true;
            Debug.Log("Swing Enabled");
        }
    }
}
