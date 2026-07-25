using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [SerializeField] private int homeTeamScore = 0;
    [SerializeField] private int awayTeamScore = 0;

    // UI References
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip goalSoundEffect;
    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateScoreUI();
    }

    public void ScoreGoal(GoalZone.GoalSide team)
    {
        if (team == GoalZone.GoalSide.Home)
        {
            homeTeamScore++;
        }
        else
        {
            awayTeamScore++;
        }

        UpdateScoreUI();
        PlayGoalSound();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{homeTeamScore} - {awayTeamScore}";
        }
        else
        {
            Debug.LogWarning("Score Text không được gán trong Inspector!");
        }
    }

    private void PlayGoalSound()
    {
        if (audioSource != null && goalSoundEffect != null)
        {
            audioSource.PlayOneShot(goalSoundEffect);
        }
    }

    public int GetHomeScore() => homeTeamScore;
    public int GetAwayScore() => awayTeamScore;

    public void ResetScore()
    {
        homeTeamScore = 0;
        awayTeamScore = 0;
        UpdateScoreUI();
    }
}