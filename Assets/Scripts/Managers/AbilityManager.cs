using System;
using UnityEngine;

namespace Managers
{
    public class AbilityManager : MonoBehaviour
    {
        #region Variables
        public static AbilityManager Instance { get; private set; }

        private MusicManager _musicManager;
        private int _numberOfEnabled;
    
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
        #endregion
        
        #region Unity Built-in Methods
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
            _musicManager = FindFirstObjectByType<MusicManager>();
        
            if (enableDoubleJump) DoubleJumpEnabled = true;
            if (enableSlide) SlideEnabled = true;
            if (enableDash) DashEnabled = true;
            if (enableSwing) SwingEnabled = true;
        }
        #endregion

        public void EnableAbility(Abilities ability)
        {
            _numberOfEnabled++;
        
            AddTracks();
        
            switch (ability)
            {
                case Abilities.DoubleJump:
                    DoubleJumpEnabled = true;
                    FindFirstObjectByType<UIManager>()?.EnableAbilityUI(Abilities.DoubleJump);
                    Debug.Log("Double Jump Enabled");
                    break;
                case Abilities.Slide:
                    SlideEnabled = true;
                    FindFirstObjectByType<UIManager>()?.EnableAbilityUI(Abilities.Slide);
                    Debug.Log("Slide Enabled");
                    break;
                case Abilities.Dash:
                    DashEnabled = true;
                    FindFirstObjectByType<UIManager>()?.EnableAbilityUI(Abilities.Dash);
                    Debug.Log("Dash Enabled");
                    break;
                case Abilities.Swing:
                    SwingEnabled = true;
                    FindFirstObjectByType<UIManager>()?.EnableAbilityUI(Abilities.Swing);
                    Debug.Log("Swing Enabled");
                    break;
                case Abilities.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ability), ability, null);
            }
        }

        private void AddTracks()
        {
            _musicManager.AddTrack(_numberOfEnabled - 1);
        }
    }
}
