using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    private string playSceneName = "01_Adding_BackGround"; // Tên Scene màn chơi chính
    private string settingsSceneName = "02_Settings"; // Tên Scene màn hình cài đặt

    // Gọi khi bấm nút PLAY
    public void OnClickPlay()
    {
        // Tải scene màn chơi (Nhớ add Scene vào Build Settings)
        SceneManager.LoadScene(playSceneName);
    }

    public void OnClickSettings()
    {
        // Tải scene màn hình cài đặt (Nhớ add Scene vào Build Settings)
        SceneManager.LoadScene(settingsSceneName);
    }

    // Gọi khi bấm nút QUIT
    public void OnClickQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Dừng Play trong Editor
        #else
            Application.Quit(); // Đóng game khi build
        #endif
    }

}