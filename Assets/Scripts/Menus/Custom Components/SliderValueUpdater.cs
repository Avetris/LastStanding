using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderValueUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrefab = null;

    Slider m_Slider = null;

    private void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(ChangeValue);
        ChangeValue(m_Slider.value);
    }

    private void OnDestroy()
    {
        m_Slider?.onValueChanged.RemoveListener(ChangeValue);
    }

    public void ChangeValue(float newValue)
    {
        textPrefab.text = newValue.ToString(m_Slider.wholeNumbers ? "f0" : "f1");
    }
}
