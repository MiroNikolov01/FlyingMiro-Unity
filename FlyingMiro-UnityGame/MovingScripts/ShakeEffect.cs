using System.Collections;
using UnityEngine;

namespace FlappyBirdScripts.MovingScripts
{
    public class ShakeEffect : MonoBehaviour
    {
        public float shakeDuration = 0.5f;  
        public float shakeMagnitude = 0.1f; 

        private Vector3 _originalPosition;  
        private RectTransform _rectTransform;  

        private void OnEnable()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
                _originalPosition = _rectTransform.localPosition;  
            }
        }

        public void TriggerShake() 
            => StartCoroutine(Shake());

        private IEnumerator Shake()
        {
            float elapsedTime = 0f;

            while (elapsedTime < this.shakeDuration)
            {
                float x = Random.Range(-shakeMagnitude, shakeMagnitude);
                float y = Random.Range(-shakeMagnitude, shakeMagnitude);

                this._rectTransform.localPosition = _originalPosition + new Vector3(x, y, 0);

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            this._rectTransform.localPosition = _originalPosition;
        }
    }
}
