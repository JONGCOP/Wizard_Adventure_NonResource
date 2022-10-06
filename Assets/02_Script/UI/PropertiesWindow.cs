using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// �Ӽ� ������ ���� Ŭ����
/// �Ӽ� ���� â�� ȸ�����Ѽ� ���ϴ� �Ӽ��� �����ϰԲ� ����
/// �ۼ��� - �赵��
/// </summary>
public class PropertiesWindow : MonoBehaviour
{
    #region ����
    public GameObject pp_base;                           // �Ӽ� ������ ���� ȸ���� UI
    private CanvasGroup cg;
    private PlayerMagic magic;
    private ECS_Changer ecs;

    Vector3 angle;

    private int[] pp_index = { 0, 1, -1 };                // �Ӽ� �ε���
    private int pp_Angle = 0;    
    private int horizontal;    
    private float speed = 0.5f;
    private float currentAngle = 0.0f;
    private float moveAngle = -120f, reversAngle = 120f;    
    private bool isRotate = false;   
    ElementType[] et = { ElementType.Fire, ElementType.Ice, ElementType.Lightning };  //�Ӽ� ����    
    ElementType pp;
    Ease ease;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        //ȸ�� �ӵ��� ��ȭ
        ease = Ease.InOutCubic;
        cg = this.GetComponent<CanvasGroup>();
        ecs = GetComponent<ECS_Changer>();
        magic = GetComponentInParent<PlayerMagic>();
        //magic.onChangeElement += ChangeElementAnimation;       
    }

    private void ChangeElementAnimation(ElementType element)
    {
        Debug.Log("�Ӽ� ����");
    }
    
    //�÷��̾� ��Ʈ�ѷ����� ���� ���� ���� �޾ƿ�
    public void OnChangeElement(int input)
    {       
        horizontal = input;
        StartCoroutine(IEBaseRotate(input));
       ElementChange((int)magic.CurrentElement);
       
    }


    //Axis Ű ���� ������ Circle�� ȸ��
    IEnumerator IEBaseRotate(int vector)
    {       
        float delay = 1.0f;

        if (vector == 0)
        {
            yield return null;
        }

        if (vector == -1 && !isRotate) //���� ȸ��
        {
            LeftMove();
            pp_Angle += horizontal;
            isRotate = false;
            yield return new WaitForSeconds(delay); //ȸ���ϸ� delay �ð� ���� �Է� ����
            OnPropertise(0);
        }
        else if (vector == 1 && !isRotate) //������ ȸ��
        {
            RightMove();
            pp_Angle += horizontal;
            isRotate = false;
            yield return new WaitForSeconds(delay);
            OnPropertise(0);
        }
        //�ѹ��� ���� �Ӽ��� �⺻�Ӽ����� �ʱ�ȭ
        if (pp_Angle > 2 || pp_Angle < -2)
        {
            pp_Angle = 0;

        }

    }

    //�Ӽ� ���� â�� �������� ȸ��
    private void LeftMove()
    {                
        isRotate = true;
        currentAngle += moveAngle;                                                   //���� ������ �����ϰ� ȸ�������� ���ϹǷ�  Circle�� ȸ����Ŵ
        angle = new Vector3(0, 0, currentAngle);
        pp_base.transform.DOLocalRotate(angle, speed).SetEase(ease);
        //ecs.SoundChangeRotate(0);
        if (currentAngle <= -360f) { currentAngle = 0.0f; }           //ȸ���� �� ���� ���� ���� �ʱ�ȭ
    }

    //�Ӽ� ���� â�� ���������� ȸ��
    private void RightMove()
    {        
        isRotate = true;
        currentAngle += reversAngle; 
        angle = new Vector3(0, 0, currentAngle);
        pp_base.transform.DOLocalRotate(angle, speed).SetEase(ease);
        //ecs.SoundChangeRotate(1);
        if (currentAngle >= 360f) { currentAngle = 0.0f; }       
    }

    //�Ӽ� ���� �Լ�
    //ȸ�� ���� �޾Ƽ� ElementType���� ��ȯ
    private void ElementChange(int Element)
    {
        ecs.SoundChange(Element);        
    }  
    
    public void OnPropertise(int alpha)
    {
        cg.DOFade(alpha, 1.0f);
    }


}
