using UnityEngine;

namespace FlappyBirdScripts.FlyObjectManager
{
    public class BirdSmokeTrail : MonoBehaviour
    {
        [Header("Smoke Settings")]
        public ParticleSystem smokePrefab;
        public Vector3 offset = new Vector3(-2f, -2f, -1f);
        public float smokeScale = 0.3f;

        [Header("Materials")]
        public Material particleMaterial;
        public Material trailMaterial;

        private ParticleSystem _smokeInstance;

        void Start()
        {
            if (smokePrefab == null)
            {
                Debug.LogWarning("Smoke prefab is not assigned.");
                return;
            }

            // Instantiate smoke and parent it to the bird
            _smokeInstance = Instantiate(smokePrefab, transform);
            _smokeInstance.transform.localPosition = offset;
            _smokeInstance.transform.localRotation = Quaternion.identity;
            _smokeInstance.transform.localScale = Vector3.one * smokeScale;

            // Assign materials to the particle renderer
            var renderer = _smokeInstance.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                if (particleMaterial != null)
                    renderer.material = particleMaterial;
                if (trailMaterial != null)
                    renderer.trailMaterial = trailMaterial;
            }

            // Basic particle settings
            var main = _smokeInstance.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.startLifetime = 0.4f;
            main.startSpeed = 0.5f;
            main.startSize = 0.15f;
            main.startColor = new Color(1f, 1f, 1f, 0.6f);
            main.maxParticles = 100;

            // Trail setup
            var trails = _smokeInstance.trails;
            trails.enabled = true;
            trails.mode = ParticleSystemTrailMode.PerParticle;
            trails.ratio = 1f;
            trails.lifetime = 0.5f;
            trails.textureMode = ParticleSystemTrailTextureMode.Stretch;

            // Size over lifetime for nice fade-out
            var sizeOverLifetime = _smokeInstance.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(1f, 0f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Color fade over lifetime
            var colorOverLifetime = _smokeInstance.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0.0f),
                    new GradientColorKey(Color.gray, 1.0f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.7f, 0.0f),
                    new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            _smokeInstance.Play();
        }

        void LateUpdate()
        {
            // Prevent smoke trail from rotating with the bird
            if (_smokeInstance != null)
                _smokeInstance.transform.rotation = Quaternion.identity;
        }
    }
}
