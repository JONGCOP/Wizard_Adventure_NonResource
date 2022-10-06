using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// ���� ȭ�� ���� Ŭ����
/// �ۼ��� - �赵��
/// </summary>
public class MoviePlayer : MonoBehaviour
{
    [SerializeField]
    private RawImage canvas = null;
    [SerializeField]
    private VideoPlayer director = null;
    // Start is called before the first frame update
    void Start()
    {
        if (canvas != null && director != null)
        {
            // ���� �غ� �ڷ�ƾ ȣ��
           StartCoroutine(PrepareVideo());
        }
    }

   

    protected IEnumerator PrepareVideo()
    {
        // ���� �غ�
        director.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!director.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);            
        }
        // VideoPlayer�� ��� texture�� RawImage�� texture�� �����Ѵ�
        canvas.texture = director.texture;
    }

    public void PlayVideo()
    {       
        if (director != null && director.isPrepared)
        {
            // ���� ���
            director.Play();           
        }
    }

    public void StopVideo()
    {
        if (director != null && director.isPrepared)
        {
            // ���� ����
            director.Stop();
        }
    }

}
