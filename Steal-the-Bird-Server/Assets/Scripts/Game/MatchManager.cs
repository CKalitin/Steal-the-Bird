using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchState {
    Lobby,
    InGame,
    Paused,
    Ended
}

public class MatchManager : MonoBehaviour {
    public static MatchManager instance;

    private MatchState matchState = MatchState.Lobby;

    private bool timerActive = false;
    private DateTime startTime;
    private float duration;
    private MatchState targetMatchState;

    public MatchState MatchState { get => matchState; set => matchState = value; }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.Log($"Match Manager instance already exists on ({gameObject}), destroying this.");
            Destroy(this);
        }
    }
    
    private void Update() {
        Countdown();
    }

    private void Countdown() {
        if (!timerActive) return;
        
        float timePassed = (float)(DateTime.Now - startTime).TotalSeconds;
        if (timePassed >= duration) {
            timerActive = false;
            ChangeMatchState(targetMatchState);
        }
    }

    public void ChangeMatchState(MatchState _matchState) {
        matchState = _matchState;

        timerActive = false;

        USNL.PacketSend.MatchUpdate((int)matchState);
    }

    public void NewCountdown(float _duration, MatchState _targetMatchState, string _countdownTag) {
        startTime = DateTime.Now;
        duration = _duration;
        targetMatchState = _targetMatchState;
        
        int[] startTimeArray = new int[4] { startTime.Hour, startTime.Minute, startTime.Second, startTime.Millisecond };

        USNL.PacketSend.Countdown(startTimeArray, duration, _countdownTag);
    }
}
