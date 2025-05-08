using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBirdScripts.PipeManager
{
    public class PipeMover : MonoBehaviour
    {
        public static PipeMover instance;
        public float moveSpeed = 1f;
        public float moveRange = 1f;

        private Vector3 _startPos;
        private bool _canMove = false;
        private float _moveOffset = 0f;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            this._startPos = transform.position;
            this._moveOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            if (this._canMove)
            {
                float y = Mathf.Sin(Time.unscaledTime * this.moveSpeed + this._moveOffset) * this.moveRange;
                transform.position = new Vector3(this._startPos.x, this._startPos.y + y, this._startPos.z);
            }
        }

        public void EnableMovement() => this._canMove = true;
        public void DisableMovement() => this._canMove = false;

        public void SetMoveSpeed(float speed) =>  this.moveSpeed = speed;

        public void SetMoveRange(float range) => this.moveRange = range;
    }
}