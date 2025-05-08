using UnityEngine;
using UnityEngine.UI;

namespace FlappyBirdScripts.MovingScripts
{
    public class ClickToStartEffect : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 1.5f;
        [SerializeField] private float minAlpha = 0.3f;
        [SerializeField] private float maxAlpha = 1.0f;

        private Image _uiImage;
        private float _timer;

        private void Start()
        {
            _uiImage = GetComponent<Image>();
            if (_uiImage == null)
            {
                Debug.LogError("No Image component found on this GameObject!");
                return;
            }
        }

        private void Update()
        {
            if (_uiImage == null) return;

            _timer += Time.unscaledDeltaTime * fadeSpeed;

            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(_timer) + 1f) * 0.5f);

            Color color = _uiImage.color;
            color.a = alpha;
            _uiImage.color = color;
        }
    }
}