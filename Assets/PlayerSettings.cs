using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle aiToggle;
    [SerializeField] private Text godDescription;
    [SerializeField] private Slider godActivity;

    public GodName SelectedGod
    {
        get { return toggleGroup.ActiveToggles().First().GetComponent<GodToggle>().God; }
    }

    public bool IsBot
    {
        get { return aiToggle.isOn; }
    }

    public float Activity
    {
        get { return godActivity.value; }
    }

    private void Update()
    {
        godDescription.text = ((God) SelectedGod).Description;
    }
}