using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LookingGlass.StatsDisplay
{
    // modified from https://github.com/Rune580/RiskOfOptions/blob/master/RiskOfOptions/Components/ColorPicker/RooSliderInput.cs
    public class SliderWithText : MonoBehaviour
    {
        public Slider slider;
        public TMP_InputField inputField;
        public string formatString = "f0";
        public UnityEvent<float> onValueChanged = new();

        private float _value;

        public float Value
        {
            get => _value;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_value != value)
                {
                    _value = value;
                    onValueChanged.Invoke(_value);
                }

                UpdateControls();
            }
        }

        public void Setup(Slider slider, TMP_InputField inputField, float min, float max)
        {
            this.slider = slider;
            this.inputField = inputField;
            inputField.enabled = true;
            slider.minValue = min;
            slider.maxValue = max;

            slider.onValueChanged.AddListener(SliderChanged);
            inputField.onEndEdit.AddListener(OnTextEdited);
            inputField.onSubmit.AddListener(OnTextEdited);
        }

        private void UpdateControls()
        {
            slider.value = Value;
            inputField.text = Value.ToString(formatString, CultureInfo.InvariantCulture);
        }

        private void SliderChanged(float newValue)
        {
            Value = newValue;
        }

        private void OnTextEdited(string newText)
        {
            if (float.TryParse(newText, out float num))
            {
                Value = Mathf.Clamp(num, slider.minValue, slider.maxValue);
            }
            else
            {
                Value = _value;
            }
        }
    }
}