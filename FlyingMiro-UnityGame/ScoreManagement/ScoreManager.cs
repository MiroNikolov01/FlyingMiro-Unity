using FlappyBirdScripts.AudioManagement;
using FlappyBirdScripts.GameManagement;
using TMPro;
using UnityEngine;

namespace FlappyBirdScripts.ScoreManagement
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        public TextMeshProUGUI scoreValueText;
        public TextMeshProUGUI bestValueText;

        private int _currentScore;
        private int _bestScore;
        public int GetCurrentScore() => this._currentScore;

        private void Awake()
        {
            if (instance == null) instance = this;

            _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        }

        private void Start()
        {
            _currentScore = -2;
            UpdateScoreText();
            UpdateBestScoreText();
        }

        public void UpdateScore()
        {
            if (GameManager.instance.isGameOver) return;
            _currentScore++;
            UpdateScoreText();

            if (_currentScore > _bestScore)
            {
                _bestScore = _currentScore;
                PlayerPrefs.SetInt("BestScore", _bestScore);
                UpdateBestScoreText();
            }
        }

        private void UpdateScoreText()
        {
            scoreValueText.text = _currentScore switch
            {
                -1 or -2 => "0",
                _ => _currentScore.ToString(),
            };
            if (_currentScore > 0) AudioManager.instance.PlayScoreSound();
        }

        private void UpdateBestScoreText()
        {
            bestValueText.text = _bestScore.ToString();
        }
    }
}