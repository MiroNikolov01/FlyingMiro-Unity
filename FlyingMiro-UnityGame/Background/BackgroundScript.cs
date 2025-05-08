using UnityEngine;

namespace FlappyBirdScripts.Background
{
    public class BackgroundScript : MonoBehaviour
    {
        public float scrollSpeed = 5f;
        public float length = 144f;
        
        private Vector3 _startPosition;
        private float _width;

        void Start()
        {
            this._startPosition = this.transform.position;
        }

        void Update()
        {
            float newPosition = Mathf.Repeat(Time.unscaledTime * this.scrollSpeed, length);
            this.transform.position = this._startPosition + Vector3.left * newPosition;

        }
    }
}
