using FlappyBirdScripts.ScoreManagement;
using UnityEngine;

namespace FlappyBirdScripts.PipeManager
{
    public class PipeScoreTrigger : MonoBehaviour
    {
        private bool _hasScored = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_hasScored) return;

            if (collision.CompareTag("Pipe") && 
                collision.gameObject.name == "PipeUp")
            {
                ScoreManager.instance.UpdateScore();
                this._hasScored = true;
            }
        }
    }
}