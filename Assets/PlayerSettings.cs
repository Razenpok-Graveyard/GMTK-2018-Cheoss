using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle aiToggle;

    public GodName SelectedGod
    {
        get { return toggleGroup.ActiveToggles().First().GetComponent<GodToggle>().God; }
    }

    public bool IsBot
    {
        get { return aiToggle.isOn; }
    }
}