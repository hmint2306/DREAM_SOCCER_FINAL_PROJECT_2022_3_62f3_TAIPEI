using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    [Header("--- 1. SCREEN SETTINGS ---")]
    public TMP_Dropdown resolutionDropdown; 
    public Toggle fullscreenToggle;          

    [Header("--- 2. SOUND SETTINGS ---")]
    public Slider soundSlider;               

    [Header("--- 3. BRIGHTNESS SETTINGS ---")]
    public Slider brightnessSlider;

    [Header("--- 4. LANGUAGE SETTINGS ---")]
    public TMP_Dropdown languageDropdown;     

    [Header("--- 5. PANEL & BUTTONS ---")]
    public GameObject settingsPanel;         

    private List<Resolution> filteredResolutions = new List<Resolution>();

    private IEnumerator Start()
    {
        SetupResolutions();

        // Chờ 1 frame đảm bảo GlobalSettings.Instance đã khởi tạo xong
        yield return null;

        // BỔ SUNG: Kiểm tra nếu GlobalSettings chưa tồn tại (khi test thẳng Scene này), tự động gắn Script GlobalSettings vào!
        if (GlobalSettings.Instance == null)
        {
            GameObject globalObj = new GameObject("GlobalSettings");
            globalObj.AddComponent<GlobalSettings>();
        }

        // BỔ SUNG: Đăng ký sự kiện lắng nghe khi kéo Brightness Slider
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }

        LoadSettings();
    }

    private void SetupResolutions()
    {
        if (resolutionDropdown == null) return;

        Resolution[] allResolutions = Screen.resolutions;
        filteredResolutions.Clear();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            bool isDuplicate = false;
            for (int j = 0; j < filteredResolutions.Count; j++)
            {
                if (allResolutions[i].width == filteredResolutions[j].width &&
                    allResolutions[i].height == filteredResolutions[j].height)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                filteredResolutions.Add(allResolutions[i]);
                string option = allResolutions[i].width + " x " + allResolutions[i].height;
                options.Add(option);

                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        if (savedResIndex >= filteredResolutions.Count) savedResIndex = 0;

        resolutionDropdown.value = savedResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (filteredResolutions == null || resolutionIndex >= filteredResolutions.Count) return;
        
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetSoundVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public void SetBrightness(float value)
    {
        if (GlobalSettings.Instance != null)
        {
            GlobalSettings.Instance.SetBrightness(value);
        }
    
        PlayerPrefs.SetFloat("Brightness", value);
    }

    public void SetLanguage(int langIndex)
    {
        // Gọi sang LanguageManager để đổi ngôn ngữ toàn bộ UI ngay lập tức
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.ChangeLanguage(langIndex);
        }
        else
        {
            PlayerPrefs.SetInt("Language", langIndex);
        }
    }

    public void OnClickSave()
    {
        PlayerPrefs.Save();
        Debug.Log("SETTINGS: Đã lưu cài đặt thành công!");
    }

    public void OnClickCancel()
    {
        LoadSettings(); 
        Debug.Log("SETTINGS: Đã hủy thay đổi!");
        SceneManager.LoadScene("00_MainMenu");
    }

    private void LoadSettings()
    {
        // 1. Fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;

        // 2. Sound
        float soundVol = PlayerPrefs.GetFloat("SoundVolume", 1f);
        if (soundSlider != null) soundSlider.value = soundVol;
        AudioListener.volume = soundVol;

        // 3. Brightness
        float brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        if (brightnessSlider != null) brightnessSlider.value = brightness;
        SetBrightness(brightness);

        // 4. Language
        int lang = PlayerPrefs.GetInt("Language", 0);
        if (languageDropdown != null) languageDropdown.value = lang;

        // 5. Resolution
        int savedResIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        if (resolutionDropdown != null && filteredResolutions.Count > 0)
        {
            if (savedResIndex < filteredResolutions.Count)
            {
                resolutionDropdown.value = savedResIndex;
                resolutionDropdown.RefreshShownValue();
                SetResolution(savedResIndex);
            }
        }
    }
}