using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [Header("Nội dung theo ngôn ngữ")]
    [TextArea] public string vietnameseText; // Nhập chữ Tiếng Việt ở Inspector
    [TextArea] public string englishText;    // Nhập chữ Tiếng Anh ở Inspector

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (textComponent == null) return;

        int lang = PlayerPrefs.GetInt("Language", 0);
        
        if (lang == 1) // Tiếng Việt
        {
            textComponent.text = vietnameseText;
        }
        else if (lang == 0) // Tiếng Anh
        {
            textComponent.text = englishText;
        }
    }
}