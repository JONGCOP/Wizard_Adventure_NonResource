using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// Main Title ���ۿ� Ŭ����
/// UI Ŭ�� �� ��ƼŬ ǥ��
/// �ۼ��� - �赵��
/// </summary>
public class PointShoot : MonoBehaviour
{
    [Header("�÷��̾� UI ����")]    
    [SerializeField] private Transform basePos;    
    [SerializeField] private GameObject Firebullet;
    [SerializeField] private GameObject FireExp;
    [SerializeField] private Transform ExPoint;

    private GameObject bulletFactory, ExpFactory;
    public Ease ease;

    // Start is called before the first frame update
    void Start()
    {        
        Firebullet.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        FireExp.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

    }

    // Update is called once per frame
    void Update()
    {        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (basePos != null)
            {
                Point_Particle();
            }
        }
    }

    void Point_Particle()
    {
        bulletFactory = Instantiate(Firebullet);        
        bulletFactory.transform.position = basePos.position;
        bulletFactory.transform.rotation = Quaternion.LookRotation(basePos.forward);
        bulletFactory.transform.DOMove(ExPoint.position, 0.5f).SetEase(ease);
        Destroy(bulletFactory, 0.6f);
        StartCoroutine(IEPointExp());
        
        
    }

    IEnumerator IEPointExp()
    {
        yield return new WaitForSeconds(0.6f);
        ExpFactory = Instantiate(FireExp);
        ExpFactory.transform.position = ExPoint.position;
    }


    

}
