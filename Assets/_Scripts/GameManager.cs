using UnityEngine;
using TMPro; // Đã có sẵn trong file của bạn để dùng TextMeshPro

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int homeTeamScore = 0;
    [SerializeField] private int awayTeamScore = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip goalSoundEffect;
    private AudioSource audioSource;

    [Header("Match Settings")]
    public float halfDuration = 45f; // Thời gian 1 hiệp (45 giây)
    private float currentTime;
    private int currentHalf = 1;
    private bool isMatchOver = false;

    [Header("References")]
    public Transform player1;
    public Transform player2;
    public Transform ball;
    [SerializeField] private TextMeshProUGUI timerText; // Text hiển thị thời gian

    // Biến lưu vị trí xuất phát
    private Vector2 p1StartPos;
    private Vector2 p2StartPos;
    private Vector2 ballStartPos;

    // Biến lưu Rigidbody để reset vận tốc
    private Rigidbody2D p1Rb;
    private Rigidbody2D p2Rb;
    private Rigidbody2D ballRb;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Lưu ý: Nếu bạn chỉ chơi trong 1 Scene và có lỗi mất object khi load lại, 
        // hãy thêm // vào trước dòng DontDestroyOnLoad bên dưới để tắt nó đi.
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateScoreUI();
    }

    private void Start()
    {
        // 1. Lưu lại vị trí ban đầu và lấy Rigidbody của 2 người chơi + quả bóng
        if (player1 != null)
        {
            p1StartPos = player1.position;
            p1Rb = player1.GetComponent<Rigidbody2D>();
        }

        if (player2 != null)
        {
            p2StartPos = player2.position;
            p2Rb = player2.GetComponent<Rigidbody2D>();
        }

        if (ball != null)
        {
            ballStartPos = ball.position;
            ballRb = ball.GetComponent<Rigidbody2D>();
        }

        // 2. Đặt thời gian bắt đầu
        currentTime = halfDuration;
    }

    private void Update()
    {
        // Nếu trận đấu kết thúc, dừng đếm giờ
        if (isMatchOver) return;

        // Trừ lùi thời gian
        currentTime -= Time.deltaTime;

        // Cập nhật UI thời gian
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = seconds.ToString();
        }

        // Kiểm tra hết giờ
        if (currentTime <= 0)
        {
            EndHalf();
        }
    }

    private void EndHalf()
    {
        if (currentHalf == 1)
        {
            // Hết hiệp 1 -> Reset game và chuyển sang hiệp 2
            currentHalf = 2;
            currentTime = halfDuration;
            Debug.Log("Hết hiệp 1! Bắt đầu hiệp 2!");
            ResetPositions();
        }
        else if (currentHalf == 2)
        {
            // Hết hiệp 2 -> Kết thúc trận
            currentTime = 0;
            isMatchOver = true;
            Debug.Log("Hết giờ! Kết thúc trận đấu!");

            if (timerText != null) timerText.text = "END";

            // Dừng mọi chuyển động
            if (p1Rb != null) p1Rb.velocity = Vector2.zero;
            if (p2Rb != null) p2Rb.velocity = Vector2.zero;
            if (ballRb != null) ballRb.velocity = Vector2.zero;
        }
    }

    public void ResetPositions()
    {
        // Reset lại vị trí
        if (player1 != null) player1.position = p1StartPos;
        if (player2 != null) player2.position = p2StartPos;
        if (ball != null) ball.position = ballStartPos;

        // Reset vận tốc
        if (p1Rb != null) p1Rb.velocity = Vector2.zero;
        if (p2Rb != null) p2Rb.velocity = Vector2.zero;

        if (ballRb != null)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    public void ScoreGoal(GoalZone.GoalSide team)
    {
        // Nếu trận đấu đã kết thúc, không ghi nhận bàn thắng nữa
        if (isMatchOver) return;

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

        // Tùy chọn: Đưa người chơi và bóng về giữa sân sau mỗi lần ghi bàn 
        // Bỏ dấu // ở dòng bên dưới nếu bạn muốn bật tính năng này
         ResetPositions();
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