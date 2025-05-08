using UnityEngine;

namespace FlappyBirdScripts.MovingScripts
{
    public class FloatUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector2 _startPos;
        private Vector3 _startWorldPos;

        public float floatSpeed = 1f;       
        public float floatAmount = 0.5f;     

        void Start()
        {
            this._rectTransform = GetComponent<RectTransform>();

            if (this._rectTransform != null)
            {
                this._startPos = _rectTransform.anchoredPosition;
            }
            else
            {
                this._startWorldPos = transform.position;
            }
        }

        void Update()
        {
            float newY = Mathf.Sin(Time.unscaledTime * this.floatSpeed) * this.floatAmount;

            if (this._rectTransform != null)
            {
                this._rectTransform.anchoredPosition = this._startPos + new Vector2(0, newY);
            }
            else
            {
                this.transform.position = this._startWorldPos + new Vector3(0, newY, 0);
            }
        }
    }
}
