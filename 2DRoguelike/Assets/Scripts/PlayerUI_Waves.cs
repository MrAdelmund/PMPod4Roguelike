using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI_Waves : MonoBehaviour
{
    private WaveManager wave;
    [SerializeField] private TMP_Text waveText;

    void Start()
    {
        wave = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WaveManager>();
    }

    private void Update()
    {
        waveText.text = $"Wave: {wave.currentWave}";
    }
}
