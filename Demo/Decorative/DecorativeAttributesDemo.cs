using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class DecorativeAttributesDemo : MonoBehaviour
    {
        [Title("Player Settings", Icon.GameObject)]
        [Subtitle("Basic Info")]
        [Separator]
        
        [Tooltip("The name of the player character.")]
        public string playerName;
        
        [Tooltip("Enable advanced configuration options.")]
        public bool useAdvancedSettings;
        
        
        [Separator(Bottom = true)]
        [AddSpace(10)]
        [Subtitle("Transform Settings", Icon.Transform)]
        [Header("Spawn Configuration")]
        
        public Vector3 startPosition;
        public Vector3 startRotation;
        [Space(10)]
        public float startScale = 1f;
        
        [Separator]
        [Subtitle("Movement")]
        
        [InfoBox("Speed must be greater than zero.", InfoType.Warning, nameof(IsSpeedInvalid))]
        [Range(0f, 20f)]
        public float moveSpeed = 5f;
        
        [InfoBox("High acceleration may feel too responsive.", InfoType.Info, nameof(IsAccelerationHigh))]
        [Range(0f, 50f)]
        public float acceleration = 10f;
        
        [InfoBox("Debug mode is enabled.", InfoType.Info, nameof(showDebug), Bottom = true)]
        public bool showDebug = true;
        
        
        [Separator]
        [Subtitle("Health System")]
        
        [InfoBox("Health should not be negative.", InfoType.Error, nameof(IsHealthInvalid))]
        [Min(0)]
        public int health = 100;
        public int maxHealth = 100;
        
        [InfoBox("Health exceeds max health!", InfoType.Warning, nameof(IsHealthOverflow))]
        [InfoBox("Everything looks good.", InfoType.None, nameof(IsHealthValid), Bottom = true)]
        public string statusMessage = "OK";
        
        
        [Separator]
        [Subtitle("Advanced Settings", Icon.Script)]
        
        [ShowIf(nameof(useAdvancedSettings))]
        [InfoBox("Advanced settings enabled.", InfoType.Info, nameof(useAdvancedSettings))]
        public float airControl = 0.5f;
        
        [ShowIf(nameof(useAdvancedSettings))]
        public float gravityMultiplier = 1.2f;
        
        
        [Separator]
        [Subtitle("Runtime Info")]
        [Header("Read Only Debug")]
        
        [SerializeField] private Vector3 currentVelocity;
        
        [InfoBox("Careful tweaking these values.", InfoType.Warning, nameof(useAdvancedSettings), Bottom = true)]
        [SerializeField] private bool isGrounded;
        
        
        // METHODS
        private void Start()
        {
            currentVelocity = Vector3.zero;
            isGrounded = true;
        }
        
        // --- Conditions ---
        private bool IsSpeedInvalid() => moveSpeed <= 0f;
        private bool IsAccelerationHigh() => acceleration > 25f;
        private bool IsHealthInvalid() => health < 0;
        private bool IsHealthOverflow() => health > maxHealth;
        private bool IsHealthValid() => health >= 0 && health <= maxHealth;
    }
}