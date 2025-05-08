using FlappyBirdScripts.GameManagement;
using UnityEngine;

namespace FlappyBirdScripts.PipeManager
{
    public class PipeMovement : MonoBehaviour
    {
        public float moveSpeed = 2f;

        void Update()
        {
            if (GameManager.instance.isGameOver)  return;
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }

        public void SetMoveSpeed(float speed) 
            => moveSpeed = speed;
    }
}