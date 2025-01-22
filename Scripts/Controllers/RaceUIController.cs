using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum UIState
{
    Waiting,
    Countdown,
    InRace,
    LocalFinished,
    RaceOver
}

[System.Serializable]
struct StateObjectPair
{
    public UIState State;
    public GameObject Object;
}

public unsafe class RaceUIController : QuantumCallbacks
{
    [SerializeField] private List<StateObjectPair> _stateObjectPairs = new();
    private Dictionary<UIState, GameObject> _stateObjectDictionary = new();

    [Header("Waiting")]
    [SerializeField] private TextMeshProUGUI _readyCtaText;
    
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI _countdownTimer;

    [Header("In Race")]
    [SerializeField] private TextMeshProUGUI _lapText;
    [SerializeField] private GameObject _finalLapText;

    [SerializeField] private GameObject _wrongWayRoot;

    [SerializeField] private TextMeshProUGUI _debugWeaponText;
    [SerializeField] private Image _weaponImage;
    [SerializeField] private TextMeshProUGUI _weaponUsesText;

    [Header("Race finishing")]
    [SerializeField] private TextMeshProUGUI _timeLeftText;

    [Header("General")] [SerializeField] private TextMeshProUGUI _raceStateText;
    [SerializeField] private List<TextMeshProUGUI> _positionTexts;
    private int _previousPosition = -1;

    [Header("Race over")]
    [SerializeField] private RectTransform _resultContainer;

    [SerializeField] private ResultItemController _resultItemPrefab;

    [Header("Stamina")]
    [SerializeField] private Image _staminaBar;

    [Header("Spurt UI")]
    [SerializeField] private GameObject _lastSpurtRoot;  // LastSpurtRoot 참조
    [SerializeField] private TextMeshProUGUI _lastSpurtText;  // LastSpurtText 참조

    private UIState _currentUIState = UIState.Waiting;
    private int lastCountdownTimer = 100;

    private QuantumGame _game;

    public override void OnUnitySceneLoadDone(QuantumGame game)
    {
        _game = game;
    }

    public override void OnGameStart(QuantumGame game, bool first)
    {
        _game = game;
    }

    public void OnContinueClicked()
    {
        Disconnect();
    }

    private void Awake()
    {
        QuantumEvent.Subscribe(this, (EventOnRaceStateChanged e) => OnRaceStateChanged(e));
        QuantumEvent.Subscribe(this, (EventOnPositionsUpdated e) => OnPositionsUpdated(e));
        QuantumEvent.Subscribe(this, (EventOnPlayerCompletedLap e) => OnPlayerCompletedLap(e));
        QuantumEvent.Subscribe(this, (EventOnWrongWayChanged e) => OnWrongWayChanged(e));
        QuantumEvent.Subscribe(this, (EventLocalPlayerReady e) => OnLocalPlayerReady(e));

        foreach (var pair in _stateObjectPairs)
        {
            _stateObjectDictionary.Add(pair.State, pair.Object);
        }
    }

    private void Update()
    {
        switch (_currentUIState)
        {
            default:
            case UIState.Waiting:
                break;
            case UIState.Countdown:
                UpdateCountdownText();
                break;
            case UIState.InRace:
                UpdateRaceTimeText();
                UpdateStaminaUI();
                UpdateSpurtUI();
                break;
            case UIState.LocalFinished:
                UpdateSpurtUI();
                break;
            case UIState.RaceOver:
                break;
        }
    }

    private void OnRaceStateChanged(EventOnRaceStateChanged e)
    {
        switch (e.state)
        {
            case RaceState.Waiting:
                SetUIState(UIState.Waiting);
                break;
            case RaceState.Countdown:
                SetUIState(UIState.Countdown);
                break;
            case RaceState.InProgress:
                SetUIState(UIState.InRace);
                break;
            case RaceState.Finishing:
                _timeLeftText.transform.parent.gameObject.SetActive(true);
                break;
            case RaceState.Finished:
                SetUIState(UIState.RaceOver);
                ShowResults();
                break;
            default:
                Debug.LogWarning("Unhandled UI state");
                break;
        }
    }

    private void Disconnect()
    {
        QuantumRunner.ShutdownAll();
        SceneManager.LoadScene(0);
    }

    private void ShowResults()
    {
        var allKarts = PlayerManager.Instance.GetAllKarts();

        Frame f = _game.Frames.Verified;

        List<(RaceProgress p, KartViewController k)> progresses = new List<(RaceProgress, KartViewController)>();

        foreach (var kart in allKarts)
        {
            RaceProgress* progress = f.Unsafe.GetPointer<RaceProgress>(kart.EntityView.EntityRef);

            progresses.Add((*progress, kart));
        }

        progresses.Sort(
            (a, b) =>
            {
                if (!a.p.Finished && b.p.Finished)
                {
                    return 1;
                }

                if (a.p.Finished && !b.p.Finished)
                {
                    return -1;
                }

                if (!a.p.Finished && !b.p.Finished)
                {
                    return 0;
                }

                return a.p.FinishTime.CompareTo(b.p.FinishTime);
            }
        );

        foreach (var pair in progresses)
        {
            int position = pair.p.Position;

            ResultItemController item = Instantiate(_resultItemPrefab, _resultContainer);
            item.SetResults(position, pair.k.DriverName, pair.p.FinishTime, pair.k.EntityView == LocalPlayerManager.Instance.LocalPlayerView);
        }
    }

    private void OnLocalPlayerReady(EventLocalPlayerReady e)
    {
        _readyCtaText.gameObject.SetActive(!e.ready);
    }

    private void OnWrongWayChanged(EventOnWrongWayChanged e)
    {
        _wrongWayRoot.SetActive(e.wrongWay);
    }

