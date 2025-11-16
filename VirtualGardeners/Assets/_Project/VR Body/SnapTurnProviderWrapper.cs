using UnityEngine;
using System.Reflection;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// A wrapper component that provides external access to trigger snap turn rotations
    /// without requiring input from the input system. Attach this to the same GameObject
    /// as your SnapTurnProvider (ActionBasedSnapTurnProvider or DeviceBasedSnapTurnProvider).
    /// </summary>
    [RequireComponent(typeof(SnapTurnProviderBase))]
    public class SnapTurnProviderWrapper : MonoBehaviour
    {
        private SnapTurnProviderBase m_SnapTurnProvider;
        private float m_LastTurnTime;
        private float m_DebounceTime = 0.5f;
        
        // Cached reflection methods for BeginLocomotion and EndLocomotion
        private MethodInfo m_BeginLocomotionMethod;
        private MethodInfo m_EndLocomotionMethod;

        void Awake()
        {
            m_SnapTurnProvider = GetComponent<SnapTurnProviderBase>();
            if (m_SnapTurnProvider == null)
            {
                Debug.LogError($"SnapTurnProviderWrapper on {gameObject.name} requires a SnapTurnProviderBase component.", this);
                return;
            }

            // Cache reflection methods for protected BeginLocomotion and EndLocomotion methods
            var baseType = typeof(LocomotionProvider);
            m_BeginLocomotionMethod = baseType.GetMethod("BeginLocomotion", BindingFlags.NonPublic | BindingFlags.Instance);
            m_EndLocomotionMethod = baseType.GetMethod("EndLocomotion", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        void Start()
        {
            // Sync debounce time with the snap turn provider
            if (m_SnapTurnProvider != null)
            {
                m_DebounceTime = m_SnapTurnProvider.debounceTime;
            }
        }

        /// <summary>
        /// Performs a snap turn rotation externally without requiring input from the input system.
        /// This method can be called by external objects to trigger a snap turn rotation.
        /// </summary>
        /// <param name="turnAmountInDegrees">The amount to rotate in degrees. Positive values rotate clockwise, negative values rotate counter-clockwise.</param>
        /// <returns>Returns <see langword="true"/> if the turn was successfully performed. Returns <see langword="false"/> if debounce time hasn't elapsed or locomotion system is busy.</returns>
        public bool TriggerSnapTurn(float turnAmountInDegrees)
        {
            if (m_SnapTurnProvider == null)
            {
                Debug.LogWarning("SnapTurnProvider is not assigned.", this);
                return false;
            }

            // Check if debounce time has elapsed
            if (Time.time - m_LastTurnTime < m_DebounceTime)
            {
                return false;
            }

            // Get the locomotion system
            var locomotionSystem = m_SnapTurnProvider.system;
            if (locomotionSystem == null)
            {
                Debug.LogWarning("LocomotionSystem is not assigned on SnapTurnProvider.", this);
                return false;
            }

            // Check if locomotion system is busy
            if (locomotionSystem.busy)
            {
                return false;
            }

            // Request exclusive access to locomotion system using reflection
            bool beginSuccess = false;
            if (m_BeginLocomotionMethod != null)
            {
                beginSuccess = (bool)m_BeginLocomotionMethod.Invoke(m_SnapTurnProvider, null);
                if (!beginSuccess)
                {
                    return false; // Failed to acquire exclusive access
                }
            }

            // Get the XR Origin
            var xrOrigin = locomotionSystem.xrOrigin;
            if (xrOrigin == null)
            {
                Debug.LogWarning("XR Origin is not assigned on LocomotionSystem.", this);
                // Release locomotion if we acquired it
                if (beginSuccess && m_EndLocomotionMethod != null)
                {
                    m_EndLocomotionMethod.Invoke(m_SnapTurnProvider, null);
                }
                return false;
            }

            // Perform the rotation immediately (bypassing delay time for external calls)
            xrOrigin.RotateAroundCameraUsingOriginUp(turnAmountInDegrees);
            
            // Release exclusive access to locomotion system
            if (beginSuccess && m_EndLocomotionMethod != null)
            {
                m_EndLocomotionMethod.Invoke(m_SnapTurnProvider, null);
            }
            
            // Update timing to respect debounce
            m_LastTurnTime = Time.time;
            
            return true;
        }

        /// <summary>
        /// Performs a snap turn rotation using the configured turn amount from the SnapTurnProvider.
        /// This is a convenience method that uses the current turnAmount setting.
        /// </summary>
        /// <param name="turnLeft">If true, rotates left (counter-clockwise), otherwise rotates right (clockwise).</param>
        /// <returns>Returns <see langword="true"/> if the turn was successfully performed. Returns <see langword="false"/> if debounce time hasn't elapsed or locomotion system is busy.</returns>
        public bool TriggerSnapTurn(bool turnLeft = false)
        {
            if (m_SnapTurnProvider == null)
            {
                return false;
            }

            float amount = turnLeft ? -m_SnapTurnProvider.turnAmount : m_SnapTurnProvider.turnAmount;
            return TriggerSnapTurn(amount);
        }

        /// <summary>
        /// Performs a 180-degree snap turn rotation.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the turn was successfully performed. Returns <see langword="false"/> if debounce time hasn't elapsed or locomotion system is busy.</returns>
        public bool TriggerSnapTurnAround()
        {
            return TriggerSnapTurn(180f);
        }

        /// <summary>
        /// Updates the debounce time to match the SnapTurnProvider's current debounce time.
        /// Call this if you've changed the debounce time on the SnapTurnProvider at runtime.
        /// </summary>
        public void SyncDebounceTime()
        {
            if (m_SnapTurnProvider != null)
            {
                m_DebounceTime = m_SnapTurnProvider.debounceTime;
            }
        }
    }
}

