using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int homeTeamScore = 0;
    [SerializeField] private int awayTeamScore = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip goalSoundEffect;
    private AudioSource audioSource;

    [Header("Match Settings")]
    public float halfDuration = 45f; 
    public float goalResetDelay = 1.2f; // Thời gian chờ ăn mừng bàn thắng
    public float startDelay = 1.5f;      // Thời gian đứng im 1.5s chờ bắt đầu/đá lại

    private float currentTime;
    private int currentHalf = 1;
    private bool isMatchOver = false;
    private bool isResettingGoal = false;
    private bool isGameActive = false;   // Kiểm tra xem quả bóng có đang trong cuộc hay không

    [Header("References")]
    public Transform player1;
    public Transform player2;
    public Transform ball;
    [SerializeField] private TextMeshProUGUI timerText;

    private Vector2 p1StartPos;
    private Vector2 p2StartPos;
    private Vector2 ballStartPos;

    private Rigidbody2D p1Rb;
    private Rigidbody2D p2Rb;
    private Rigidbody2D ballRb;

    private PlayerController p1Controller;
    private PlayerController p2Controller;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateScoreUI();
    }

    private void Start()
    {
        if (player1 != null)
        {
            p1StartPos = player1.position;
            p1Rb = player1.GetComponent<Rigidbody2D>();
            p1Controller = player1.GetComponent<PlayerController>();
        }

        if (player2 != null)
        {
            p2StartPos = player2.position;
            p2Rb = player2.GetComponent<Rigidbody2D>();
            p2Controller = player2.GetComponent<PlayerController>();
        }

        if (ball != null)
        {
            ballStartPos = ball.position;
            ballRb = ball.GetComponent<Rigidbody2D>();
        }

        currentTime = halfDuration;

        // Vừa vào game -> Đứng im 1.5s rồi mới bắt đầu đá
        StartCoroutine(StartRoundRoutine());
    }

    private void Update()
    {
        // Chỉ đếm ngược thời gian khi game đang thực sự diễn ra
        if (isMatchOver || !isGameActive) return;

        currentTime -= Time.deltaTime;

        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(Mathf.Max(0, currentTime));
            timerText.text = seconds.ToString();
        }

        if (currentTime <= 0)
        {
            EndHalf();
        }
    }

    private void EndHalf()
    {
        if (currentHalf == 1)
        {
            currentHalf = 2;
            currentTime = halfDuration;
            Debug.Log("📢 Hết hiệp 1! Bắt đầu hiệp 2!");

            // Reset vị trí & chờ 1.5s chuẩn bị đá hiệp 2
            ResetPositions();
            StartCoroutine(StartRoundRoutine());
        }
        else if (currentHalf == 2)
        {
            currentTime = 0;
            isMatchOver = true;
            isGameActive = false;
            Debug.Log("📢 Hết giờ! Kết thúc trận đấu!");

            if (timerText != null) timerText.text = "END";

            // Khóa hoàn toàn cầu thủ khi hết giờ
            ResetPositions();
            FreezeAllPlayers();
        }
    }

    public void ScoreGoal(GoalZone.GoalSide team)
    {
        if (isMatchOver || isResettingGoal || !isGameActive) return;

        isResettingGoal = true;
        isGameActive = false; // Tạm dừng đồng hồ đếm ngược

        // DỪNG LẬP TỨC PLAYER & AI KHÔNG CHO CHẠY LUNG TUNG
        FreezeAllPlayers();

        // Cộng điểm
        if (team == GoalZone.GoalSide.Home)
        {
            awayTeamScore++;
            Debug.Log("⚽ GOOOAL! Bàn thắng cho đội AWAY!");
        }
        else
        {
            homeTeamScore++;
            Debug.Log("⚽ GOOOAL! Bàn thắng cho đội HOME!");
        }

        UpdateScoreUI();
        PlayGoalSound();

        // Tiến hành Reset vị trí và đếm lùi lượt đá mới
        StartCoroutine(GoalResetRoutine());
    }

    private IEnumerator GoalResetRoutine()
    {
        // 1. Chờ 1.2s ngắn để xem bàn thắng / âm thanh
        yield return new WaitForSeconds(goalResetDelay);

        // 2. Reset vị trí bóng + người chơi
        ResetPositions();

        // 3. Đứng im 1.5s đếm lùi trước khi bắt đầu lượt mới
        yield return StartCoroutine(StartRoundRoutine());

        // 4. Bật lại vùng kiểm tra GoalZone
        GoalZone[] zones = FindObjectsOfType<GoalZone>();
        foreach (var zone in zones)
        {
            zone.ResetGoalZone();
        }

        isResettingGoal = false;
    }

    // Coroutine đếm lùi 1.5s trước khi thả cho chạy
    private IEnumerator StartRoundRoutine()
    {
        isGameActive = false;

        // Khóa di chuyển & triệt tiêu vận tốc
        FreezeAllPlayers();
        StopAllMovement();

        // Chờ 1.5s
        yield return new WaitForSeconds(startDelay);

        // Mở khóa cho đá tiếp
        if (!isMatchOver)
        {
            UnfreezeAllPlayers();
            isGameActive = true;
        }
    }

    public void ResetPositions()
    {
        if (player1 != null) player1.position = p1StartPos;
        if (player2 != null) player2.position = p2StartPos;
        if (ball != null) ball.position = ballStartPos;

        StopAllMovement();
    }

    private void StopAllMovement()
    {
        if (p1Rb != null) p1Rb.velocity = Vector2.zero;
        if (p2Rb != null) p2Rb.velocity = Vector2.zero;

        if (ballRb != null)
        {
            ballRb.velocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
    }

    private void FreezeAllPlayers()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var p in players)
        {
            p.FreezePlayer();
        }
    }

    private void UnfreezeAllPlayers()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var p in players)
        {
            p.UnfreezePlayer();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{homeTeamScore} - {awayTeamScore}";
        }
    }

    private void PlayGoalSound()
    {
        if (goalSoundEffect != null)
        {
            // Gọi AudioManager dập nhạc nền và kích hoạt hiệu ứng GOAL
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGoalSFXWithDucking(goalSoundEffect);
            }
            else if (audioSource != null)
            {
                // Dự phòng nếu không có AudioManager
                audioSource.PlayOneShot(goalSoundEffect);
            }
        }
    }
}