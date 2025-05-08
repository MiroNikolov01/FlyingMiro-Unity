using System.Collections;
using FlappyBirdScripts.GameManagement;
using FlappyBirdScripts.ScoreManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlappyBirdScripts.PipeManager
{
    public class PipeSpawner : MonoBehaviour
    {
        [Header("PipePrefabs")] public GameObject pipePairPrefab;
        public GameObject finalPipePrefab;

        [Header("Colorful Pipe Prefabs")] public GameObject bluePipePrefab;
        public GameObject orangePipePrefab;
        public GameObject redPipePrefab;
        public GameObject pinkPipePrefab;

        [Header("LevelSingPipePrefabs")] public GameObject level1PipePrefab;
        public GameObject level2PipePrefab;
        public GameObject level3PipePrefab;
        public GameObject level4PipePrefab;
        public GameObject level5PipePrefab;
        public GameObject level6PipePrefab;
        public GameObject level7PipePrefab;
        public GameObject level8PipePrefab;
        public GameObject level9PipePrefab;
        public GameObject level10PipePrefab;


        public float moveSpeed = 5f;
        [FormerlySerializedAs("maxTime")] public float pipeSpacing = 6f;
        public float heightRange = 3f;
        private readonly float _spawnX = 6f;
        private float _spawnTimer = 0f;
        private int _pipeSpawnCount = 0;
        private int _movingPipesInCycle = 0;

        void Start() => SpawnPipePair();

        void Update()
        {
            if (GameManager.instance.isGameOver) return;
            
            if (GameManager.instance || GameManager.hasStarted)
                this._spawnTimer += Time.deltaTime;


            if (this._spawnTimer > this.pipeSpacing)
            {
                SpawnPipePair();
                this._spawnTimer = 0f;
            }
        }

        public void SpawnPipePair()
        {
            float randomY = Random.Range(-heightRange, heightRange);

            Vector3 spawnPosition = new Vector3(_spawnX, randomY, 0f);

            GameObject pipePair;
            _pipeSpawnCount++;

            int score = ScoreManager.instance.GetCurrentScore();
            pipePair = InstantiatePipePair(spawnPosition, _pipeSpawnCount);

            if (_pipeSpawnCount == 200 || GameManager.instance.isGameOver) return;

            if (_pipeSpawnCount % 5 == 0)
            {
                _movingPipesInCycle = 0;
            }

            int allowedMovingPipes = GetAllowedMovingPipes(score);

            if (_movingPipesInCycle < allowedMovingPipes)
            {
                EnablePipeMovement(pipePair);
            }

            Destroy(pipePair, 120f);

            PipeMovement pipeMovement = pipePair.GetComponent<PipeMovement>();
            if (pipeMovement != null)
            {
                pipeMovement.SetMoveSpeed(moveSpeed);
            }
        }

        private GameObject InstantiatePipePair(Vector3 spawnPosition, int pipeSpawnCount)
        {
            GameObject prefabToUse = pipePairPrefab;

            switch (pipeSpawnCount)
            {
                case 1:
                    prefabToUse = level1PipePrefab;
                    break;
                case 20:
                    prefabToUse = level2PipePrefab;
                    break;
                case 40:
                    prefabToUse = level3PipePrefab;
                    break;
                case 60:
                    prefabToUse = level4PipePrefab;
                    break;
                case 80:
                    prefabToUse = level5PipePrefab;
                    break;
                case 100:
                    prefabToUse = level6PipePrefab;
                    break;
                case 120:
                    prefabToUse = level7PipePrefab;
                    break;
                case 140:
                    prefabToUse = level8PipePrefab;
                    break;
                case 160:
                    prefabToUse = level9PipePrefab;
                    break;
                case 180:
                    prefabToUse = level10PipePrefab;
                    break;
                case 200:
                    prefabToUse = finalPipePrefab;
                    break;
                default:
                    if (pipeSpawnCount > 180)
                        prefabToUse = redPipePrefab;
                    else if (pipeSpawnCount > 160)
                        prefabToUse = redPipePrefab;
                    else if (pipeSpawnCount > 120)
                        prefabToUse = bluePipePrefab;
                    else if (pipeSpawnCount > 80)
                        prefabToUse = pinkPipePrefab;
                    else if (pipeSpawnCount > 40)
                        prefabToUse = orangePipePrefab;
                    break;
            }

            GameObject pipePair = StartLevelPipeAtSafeHeight(spawnPosition, pipeSpawnCount, prefabToUse);

            if (pipeSpawnCount == 200)
            {
                PipeMover mover = pipePair.GetComponent<PipeMover>();
                if (mover != null)
                {
                    mover.DisableMovement();
                }
            }

            return pipePair;
        }

        private GameObject StartLevelPipeAtSafeHeight(Vector3 spawnPosition, int pipeSpawnCount, GameObject prefabToUse)
        {
            GameObject pipePair = Instantiate(prefabToUse, spawnPosition, Quaternion.identity);
            if (pipeSpawnCount % 20 == 0 && pipeSpawnCount <= 200)
            {
                float finalY = 2.5f;
                pipePair.transform.position = new Vector3(_spawnX, finalY, 0f);

                PipeMover mover = pipePair.GetComponent<PipeMover>();
                if (mover != null)
                {
                    mover.DisableMovement();

                    StartCoroutine(EnablePipeAfterDelay(mover, 2f));
                }
            }

            return pipePair;
        }

        private IEnumerator EnablePipeAfterDelay(PipeMover mover, float delay)
        {
            yield return new WaitForSeconds(delay);

            mover.DisableMovement();
        }

        int GetAllowedMovingPipes(int score) //Part of level difficulty
        {
            return score switch
            {
                >= 180 => 5,
                >= 140 => 4,
                >= 120 => 3,
                >= 60 => 2,
                >= 40 => 1,
                _ => 0
            };
        }

        private void EnablePipeMovement(GameObject pipePair)
        {
            PipeMover mover = pipePair.GetComponent<PipeMover>();
            if (mover != null)
            {
                mover.EnableMovement();
                mover.moveSpeed = GameManager.instance.currentPipeMoveSpeed;
                mover.moveRange = GameManager.instance.currentPipeMoveRange;
                this._movingPipesInCycle++;
            }
        }
    }
}
