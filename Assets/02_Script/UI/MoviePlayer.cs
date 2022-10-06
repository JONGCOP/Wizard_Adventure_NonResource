using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 영상 화면 동작 클래스
/// 작성자 - 김도영
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
            // 비디오 준비 코루틴 호출
           StartCoroutine(PrepareVideo());
        }
    }

   

    protected IEnumerator PrepareVideo()
    {
        // 비디오 준비
        director.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!director.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);            
        }
        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        canvas.texture = director.texture;
    }

    public void PlayVideo()
    {       
        if (director != null && director.isPrepared)
        {
            // 비디오 재생
            director.Play();           
        }
    }

    public void StopVideo()
    {
        if (director != null && director.isPrepared)
        {
            // 비디오 멈춤
            director.Stop();
        }
    }

}
