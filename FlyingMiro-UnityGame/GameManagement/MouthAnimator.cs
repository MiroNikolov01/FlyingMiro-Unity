using System.Collections;
using FlappyBirdScripts.AudioManagement;
using UnityEngine;

namespace FlappyBirdScripts.GameManagement
{
    public class MouthAnimator : MonoBehaviour
    {
        public Sprite mouthClosed;
        public Sprite mouthHalfOpen;

        public float changeRate = 0.2f;

        private SpriteRenderer _spriteRenderer;
        private Sprite[] _mouthStates;
        private Coroutine _mouthCoroutine;
        
        private int _currentIndex = 0;
        private bool _isAnimating = false;

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (!_spriteRenderer)
            {
                Debug.LogError("No SpriteRenderer attached to " + gameObject.name);
                return;
            }

            if (mouthClosed == null || mouthHalfOpen == null)
            {
                Debug.LogError("One or more mouth sprites not assigned!");
                return;
            }

            _mouthStates = new Sprite[] { mouthClosed, mouthHalfOpen };
        
            _spriteRenderer.sprite = mouthClosed;
        }
    
        public void StartTalkingAnimation()
        {
            if (_isAnimating && _mouthCoroutine != null)
            {
                StopCoroutine(_mouthCoroutine);
            }
        
            _mouthCoroutine = StartCoroutine(TalkWithAudio());
        }
    
        private IEnumerator TalkWithAudio()
        {
            _isAnimating = true;
        
            StartCoroutine(AnimateMouth());
        
            AudioClip finalMessage = AudioManager.instance.PlayThanksFinalMessage();
        
            float messageLength = finalMessage != null ? finalMessage.length : 3.0f;
        
            yield return new WaitForSecondsRealtime(messageLength);
        
            StopTalking();
        }

        private IEnumerator AnimateMouth()
        {
            while (this._isAnimating)
            {
                if (this._mouthStates.Length == 0 || this._spriteRenderer == null)
                {
                    Debug.LogWarning("MouthAnimator stopped: missing data");
                    yield break;
                }

                this._spriteRenderer.sprite = _mouthStates[_currentIndex];

                this._currentIndex = (_currentIndex + 1) % _mouthStates.Length;

                yield return new WaitForSecondsRealtime(changeRate);
            }
        }
    
        private void StopTalking()
        {
            this._isAnimating = false;
        
            if (this._mouthCoroutine != null)
            {
                StopCoroutine(this._mouthCoroutine);
                this._mouthCoroutine = null;
            }
        
            if (this._spriteRenderer != null && this.mouthClosed != null)
            {
                this._spriteRenderer.sprite = mouthClosed;
            }
        }
    }
}