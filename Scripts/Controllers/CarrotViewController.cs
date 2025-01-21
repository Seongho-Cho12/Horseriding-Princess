using System.Collections;
using Quantum;
using UnityEngine;
using TMPro;

public class CarrotViewController : QuantumCallbacks
{
    [SerializeField] private GameObject _carrotObject;

    private EntityRef _cachedRef;

    private EntityRef _entityRef
    {
        get
        {
            if (_cachedRef == default)
            {
                _cachedRef = GetComponent<QuantumEntityView>().EntityRef;
            }
            return _cachedRef;
        }
    }

    private QuantumGame _game;

    public override void OnUnitySceneLoadDone(QuantumGame game)
    {
        _game = game;
    }

    public override void OnGameStart(QuantumGame game, bool first)
    {
        _game = game;
    }

    private void Awake()
    {
        QuantumEvent.Subscribe(this, (EventOnCarrotCollected e) => OnCarrotCollected(e));
    }

    private void OnCarrotCollected(EventOnCarrotCollected callback)
    {
        if (callback.CarrotEntity != _entityRef) { return; }

        ToggleVisible(false);
    }

    private void ToggleVisible(bool visible)
    {
        _carrotObject.SetActive(visible);
    }
}
