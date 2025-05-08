using System.Collections;
using FlappyBirdScripts.AudioManagement;
using FlappyBirdScripts.ScoreManagement;
using TMPro;
using UnityEngine;

namespace FlappyBirdScripts.GameManagement
{
    public class ConfettiController : MonoBehaviour
    {
        public ParticleSystem confettiEffect1;
        public ParticleSystem confettiEffect2;
        
        public TextMeshProUGUI congratsTextAnimation;

        public int pointsToCelebrate = 20;

        private Vector3 _originalTextPosition;
        private int _lastCelebratedScore = 0;

        void Start()
        {
            if (confettiEffect1 != null && confettiEffect2 != null)
            {
                this.confettiEffect1.Stop();
                this.confettiEffect2.Stop();
            }

            if (congratsTextAnimation != null)
            {
                this._originalTextPosition = this.congratsTextAnimation.rectTransform.anchoredPosition;
            }
            else
            {
                Debug.LogError("Text component not assigned to TextAnimator!");
            }

            this.congratsTextAnimation.gameObject.SetActive(false);

            ShowStageIntroText();
        }

        void Update()
        {
            int currentScore = ScoreManager.instance.GetCurrentScore();

            if (currentScore >= this.pointsToCelebrate &&
                currentScore % this.pointsToCelebrate == 0 &&
                currentScore != this._lastCelebratedScore)
            {
                this._lastCelebratedScore = currentScore;
                StartCoroutine(ShowSpecialText());
            }
            if (currentScore == 115)
            {
                StartCoroutine(ShowTextPrepareForStage2());
            }
        }

        private IEnumerator ShowSpecialText()
        {
            AudioManager.instance.PlayLevelUpSound();

            this.confettiEffect1.Play();
            this.confettiEffect2.Play();
            
            this.congratsTextAnimation.gameObject.SetActive(true);
            
            int currentScore = ScoreManager.instance.GetCurrentScore();

            int level = (currentScore / 20) + 1;

            if (currentScore == 120)
            {
                this.congratsTextAnimation.text = $"Stage 2 - Level {level}";
            }
            else
            {
                this.congratsTextAnimation.text = $"Congrats Level - {level}";
            }

            yield return JumpAndFadeText();

            yield return new WaitForSeconds(1.5f);
            
            yield return FallAndFadeText();

            this.confettiEffect1.Stop();
            this.confettiEffect2.Stop();
        }

        public void AnimateText()
        {
            StartCoroutine(TextAnimationSequence());
        }

        private IEnumerator TextAnimationSequence()
        {
            yield return StartCoroutine(JumpAndFadeText());
            yield return StartCoroutine(FallAndFadeText());
        }

        private IEnumerator JumpAndFadeText()
        {
            // Reset the text position and opacity before starting the animation
            this.congratsTextAnimation.rectTransform.anchoredPosition = this._originalTextPosition;
            this.congratsTextAnimation.color =
                new Color(this.congratsTextAnimation.color.r, this.congratsTextAnimation.color.g, this.congratsTextAnimation.color.b,
                    0f); // Start transparent
            this.congratsTextAnimation.gameObject.SetActive(true);

            Vector3 startPos = this.congratsTextAnimation.rectTransform.anchoredPosition;
            Vector3 targetPos = startPos + new Vector3(-1, 1f, 0); // Move up and slightly left
            float duration = 5f;
            float timeElapsed = 0f;

            // Fade in the text
            Color startColor = this.congratsTextAnimation.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Full opacity

            while (timeElapsed < duration)
            {
                this.congratsTextAnimation.rectTransform.anchoredPosition =
                    Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
                this.congratsTextAnimation.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure it ends at the target position
            this.congratsTextAnimation.rectTransform.anchoredPosition = targetPos;
            this.congratsTextAnimation.color = targetColor;
        }

        private IEnumerator FallAndFadeText()
        {
            Vector3 startPos = this.congratsTextAnimation.rectTransform.anchoredPosition;
            Vector3 targetPos = this._originalTextPosition - new Vector3(0, 50f, 0); // Move down below original position
            float duration = 5f;
            float timeElapsed = 0f;

            // Fade out the text while falling
            Color startColor = this.congratsTextAnimation.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Transparent

            while (timeElapsed < duration)
            {
                this.congratsTextAnimation.rectTransform.anchoredPosition =
                    Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
                this.congratsTextAnimation.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure it ends at the target position and becomes transparent
            this.congratsTextAnimation.rectTransform.anchoredPosition = targetPos;
            this.congratsTextAnimation.color = targetColor;

            // Hide the text after the effect
            this.congratsTextAnimation.gameObject.SetActive(false);

            // Reset position while hidden (important for next animation)
            this.congratsTextAnimation.rectTransform.anchoredPosition = _originalTextPosition;
        }

        private void ShowStageIntroText()
            => StartCoroutine(ShowIntroTextSequence());

        private IEnumerator ShowIntroTextSequence()
        {
            // First text: Level
            this.congratsTextAnimation.text = $"Stage 1 - Level 1";
            this.congratsTextAnimation.gameObject.SetActive(true);
            
            yield return StartCoroutine(JumpAndFadeText()); 
            yield return new WaitForSeconds(1f);

            // Second text: Score 200 points to win
            this.congratsTextAnimation.text = "Can you reach 200 points to win?";
            
            yield return StartCoroutine(JumpAndFadeText()); // Show with jump/fade effect
            yield return new WaitForSeconds(1f);

            // Third text: Good Luck
            this.congratsTextAnimation.text = "Good Luck!";
            
            yield return StartCoroutine(JumpAndFadeText()); // Show with jump/fade effect
            yield return new WaitForSeconds(1.5f); 
            yield return FallAndFadeText(); 
        }

        private IEnumerator ShowTextPrepareForStage2()
        {
            this.congratsTextAnimation.text = $"Stage 2 Awaits.. Get Ready!";
            this.congratsTextAnimation.gameObject.SetActive(true);
            
            yield return StartCoroutine(JumpAndFadeText());
            yield return new WaitForSeconds(9f);
            yield return FallAndFadeText();
        }
    }
}