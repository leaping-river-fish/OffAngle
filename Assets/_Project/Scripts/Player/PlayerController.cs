// =============================================================================
// PlayerController — composition root for the player prefab.
//
// This MonoBehaviour contains ZERO gameplay logic. Its only responsibilities:
//   1. Fetch required components via GetComponent
//   2. Build and populate MovementStateContext
//   3. Call MovementStateMachine.Initialize(ctx)
//
// Any gameplay code placed here is a design error. Route it to a state class.
//
// ─────────────────────────────────────────────────────────────────────────────
// INSPECTOR SETUP CHECKLIST
// ─────────────────────────────────────────────────────────────────────────────
//   [ ] PlayerInputReader._actionAsset  ← drag PlayerInputActions.inputactions
//   [ ] PlayerCameraController is on the Camera child GameObject
//   [ ] PlayerCameraController._playerRoot ← leave null (auto-finds parent)
//   [ ] PlayerCameraController._inputReader ← leave null (auto-finds via parent)
//   [ ] Tune Movement Settings values below in the Inspector
//
// ─────────────────────────────────────────────────────────────────────────────
// CHARACTERCONTROLLER RECOMMENDED SETTINGS
// ─────────────────────────────────────────────────────────────────────────────
//   Slope Limit      45
//   Step Offset       0.3
//   Skin Width        0.08
//   Min Move Distance 0       ← prevents micro-stutter at low speeds
//   Center           (0, 1, 0) for a 2 m capsule standing on origin
//   Radius            0.5
//   Height            2.0
//
// ─────────────────────────────────────────────────────────────────────────────
// MULTIPLAYER INTEGRATION HOOK
// ─────────────────────────────────────────────────────────────────────────────
//   When adding NGO or Mirror, override OnNetworkSpawn() (or the equivalent)
//   and gate input on ownership:
//
//     public override void OnNetworkSpawn()
//     {
//         _inputReader.enabled = IsOwner;
//         GetComponentInChildren<PlayerCameraController>().enabled = IsOwner;
//     }
//
//   Remote players receive ctx.Velocity and StateId via NetworkVariables
//   and are animated/moved by a separate NetworkPlayerVisual component.
// =============================================================================

using UnityEngine;
using OffAngle.Core;
using OffAngle.Movement;

namespace OffAngle.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(MovementStateMachine))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MovementSettings _movementSettings = new MovementSettings();

        private PlayerInputReader    _inputReader;
        private MovementStateMachine _stateMachine;
        private CharacterController  _characterController;

        private void Awake()
        {
            _inputReader         = GetComponent<PlayerInputReader>();
            _stateMachine        = GetComponent<MovementStateMachine>();
            _characterController = GetComponent<CharacterController>();

            var ctx = new MovementStateContext
            {
                Controller      = _characterController,
                Input           = _inputReader,
                PlayerTransform = transform,
                StateMachine    = _stateMachine,
                Settings        = _movementSettings,
            };

            _stateMachine.Initialize(ctx);
        }
    }
}
