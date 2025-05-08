using System.Collections;
using FlappyBirdScripts.AudioManagement;
using FlappyBirdScripts.FlyObjectManager;
using FlappyBirdScripts.MovingScripts;
using FlappyBirdScripts.PipeManager;
using FlappyBirdScripts.ScoreManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FlappyBirdScripts.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [Header("Scores")] public GameObject bestScorePanel;

        [Header("Tittles")] public GameObject titleScreen;
        public GameObject gameOverScreen;
        public GameObject billboardBas;
        public GameObject finishScreen;
        public GameObject titleScreenFinish;

        [Header("Flash screen")] public Image flashPanel;
        public float flashDuration = 0.1f;

        [Header("Buttons")] public Button playButton;
        public GameObject volumeButton;
        public Button playAgainButton;

        [Header("Fly object")] public FlyBehaviour flappyMiroObject;

        [Header("Particles - Confetti")] public ParticleSystem smokeTrail;
        public ParticleSystem confetti1, confetti2, confetti3;
        public ParticleSystem fireworkParticles;

        [Header("Pipe movement")] public float currentPipeMoveSpeed = 1f;
        public float currentPipeMoveRange = 1f;


        [Header("Click pulse start menu")] [SerializeField]
        private ClickToStartEffect clickToStartEffect;

        public GameObject clickToStartEffectObject;

        public static bool hasStarted;
        public bool isGameOver;
        private int _oncePlayedWinSound = 1;

        private void Awake()
        {
            if (instance == null) instance = this;
            clickToStartEffect = FindObjectOfType<ClickToStartEffect>();
            Time.timeScale = 0f;
        }

        private void Start()
        {
            AdjustConfettiParticles("Clear");
            AdjustConfettiParticles("Play");

            if (flashPanel != null)
            {
                flashPanel.gameObject.SetActive(false);
            }

            PauseGame();

            this.playAgainButton.onClick.AddListener(RestartGame);

            //Start menu objects - screen
            this.clickToStartEffectObject.SetActive(true);
            this.titleScreen.SetActive(true);
            this.gameOverScreen.SetActive(false);
            this.volumeButton.SetActive(true);
            this.bestScorePanel.SetActive(true);
            this.finishScreen.SetActive(false);
            this.playAgainButton.gameObject.SetActive(true);
        }

        public void Update()
        {
            int currentScore = ScoreManager.instance.GetCurrentScore();

            AdjustDifficulty(currentScore);

            if (currentScore == 200)
            {
                TriggerGameFinishScreen();
            }
        }

        void AdjustDifficulty(int score)
        {
            float newTimeScale = Time.timeScale;
            float? newPipeMoveSpeed = null;
            float? newPipeMoveRange = null;

            switch (score)
            {
                case 20:
                    newTimeScale = 4.6f;
                    break;
                case 40:
                    newPipeMoveRange = 1.8f;
                    break;
                case 60:
                    newPipeMoveSpeed = 1.4f;
                    newPipeMoveRange = 2f;
                    break;
                case 80:
                    newPipeMoveSpeed = 1.7f;
                    newPipeMoveRange = 2.5f;
                    break;
                case 100:
                    newPipeMoveRange = 2.6f;
                    break;
                case 120:
                    newPipeMoveSpeed = 2f;
                    newPipeMoveRange = 2.7f;
                    break;
                case 140:
                    newPipeMoveSpeed = 2.5f;
                    newPipeMoveRange = 2.8f;
                    break;
                case 160:
                    newPipeMoveSpeed = 2.7f;
                    newPipeMoveRange = 2.9f;
                    break;
                case 180:
                    newPipeMoveSpeed = 2.8f;
                    newPipeMoveRange = 3f;
                    break;
            }

            Time.timeScale = newTimeScale;

            if (newPipeMoveSpeed.HasValue)
                this.currentPipeMoveSpeed = newPipeMoveSpeed.Value;

            if (newPipeMoveRange.HasValue)
                this.currentPipeMoveRange = newPipeMoveRange.Value;
        }

        public void StartGame()
        {
            this.smokeTrail.Play();
            this.isGameOver = false;
            AudioManager.instance.UpdateVolumes();

            if (hasStarted) return;
            hasStarted = true;
            AdjustConfettiParticles("Clear");
            AdjustConfettiParticles("Play");

            this.clickToStartEffectObject.SetActive(false);
            titleScreen.SetActive(false);
            billboardBas.SetActive(false);
            volumeButton.SetActive(false);
            bestScorePanel.SetActive(false);
            finishScreen.SetActive(false);

            flappyMiroObject.UnfreezePlayer();

            playAgainButton.gameObject.SetActive(false);
            ResumeGame();
        }

        public void GameOver()
        {
            ClearSmokeTrailEffect();
            FlashScreen();

            isGameOver = true;
            gameOverScreen.SetActive(true);
            playAgainButton.gameObject.SetActive(true);
            finishScreen.SetActive(false);

            GameObject gameOverImage = GameObject.Find("GameOverScreen");
            ShakeEffect shakeEffect = gameOverImage.GetComponent<ShakeEffect>();

            if (shakeEffect != null)
            {
                shakeEffect.TriggerShake();
            }

            ShowGameOverScreen();
        }

        private void ShowGameOverScreen()
        {
            gameOverScreen.SetActive(true);
            playAgainButton.gameObject.SetActive(true);
        }

        public void RestartGame()
        {
            hasStarted = false;
            volumeButton.SetActive(true);

            AdjustConfettiParticles("Clear");

            AudioManager.instance.UpdateMuteButtonSprite();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        Application.OpenURL("https://miro-nikolov.itch.io/flappymiro");
#else
        Application.Quit();
#endif
        }

        private void PauseGame() => Time.timeScale = 0f;

        private void ResumeGame() => Time.timeScale = 4.5f;

        private void AdjustConfettiParticles(string decision)
        {
            ParticleSystem[] confettiParticles = { confetti1, confetti2, confetti3 };
            if (decision is "Clear")
            {
                StartCoroutine(FullConfettiCleanup());
            }

            foreach (var confetti in confettiParticles)
            {
                if (decision is "Play")
                {
                    confetti.Play();
                    StartCoroutine(PlayConfettiDelayed(confetti, 0.1f));
                }
            }
        }

        private IEnumerator FullConfettiCleanup(float initialDelay = 0.1f, float cleanDelay = 1f)
        {
            ParticleSystem[] confettiParticles = { confetti1, confetti2, confetti3 };

            yield return new WaitForSecondsRealtime(initialDelay);

            foreach (var confetti in confettiParticles)
            {
                if (confetti == null) continue;
                confetti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(cleanDelay);

            foreach (var confetti in confettiParticles)
            {
                if (confetti == null) continue;
                confetti.Clear(true);
                yield return null;
            }

            foreach (var confetti in confettiParticles)
            {
                if (confetti == null) continue;
                confetti.gameObject.SetActive(true);
                confetti.Play();
                yield return null;
            }
        }


        private IEnumerator PlayConfettiDelayed(ParticleSystem confetti, float delay)
        {
            yield return new WaitForSeconds(delay);
            confetti.Play();
        }

        private void TriggerGameFinishScreen()
        {
            Time.timeScale = 0f;

            if (_oncePlayedWinSound == 1)
            {
                AudioManager.instance.PlayWinFinalLevelSound();
                StartCoroutine(PlayFinalMessageWithDelay());
                AudioManager.instance.bgmSource.Stop();
            }

            _oncePlayedWinSound++;

            finishScreen.SetActive(true);
            titleScreenFinish.SetActive(true);
            gameOverScreen.SetActive(false);
            billboardBas.SetActive(false);
            volumeButton.SetActive(false);

            playAgainButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
        }

        private IEnumerator PlayFinalMessageWithDelay()
        {
            yield return new WaitForSecondsRealtime(4.0f);

            MouthAnimator mouthAnimator = FindObjectOfType<MouthAnimator>();
            if (mouthAnimator != null)
            {
                mouthAnimator.StartTalkingAnimation();
            }
        }

        private void ClearSmokeTrailEffect()
            => smokeTrail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        private void FlashScreen()
            => StartCoroutine(FlashRoutine());

        private IEnumerator FlashRoutine()
        {
            flashPanel.gameObject.SetActive(true);
            flashPanel.color = new Color(1f, 1f, 1f, 1f);

            if (flashPanel.GetComponent<CanvasGroup>() == null)
            {
                CanvasGroup canvasGroup = flashPanel.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                CanvasGroup canvasGroup = flashPanel.GetComponent<CanvasGroup>();
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            yield return new WaitForSecondsRealtime(flashDuration);

            // Fade out the flash
            float fadeTime = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                flashPanel.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            flashPanel.gameObject.SetActive(false);
        }

        public void TriggerFireworksAndHide()
        {
            fireworkParticles.Play();

            StartCoroutine(HideFireworksAfterDelay(3f));
        }

        private IEnumerator HideFireworksAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            fireworkParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}