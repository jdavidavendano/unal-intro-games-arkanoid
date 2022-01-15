using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkanoidController : MonoBehaviour
{
    private const string BALL_PREFAB_PATH = "Prefabs/Ball";
    private const string POWER_UP_PREFAB_PATH = "Prefabs/PowerUp";

    private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);

    [SerializeField]
    private GridController _gridController;

    [Space(20)]
    [SerializeField]
    private List<LevelData> _levels = new List<LevelData>();

    private int _currentLevel = 0;

    private Ball _ballPrefab = null;
    private PowerUps _powerUpPrefab = null;
    private List<Ball> _balls = new List<Ball>();
    private int _totalScore = 0;
    [SerializeField]
    private int probabilityOfPowerUp = 30;
    public Transform PowerUp;

    public GameObject paddle;
    public GameObject paddleBody;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitGame();
        }
    }

    private void resetPaddleSize()
    {
        paddle.transform.localScale = new Vector3(1f, 1f, 1f);
        paddleBody.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
    private void InitGame()
    {
        _currentLevel = 0;
        _totalScore = 0;

        ArkanoidEvent.OnGameStartEvent?.Invoke();
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
        ArkanoidEvent.OnLevelUpdatedEvent?.Invoke(_currentLevel);

        _gridController.BuildGrid(_levels[0]);
        SetInitialBall();
        resetPaddleSize();
    }
    public void SetMultipleBalls(int missingBalls)
    {
        for (int i = 0; i < missingBalls; i++)
        {
            Ball ball = CreateBallAt(_balls[0].transform.position);
            ball.Init();
            _balls.Add(ball);
        }
    }

    private void SetInitialBall()
    {
        ClearBalls();

        Ball ball = CreateBallAt(BALL_INIT_POSITION);
        ball.Init();
        _balls.Add(ball);
    }

    private Ball CreateBallAt(Vector2 position)
    {
        if (_ballPrefab == null)
        {
            _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
        }

        return Instantiate(_ballPrefab, position, Quaternion.identity);
    }

    private void ClearBalls()
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            _balls[i].gameObject.SetActive(false);
            Destroy(_balls[i]);
        }

        _balls.Clear();
    }
    private void Start()
    {
        paddle = GameObject.Find("Paddle");
        paddleBody = GameObject.Find("PaddleBody");

        ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
        ArkanoidEvent.GetPowerUpEvent += GetPowerUp;
    }

    private void OnDestroy()
    {
        ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
        ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
    }
    private void OnBallReachDeadZone(Ball ball)
    {
        ball.Hide();
        _balls.Remove(ball);
        Destroy(ball.gameObject);

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        //Game over
        if (_balls.Count == 0)
        {
            ClearBalls();

            Debug.Log("Game Over: LOSE!!!");
            ArkanoidEvent.OnGameOverEvent?.Invoke();
        }
    }

    private bool ShouldSpawnPowerUp()
    {
        return Random.Range(0, 100) < probabilityOfPowerUp;
    }

    private void OnBlockDestroyed(int blockId)
    {
        BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);
        if (blockDestroyed != null)
        {

            _totalScore += blockDestroyed.Score;
            ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(blockDestroyed.Score, _totalScore);
            // Spawn power up
            if (ShouldSpawnPowerUp())
            {
                _powerUpPrefab = Resources.Load<PowerUps>(POWER_UP_PREFAB_PATH);
                int randomTypePower = Random.Range(1, 6);

                _powerUpPrefab.SetData(randomTypePower);
                _powerUpPrefab.Init();
                Instantiate(_powerUpPrefab, blockDestroyed.transform.position, blockDestroyed.transform.rotation);


            }
        }

        if (_gridController.GetBlocksActive() == 0)
        {
            _currentLevel++;
            resetPaddleSize();
            if (_currentLevel >= _levels.Count)
            {
                ClearBalls();
                Debug.LogError("Game Over: WIN!!!!");
            }
            else
            {
                SetInitialBall();
                _gridController.BuildGrid(_levels[_currentLevel]);
                ArkanoidEvent.OnLevelUpdatedEvent?.Invoke(_currentLevel);
            }

        }
    }


    private void GetPowerUp(PowerUps powerUpComponent)
    {
        /*
            types:
                1: slow
                2: fast
                3: multiple balls
                4: small
                5: large
        */

        if (powerUpComponent._type == 1)
        {
            foreach (var ball in _balls)
            {
                ball.changeVelocity("slow");

            }
        }
        else if (powerUpComponent._type == 2)
        {
            foreach (var ball in _balls)
            {
                ball.changeVelocity("fast");

            }
        }
        else if (powerUpComponent._type == 3)
        {

            SetMultipleBalls(3 - _balls.Count);
        }
        else if (powerUpComponent._type == 4)
        {
            // Minimal size, avoid dissapear
            if (paddle.transform.localScale.x <= 0.5f)
            {
                paddle.transform.localScale = new Vector3(0.5f, 1f, 1f);
                paddleBody.transform.localScale = new Vector3(0.3f, 0.8f, 0.8f);
            }
            else
            {
                paddle.transform.localScale -= new Vector3(0.2f, 0, 0);
                paddleBody.transform.localScale -= new Vector3(0.2f, 0, 0);
            }

        }
        else if (powerUpComponent._type == 5)
        {
            paddle.transform.localScale += new Vector3(0.2f, 0, 0);
            paddleBody.transform.localScale += new Vector3(0.2f, 0, 0);
        }
        powerUpComponent.Hide();
        Destroy(powerUpComponent.gameObject);
    }
}