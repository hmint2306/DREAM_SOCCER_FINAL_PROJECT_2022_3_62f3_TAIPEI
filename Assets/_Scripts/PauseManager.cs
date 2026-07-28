using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI References")]
    public GameObject pauseMenuPanel; // Kéo PauseMenuPanel vào đây
    public GameObject pauseButton;    // Kéo Nút Pause (góc trên phải) vào đây

    [Header("Quit Scene Settings")]
    [Tooltip("Điền Build Index của Scene 3 (hoặc đổi sang tên Scene 3)")]
    public int scene3Index = 3; 

    [HideInInspector]
    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // Ban đầu ẩn Panel và bật Nút Pause góc trên
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    private void Update()
    {
        // Bấm phím ESC để bật/tắt Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        Time.timeScale = 0f; // Dừng thời gian/vật lý game
    }

    // ==========================================
    // NÚT 1: CONTINUE (CHẠY TIẾP GAME)
    // ==========================================
    public void ContinueGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        Time.timeScale = 1f; // Mở lại thời gian bình thường
    }

    // ==========================================
    // NÚT 2: QUIT (QUAY LẠI SCENE 3)
    // ==========================================
    public void QuitToScene3()
    {
        Time.timeScale = 1f; // BẮT BUỘC reset thời gian về 1 trước khi load scene mới
        
        // Gọi theo Build Index
        SceneManager.LoadScene(scene3Index);
        
        // Hoặc nếu bạn muốn dùng theo tên chính xác của Scene 3, hãy dùng dòng dưới:
        // SceneManager.LoadScene("Tên_Scene_3_Của_Bạn");
    }
}