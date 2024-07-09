using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField, Tooltip("Remaining number of times player can spin the wheel")]
    TextMeshProUGUI turnsTextBox;

    [SerializeField, Tooltip("Total amount of winnings from player.")]
    TextMeshProUGUI winningsTextBox;

    [SerializeField, Tooltip("Fortune wheel.")]
    Transform fortuneWheel;

    [SerializeField, Tooltip("Fortune wheel.")]
    Button spinButton;

    [Space]
    [SerializeField, Tooltip("Number of times player can spin the wheel.")]
    int turns;

    [SerializeField, Tooltip("Number of full spins the wheel must complete.")]
    int cycles;

    [SerializeField, Tooltip("Number of second the wheel will spin.")]
    float maxSpinningTime;

    [SerializeField, Tooltip("Number of second to sleep the fame before jumpingg to Game Over.")]
    float gameOverSleep;

    private int _currentTurns;

    private int _winnings;
    private float[] _prizeAngles = new float[] { 1.0F, 46.0F, 91.0F, 136.0F, 181.0F, 226.0F, 271.0F, 316.0F } ;

    private float _startAngle;
    private float _finalAngle;

    private float _currentSpinningTime;
    private int _prizeAngle;


    private bool _isSpinning;
    private void Awake()
    {
        _currentTurns = turns;
        turnsTextBox.text = turns.ToString();
        winningsTextBox.text = _winnings.ToString("0.00");
    }

    private void Start()
    {
        _startAngle = _prizeAngles[Random.Range(0, _prizeAngles.Length)];
        fortuneWheel.eulerAngles = new Vector3(0.0F, 0.0F, _startAngle);
    }

    private void Update()
    {
        if (!_isSpinning) 
        {
            return;
        }

        _currentSpinningTime += Time.deltaTime;
        bool isTimeout = _currentSpinningTime > maxSpinningTime;
        bool hasReachFinalAngle = fortuneWheel.eulerAngles.z == _finalAngle;

        if (isTimeout || hasReachFinalAngle)
        {
            _currentSpinningTime = maxSpinningTime;
            _startAngle = _finalAngle % 360;
            fortuneWheel.eulerAngles = new Vector3(0.0F, 0.0F, _finalAngle);


            _isSpinning = false;

            int prize = GetPrize();
            _winnings += prize;
            winningsTextBox.text = "$" + _winnings.ToString("0.00");

            _currentTurns--;
            turnsTextBox.text = _currentTurns.ToString();



            if (_currentTurns == 0)
            {

                StartCoroutine(GameOver());
                return;

            }

            spinButton.interactable = true;

            return;
        }

        float t = _currentSpinningTime / maxSpinningTime;
        t = Mathf.Pow(t, 3.0F) * (t * (4.0F * t - 10.0F) + 8.0F);

        float angle = Mathf.Lerp(_startAngle, _finalAngle, t);

        fortuneWheel.eulerAngles = new Vector3 (0.0F, 0.0F, angle);

        

    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(gameOverSleep);
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LastLevel();
    }

    private int GetPrize()
    {
        switch (_prizeAngle)
        {
            case 5:
                return Random.Range(1500, 5000);

            case 0:
                return 1000;

            case 1:
            case 4:
                return 600;

            case 2:
            case 7:
                return 400;

            case 3:
            case 6:
                return 100;

        }

        return 0;

    }
           
    public void Spin()
    {

        spinButton.interactable = false;

        _prizeAngle = Random.Range(0, _prizeAngles.Length);

        float randomFinalAngle = _prizeAngles[_prizeAngle];
        _finalAngle = -(cycles * 360 + randomFinalAngle);
        _currentSpinningTime = 0.0F;
        _isSpinning = true;
    }


}
