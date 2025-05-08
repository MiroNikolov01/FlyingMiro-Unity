using System.Collections.Generic;
using FlappyBirdScripts.AudioManagement;
using FlappyBirdScripts.GameManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace FlappyBirdScripts.FlyObjectManager
{
    public class FlyBehaviour : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject muteButtonObject;

        private Rigidbody2D _rb;
        private Camera _camera;
        private AudioManager _audioManager;

        [Header("FlyBehaviour Settings")] public float jumpForce = 20f;
        public float gravityScale = 0.3f;
        
        private bool _isGameOver;
        private const float RotationSpeed = 5f;
        private bool _isFrozen = true;


        void Start()
        {
            this._rb = this.GetComponent<Rigidbody2D>();
            this.gameManager = FindObjectOfType<GameManager>();
            this._audioManager = FindObjectOfType<AudioManager>();
            this._camera = Camera.main;

            UnfreezePlayer();
        }

        void Update()
        {
            if (this._isFrozen) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverMuteButton())
                {
                    return;
                }

                if (!GameManager.instance.isGameOver)
                {
                    this._rb.linearVelocity = Vector2.up * (jumpForce * 1.5f);
                }

                StartOrJump();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartOrJump();
            }

            OutOfBoundsCheck();
        }

        private bool IsPointerOverMuteButton()
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (result.gameObject != null &&
                    result.gameObject.CompareTag("VolumeButton") &&
                    result.gameObject == muteButtonObject)
                {
                    return true;
                }
            }

            return false;
        }

        void FixedUpdate()
        {
            if (this._isFrozen) return;
            if (GameManager.hasStarted && !gameManager.isGameOver)
            {
                float customFallSpeed = 2f;
                float maxFallSpeed = -3f;

                _rb.linearVelocity += Vector2.down * customFallSpeed * Time.unscaledDeltaTime;

                if (_rb.linearVelocity.y < maxFallSpeed)
                {
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxFallSpeed);
                }
            }
            else if (gameManager.isGameOver)
            {
                float deathFallSpeed = 0.5f;
                float maxDeathFallSpeed = -10f;

                _rb.linearVelocity += Vector2.down * deathFallSpeed * Time.unscaledDeltaTime;

                if (_rb.linearVelocity.y < maxDeathFallSpeed)
                {
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxDeathFallSpeed);
                }
            }

            float rotationZ = _rb.linearVelocity.y * RotationSpeed;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        }

        private void OutOfBoundsCheck()
        {
            float cameraVerticalExtent = this._camera.orthographicSize;
            if (transform.position.y < -_camera.transform.position.y - cameraVerticalExtent ||
                transform.position.y > _camera.transform.position.y + cameraVerticalExtent)
            {
                if (!this._isGameOver)
                {
                    this._audioManager.PlayPipeCrashSound();
                    if (transform.position.y < -_camera.transform.position.y - cameraVerticalExtent)
                    {
                        this._audioManager.PlayDeathSoundDown();
                    }
                    else if (transform.position.y > _camera.transform.position.y + cameraVerticalExtent)
                    {
                        this._audioManager.PlayDeathSoundUp();
                    }

                    this._isGameOver = true;
                    this.gameManager.GameOver();
                }
            }
        }

        private void StartOrJump()
        {
            if (!GameManager.hasStarted)
            {
                this.gameManager.StartGame();
                UnfreezePlayer();
                return;
            }

            if (!gameManager.isGameOver)
            {
                this._audioManager.PlayJumpSound();
                this._rb.linearVelocity = Vector2.up * jumpForce;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Pipe"))
            {
                if (!_isGameOver)
                {
                    this._audioManager.PlayPipeCrashSound();
                    this._audioManager.PlayDeathSound();

                    this._isGameOver = true;
                    this.gameManager.GameOver();
                }
            }
        }

        public void UnfreezePlayer()
        {
            this._isFrozen = false;
            if (this._rb.gravityScale == 0f)
            {
                this._rb.gravityScale = this.gravityScale;
            }
        }
    }
}