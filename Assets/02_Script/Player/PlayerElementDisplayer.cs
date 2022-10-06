using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� ���Ҹ� ��Ÿ��
/// �ۼ��� - ����ö
/// </summary>
public class PlayerElementDisplayer : MonoBehaviour
{
    [SerializeField, Tooltip("�÷��̾� ���� ����")]
    private PlayerMagic playerMagic;

    [SerializeField, Tooltip("ȸ�� �ӵ�")] 
    private float rotationSpeed = 45f;
    private Transform origin;

    [SerializeField]
    private GameObject[] elementParticles = new GameObject[(int)ElementType.None * 2];
    

    private void Start()
    {
        // �ʱ�ȭ
        for (int i = 0; i < (int)ElementType.None * 2; i++)
        {
            elementParticles[i] = transform.GetChild(i).gameObject;
        }
        ChangeDisplayedElement(playerMagic.CurrentElement);

        playerMagic.onChangeElement += ChangeDisplayedElement;
    }

    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.forward);
    }

    private void ChangeDisplayedElement(ElementType elementType)
    {
        for (int i = 0; i < (int)ElementType.None * 2; i++)
        {
            bool isCurrentElement = (i / 2) == (int)elementType;
            elementParticles[i].SetActive(isCurrentElement);
        }
    }
}
