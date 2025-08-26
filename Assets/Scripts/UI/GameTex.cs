using System;
using TMPro;
using UnityEngine;

public class GameTex : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI coordinate;
    private void Update()
    {
        coordinate.text = "X: " + player.transform.position.x.ToString("F1") + " Y: " + player.transform.position.y.ToString("F1");
    }
}
