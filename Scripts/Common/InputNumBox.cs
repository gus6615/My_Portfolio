using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputNumBox : MonoBehaviour
{
    public InputField inputField;
    public Slider slider;
    public long itemNum;
    private long savedNum;

    public UnityEvent onChangedSetNum;

    private void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        }
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(delegate { OnInputValueChanged(); });
            inputField.onEndEdit.AddListener(delegate { OnInputValueChanged(); });
        }
    }

    public void OnSliderValueChanged()
    {
        SetNum((long)slider.value);
    }

    public void OnInputValueChanged()
    {
        long num;

        try
        {
            num = long.Parse(inputField.text);
        }
        catch (System.Exception)
        {
            num = savedNum;
        }

        SetNum(num);
    }

    public void SetNum(long _num)
    {
        itemNum = _num;

        if (itemNum < 1)
        {
            itemNum = 1;
        }
        if (slider != null)
        {
            if (itemNum > slider.maxValue)
                itemNum = (long)slider.maxValue;
            slider.value = itemNum;
        }
        if (inputField != null)
        {
            inputField.text = GameFuction.GetNumText(itemNum);
        }
        if (onChangedSetNum != null)
        {
            onChangedSetNum.Invoke();
        }
        if (savedNum != itemNum)
        {
            savedNum = itemNum;
        }
    }
}
