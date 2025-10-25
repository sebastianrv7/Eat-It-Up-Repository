using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;

    [SerializeField]
    private CinemachineCamera cCamera;
    [SerializeField]
    private string videoClip; 

    private VideoPlayer videoPlayer;
    private Camera mainCamera;

    public delegate void CinematicFinished();
    public event CinematicFinished OnCinematicFinished;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
            
        //Debug.Log(Application.streamingAssetsPath + "/" + videoClip);
    }

    void Start()
    {
        mainCamera = (Camera)FindAnyObjectByType(typeof(Camera));
    }

    public void SetTrackingTarget(Transform newTarget)
    {
        cCamera.Follow = newTarget;
    }

    [ContextMenu("Test Video")]
    public void GameFinished()
    {
        videoPlayer = mainCamera.gameObject.AddComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.url = Application.streamingAssetsPath + "/" + "Videos/" + videoClip;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }

    private void EndReached(VideoPlayer vp)
    {
        OnCinematicFinished?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
    }
}
