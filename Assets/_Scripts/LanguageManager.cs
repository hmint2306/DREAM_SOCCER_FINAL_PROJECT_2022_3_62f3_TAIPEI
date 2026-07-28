using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    // 1 = Tiếng Việt, 0 = Tiếng Anh (Tùy theo thứ tự trong TMP_Dropdown của bạn)
    public int currentLanguage = 0; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Đọc ngôn ngữ đã lưu (Mặc định là 0 - Tiếng Việt)
        currentLanguage = PlayerPrefs.GetInt("Language", 0);
    }

    public void ChangeLanguage(int langIndex)
    {
        currentLanguage = langIndex;
        PlayerPrefs.SetInt("Language", langIndex);

        // Báo cho tất cả các văn bản UI trong Scene tự cập nhật lại chữ
        LocalizedText[] allTexts = FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText text in allTexts)
        {
            text.UpdateText();
        }
    }
}