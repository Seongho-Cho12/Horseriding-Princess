using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour
{
    private void Start()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input input = new Quantum.Input();

        // Note: Use GetButton not GetButtonDown/Up Quantum calculates up/down itself.
        input.Drift = UnityEngine.Input.GetButton("Jump");
        input.Powerup = UnityEngine.Input.GetButton("Fire1");
        input.Respawn = UnityEngine.Input.GetKey(KeyCode.R);
        input.BoostEnhancement = UnityEngine.Input.GetKey(KeyCode.LeftShift);

        var x = UnityEngine.Input.GetAxis("Horizontal");
        var y = UnityEngine.Input.GetAxis("Vertical");

        // Input that is passed into the simulation needs to be deterministic that's why it's converted to FPVector2.
        input.Direction = new Vector2(x, y).ToFPVector2();

        callback.SetInput(input, DeterministicInputFlags.Repeatable);
    }
}
