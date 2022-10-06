using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 로딩 화면 동작 클래스
/// 작성자 - 김도영
/// </summary>
public class LoadingWindow : MonoBehaviour
{
    public static LoadingWindow instance;
    public static LoadingWindow Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<LoadingWindow>();
                instance = obj != null ? obj : Create();
                //if (obj != null) {   instance = obj;  }
                //else {   instance = Create();   }
            }
            return instance;
        }

        private set
        {
            instance = value;
        }
        
    }

    public static LoadingWindow Create()
    {
        var LoadingWindowPrefab = Resources.Load<LoadingWindow>("LoadingWindow");
        return Instantiate(LoadingWindowPrefab);
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        videoPlayer = transform.FindChildRecursive("MoviePlayer").gameObject.GetComponent<MoviePlayer>();

    }

    [SerializeField]
    private GameObject loading;
    [SerializeField]
    private Slider progressBar;
    [SerializeField]
    private MoviePlayer videoPlayer;
    private CanvasGroup cg;
    public TextMeshProUGUI loading_text;
    public GameObject circle;

    private float rotate;
    private string[] now;
    private int index;
    private bool loading_Comp = false;
    private string loadSceneName;



    // Start is called before the first frame update
    void Start()
    {        
        now = new string[] { "",".", "..", "..." };

        if (loading != null)
        {
            CircleRotate();
            StartCoroutine(nameof(IETextCycle));
        }
       videoPlayer.PlayVideo();
        SceneLoad();
    }

    IEnumerator IETextCycle()
    {
        while (index < now.Length)
        {
            TextChange(index);
            yield return new WaitForSeconds(1.0f);
            index++;
            if (index >= now.Length)
            {
                index = 0;
            }
            if (loading_Comp)
            {
                break;
            }
         }
    }

    private void TextChange(int num)
    {
        loading_text.text = "Now Loading" + now[num];     
    }

    private void CircleRotate()
    {
        float speed = 3f;
        Vector3 rot = new Vector3(0, 0, -360);
        RotateMode rotMode = RotateMode.FastBeyond360;
        Ease ease = Ease.Linear;
        LoopType loop = LoopType.Incremental;
        circle.transform.DOLocalRotate(rot, speed, rotMode).SetEase(ease).SetLoops(-1, loop);
    }


    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName;        
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        progressBar.value = 0f;
        yield return null;
        BackFade(false);
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, timer);
                if (progressBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);

                if (progressBar.value == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loadSceneName)
        {
            BackFade(true);            
            //videoPlayer.PlayVideo();
            SceneManager.sceneLoaded -= LoadSceneEnd;
            DOTween.KillAll();
        }
    }


    //씬 이동시 페이드 효과
    private void BackFade(bool Load)
    {
        int num;
        if (cg != null)
        {
            num = Load ? 0 : 1;
            cg.DOFade(num, 3.0f);

        }
        else
        {
            cg.DOKill(); //씬 이동 시 Dotween 실행 종료
        }

    }
    private void SceneLoad()
    {
        LoadScene("MainStage");
    }


}
