using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoElement : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI idText;
    [Space]
    [SerializeField] private TextMeshProUGUI usernameText;
    [Space]
    [SerializeField] private TextMeshProUGUI damageDealtText;
    [SerializeField] private TextMeshProUGUI damageTakenText;
    [Space]
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [Space]
    [SerializeField] private TextMeshProUGUI playerKillsText;
    [SerializeField] private TextMeshProUGUI playerDeathsText;
    [Space]
    [SerializeField] private TextMeshProUGUI enemyKillsText;
    [SerializeField] private TextMeshProUGUI enemyDeathsText;
    [Space]
    [SerializeField] private TextMeshProUGUI scoreText;
        
    public void SetInfo(USNL.PlayerInfoPacket _packet) {
        if (idText != null) idText.text = _packet.ClientId.ToString();
        if (usernameText != null) usernameText.text = _packet.Username;
        if (damageDealtText != null) damageDealtText.text = _packet.DamageDealt.ToString();
        if (damageTakenText != null) damageTakenText.text = _packet.DamageTaken.ToString();
        if (killsText != null) killsText.text = (_packet.PlayerKills + _packet.EnemyKills).ToString();
        if (deathsText != null) deathsText.text = (_packet.PlayerDeaths + _packet.EnemyDeaths).ToString();
        if (playerKillsText != null) playerKillsText.text = _packet.PlayerKills.ToString();
        if (playerDeathsText != null) playerDeathsText.text = _packet.PlayerDeaths.ToString();
        if (enemyKillsText != null) enemyKillsText.text = _packet.EnemyKills.ToString();
        if (enemyDeathsText != null) enemyDeathsText.text = _packet.EnemyDeaths.ToString();
        if (scoreText != null) scoreText.text = _packet.Score.ToString();
    }
}
