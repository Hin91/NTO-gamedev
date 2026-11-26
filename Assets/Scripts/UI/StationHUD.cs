using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StationHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameTickSystem gameTickSystem;

    [Header("Oxygen")]
    [SerializeField] private Slider oxygenSlider;
    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private TMP_Text oxygenLabel;

    [Header("Temperature")]
    [SerializeField] private Slider temperatureSlider;
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private TMP_Text temperatureLabel;
    private void Update()
    {
        if (gameTickSystem == null)
            return;

        StationState station = gameTickSystem.StationState;

        if (station == null)
            return;

        UpdateOxygen(station);
        UpdateTemperature(station);
    }

    private void UpdateOxygen(StationState station)
    {
        if (!station.TryGetFloat("oxygenAmount", out float oxygen))
            return;

        if (oxygenSlider != null)
            oxygenSlider.value = oxygen;

        if (oxygenText != null)
            oxygenText.text = $"{oxygen:0}%";

        if (station.TryGetDisplayName("oxygenAmount", out string displayName))
            oxygenLabel.text = displayName;
    }

    private void UpdateTemperature(StationState station)
    {
        if (!station.TryGetFloat("temperature", out float temperature))
            return;

        if (temperatureSlider != null)
            temperatureSlider.value = temperature;

        if (temperatureText != null)
            temperatureText.text = $"{temperature:0} °C";

        if (station.TryGetDisplayName("temperature", out string displayName))
            temperatureLabel.text = displayName;
    }


}
