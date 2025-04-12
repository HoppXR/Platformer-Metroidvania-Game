using System;
using System.Threading.Tasks;
using Player;
using Player.Movement;
using TMPro;
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
            Swing,
            AtkIncrease,
            MaxHealth,
        }
    
        public static bool DoubleJumpEnabled;
        public static bool SlideEnabled;
        public static bool DashEnabled;
        public static bool SwingEnabled;
    
        /*[Header("For Easy Debugging")] // might remove later
        [SerializeField] private bool enableDoubleJump;
        [SerializeField] private bool enableSlide;
        [SerializeField] private bool enableDash;
        [SerializeField] private bool enableSwing;*/
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
        
            /*// Debug
            if (enableDoubleJump) DoubleJumpEnabled = true;
            if (enableSlide) SlideEnabled = true;
            if (enableDash) DashEnabled = true;
            if (enableSwing) SwingEnabled = true;*/
        }
        #endregion

        public async void EnableAbility(Abilities ability)
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
                case Abilities.AtkIncrease:
                    GameManager.PlayerDamage *= 2;
                    Debug.Log("Atk Increase");
                    break;
                case Abilities.MaxHealth:
                    MaxHealth();
                    Debug.Log("Health Max");
                    break;
                case Abilities.Null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ability), ability, null);
            }
        }

        public void DisableAllAbilities()
        {
            DoubleJumpEnabled = false;
            SlideEnabled = false;
            DashEnabled = false;
            SwingEnabled = false;
        }

        private void AddTracks()
        {
            _musicManager?.AddTrack(_numberOfEnabled - 1);
        }

        private void MaxHealth()
        {
            FindFirstObjectByType<PlayerHealth>().MaxHeal();
        }
    }
}
