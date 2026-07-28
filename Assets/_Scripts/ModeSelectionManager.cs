using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectionManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string mainMenuSceneName = "00_MainMenu";
    public string gameplaySceneName = "16_Ball_Bounce"; // Hoặc đổi thành int nếu bạn dùng Build Index

    // Hàm gọi khi bấm nút Back
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hàm gọi khi bấm chọn 1 PLAYER
    public void SelectOnePlayerMode()
    {
        // Lưu giá trị 1 vào bộ nhớ với từ khóa "GameMode" (1 = Chơi với máy)
        PlayerPrefs.SetInt("GameMode", 1);
        PlayerPrefs.Save();
        
        // Load Scene 16
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Hàm gọi khi bấm chọn 2 PLAYERS
    public void SelectTwoPlayerMode()
    {
        // Lưu giá trị 2 vào bộ nhớ với từ khóa "GameMode" (2 = Hai người chơi)
        PlayerPrefs.SetInt("GameMode", 2);
        PlayerPrefs.Save();
        
        // Load Scene 16
        SceneManager.LoadScene(gameplaySceneName);
    }
}