    private void OnPositionsUpdated(EventOnPositionsUpdated e)
    {
        if (_game.Frames.Verified != null
            && LocalPlayerManager.Instance.LocalPlayerEntity != default
            && _game.Frames.Verified.Unsafe.TryGetPointer(LocalPlayerManager.Instance.LocalPlayerEntity, out RaceProgress* raceProgress))
        {
            UpdatePosition(raceProgress->Position);
        }
    }

    private void OnPlayerCompletedLap(EventOnPlayerCompletedLap e)
    {
        if (e.entityRef != LocalPlayerManager.Instance.LocalPlayerEntity) { return; }

        if (_game.Frames.Verified.Unsafe.TryGetPointer(e.entityRef, out RaceProgress* raceProgress))
        {
            _lapText.GetComponent<UIAnimation>().PlayOnce();

            _lapText.text = "<b>" + (raceProgress->CurrentLap + 1) + "</b>/" + raceProgress->TotalLaps;
            _finalLapText.SetActive(raceProgress->CurrentLap == raceProgress->TotalLaps);

            if (raceProgress->Finished)
            {
                SetUIState(UIState.LocalFinished);
            }
        }
    }

    private void SetUIState(UIState state)
    {
        foreach (KeyValuePair<UIState, GameObject> pair in _stateObjectDictionary)
        {
            pair.Value.SetActive(pair.Key == state);
        }

        // 게임이 진행될 때만 스태미나 바 전체 표시
        _staminaBar.transform.parent.gameObject.SetActive(state == UIState.InRace);

        _currentUIState = state;
    }

    private void UpdatePosition(int position)
    {
        if (position == _previousPosition) { return; }

        foreach (TextMeshProUGUI text in _positionTexts)
        {
            text.text = GetPositionText(position);

            if (text.TryGetComponent(out UIAnimation anim))
            {
                anim.PlayOnce();
            }
        }

        _previousPosition = position;
    }

    private string GetPositionText(int position)
    {
        string suffix;
        switch (position)
        {
            case 1:
            case 21:
            case 31:
                suffix = "st";
                break;
            case 2:
            case 22:
            case 32:
                suffix = "nd";
                break;
            case 3:
            case 23:
            case 33:
                suffix = "rd";
                break;
            default:
                suffix = "th";
                break;
        }

        return $"{position.ToString()}<size=88>{suffix}</size>";
    }

    private void UpdateRaceTimeText()
    {
        if (_game.Frames.Predicted == null) { return; }

        var f = _game.Frames.Predicted;
        
        FP timeleft = f.GetSingleton<Race>().StateTimer.RemainingTime(f)!.Value;

        int minutes = FPMath.FloorToInt(timeleft / 60);
        int seconds = FPMath.FloorToInt(timeleft) % 60;
        int milliSeconds = FPMath.FloorToInt(timeleft % 1 * 100);

        _timeLeftText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);
    }

    private void UpdateCountdownText()
    {
        if (_game.Frames.Predicted == null) { return; }
        var f = _game.Frames.Predicted;
        
        FP timeleft = f.GetSingleton<Race>().StateTimer.RemainingTime(f)!.Value;
        
        int roundedTime = FPMath.CeilToInt(timeleft);

        if (roundedTime > 3)
        {
            _countdownTimer.text = "";
        }
        else if (roundedTime < lastCountdownTimer)
        {
            _countdownTimer.GetComponent<UIAnimation>().PlayOnce();
            _countdownTimer.text = roundedTime.ToString();
        }
        else if (roundedTime > lastCountdownTimer)
        {
            _countdownTimer.text = "";
        }

        lastCountdownTimer = roundedTime;
    }

    private void UpdateStaminaUI()
    {
        if (_game.Frames.Predicted == null) return;

        Frame f = _game.Frames.Predicted;
        if (f.Unsafe.TryGetPointer(LocalPlayerManager.Instance.LocalPlayerEntity, out Kart* kart))
        {
            KartStats stats = f.FindAsset(kart->StatsAsset);
            float staminaRatio = kart->Stamina.AsFloat / stats.maxStamina.AsFloat;
            _staminaBar.fillAmount = Mathf.Clamp01(staminaRatio);

            // 슬립스트림 상태에 따른 색상 변경
            if (kart->isSlipstreaming)
            {
                _staminaBar.color = new Color32(0x42, 0x5A, 0xDD, 0xff);  // 슬립스트림 시 파란색
            }
            else if (kart->BoostEnhancementTimer > 0)
            {
                _staminaBar.color = new Color32(0xDD, 0x42, 0x50, 0xff);  // 부스트 사용 시 빨간색
            }
            else
            {
                _staminaBar.color = new Color32(0x1F, 0xD3, 0x49, 0xff); // 기본 색상 (예: 녹색)
            }

            // 스태미나 값 로그 출력
            Debug.Log($"Current Stamina: {kart->Stamina.AsFloat} / Max Stamina: {stats.maxStamina.AsFloat}");
        }
        else
        {
            Debug.LogWarning("Failed to retrieve Kart component for stamina.");
        }
    }

    // 스퍼트 UI 상태 업데이트
    private void UpdateSpurtUI()
    {
        if (_game.Frames.Predicted == null) return;

        Frame f = _game.Frames.Predicted;
        if (f.Unsafe.TryGetPointer(LocalPlayerManager.Instance.LocalPlayerEntity, out Kart* kart))
        {
            // 스퍼트 활성화 여부 확인
            if (kart->MaxSpeed > FP._100 && _currentUIState == UIState.InRace)
            {
                _lastSpurtRoot.SetActive(true);
                _lastSpurtText.text = "Last Spurt!";
            }
            else
            {
                _lastSpurtRoot.SetActive(false);
            }
        }
    }
}
