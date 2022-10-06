using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enemy ü�¹� �����̴��� ǥ��
/// �ۼ��� - ������
/// </summary>
public class HP_Slider : MonoBehaviour
{
    public Slider sliderHP;
    [SerializeField]
    private CharacterStatus status;

    
    void Start()
    {
        //sliderHP.maxValue = maxHP;
        status.onHpChange += UpdateSlider;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateSlider(float percent)
    {
        sliderHP.value = percent;
    }    
}
