using System;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }
    
    public enum Abilities
    {
        Null,
        DoubleJump,
        Slide,
        Dash,
        Swing
    }
    
    public static bool DoubleJumpEnabled;
    public static bool SlideEnabled;
    public static bool DashEnabled;
    public static bool SwingEnabled;
    
    [Header("For Easy Debugging")] // might remove later
    [SerializeField] private bool enableDoubleJump;
    [SerializeField] private bool enableSlide;
    [SerializeField] private bool enableDash;
    [SerializeField] private bool enableSwing;
    
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

    private void Start()
    {
        if (enableDoubleJump) DoubleJumpEnabled = true;
        if (enableSlide) SlideEnabled = true;
        if (enableDash) DashEnabled = true;
        if (enableSwing) SwingEnabled = true;
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
