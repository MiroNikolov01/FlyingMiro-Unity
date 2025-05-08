using UnityEngine;


namespace FlappyBirdScripts.AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        
        [Header("Source")]
        public float bgmVolume = 3f;
        public float sfxVolume = 7f;

        public AudioSource bgmSource;
        public AudioSource sfxSource;

        [Header("Background and Jump")]
        public AudioClip backgroundMusic;
        public AudioClip jumpSound;

        
        [Header("Dead sounds")]
        public AudioClip deathSoundOutOfBoundsUp;
        public AudioClip deathSoundOutOfBoundsUp1;
        public AudioClip deathSoundOutOfBoundsUp2;
        public AudioClip deathSoundOutOfBoundsUp3;

        public AudioClip deathSoundOutOfBoundsDown;
        public AudioClip deathSoundOutOfBoundsDown1;
        public AudioClip deathSoundOutOfBoundsDown2;
        public AudioClip deathSoundOutOfBoundsDown3;
        public AudioClip deathSoundOutOfBoundsDown4;

        public AudioClip deathSound1;
        public AudioClip deathSound2;
        public AudioClip crashSound;

        [Header("Win game sounds")]
        public AudioClip winGameSound;

        [Header("Score sound")]
        public AudioClip scoreSound;

        [Header("Congrats sound")]
        public AudioClip congratsSound;
        public AudioClip thanksFinalMessage;

        [Header("Volume button")]
        private bool _isMusicMuted = false;
        public UnityEngine.UI.Image muteBtnImage;

        public Sprite musicOnSprite;
        public Sprite musicOffSprite;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

            PlayBackgroundMusic();
            UpdateVolumes();
            UpdateMuteButtonSprite();
        }

        public void UpdateVolumes()
        {
            bgmSource.volume = _isMusicMuted ? 0f : bgmVolume;
            sfxSource.volume = sfxVolume;
        }

        public void PlayBackgroundMusic()
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
            UpdateVolumes();
        }

        public void PlayJumpSound()
        {
            sfxSource.PlayOneShot(jumpSound);
            sfxSource.volume = 20f;
        }

        public void PlayWinFinalLevelSound()
        {
            sfxSource.PlayOneShot(winGameSound);
            sfxSource.volume = 100f;
        }

        public void PlayLevelUpSound()
        {
            sfxSource.PlayOneShot(congratsSound);
            sfxSource.volume = sfxVolume;
        }

        public void PlayDeathSound()
        {
            sfxSource.PlayOneShot(GetRandomDeadAudioClipHitByPipe());
        }

        public void PlayPipeCrashSound()
        {
            sfxSource.PlayOneShot(crashSound);
            sfxSource.volume = 100f;
            sfxSource.ignoreListenerPause = true;
        }

        public void PlayDeathSoundUp()
        {
            sfxSource.PlayOneShot(GetRandomDeadAudioClipOutOfBoundsUp());
        }

        public void PlayDeathSoundDown()
        {
            sfxSource.PlayOneShot(GetRandomDeadAudioClipOutOfBoundsDown());
        }

        public void PlayScoreSound()
        {
            sfxSource.PlayOneShot(scoreSound);
        }

        public AudioClip PlayThanksFinalMessage()
        {
            if (thanksFinalMessage != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(thanksFinalMessage);
                return thanksFinalMessage;
            }
            return null;
        }

        public void ToggleMusic() // <- button object usage
        {
            _isMusicMuted = !_isMusicMuted;
            bgmSource.volume = _isMusicMuted ? 0f : bgmVolume;
            PlayerPrefs.SetInt("MusicMuted", _isMusicMuted ? 1 : 0);
            PlayerPrefs.Save();

            UpdateMuteButtonSprite();
        }

        public void UpdateMuteButtonSprite()
        {
            if (muteBtnImage != null)
            {
                muteBtnImage.sprite = _isMusicMuted ? musicOffSprite : musicOnSprite;
            }
        }

        public AudioClip GetRandomDeadAudioClipOutOfBoundsUp()
        {
            AudioClip[] audioClips = new AudioClip[]
            {
                deathSoundOutOfBoundsUp,
                deathSoundOutOfBoundsUp1,
                deathSoundOutOfBoundsUp2,
                deathSoundOutOfBoundsUp3,
            };
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public AudioClip GetRandomDeadAudioClipOutOfBoundsDown()
        {
            AudioClip[] audioClips = new AudioClip[]
            {
                deathSoundOutOfBoundsDown,
                deathSoundOutOfBoundsDown1,
                deathSoundOutOfBoundsDown2,
                deathSoundOutOfBoundsDown3,
                deathSoundOutOfBoundsDown4,
            };
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public AudioClip GetRandomDeadAudioClipHitByPipe()
        {
            AudioClip[] deadAudioClips = new AudioClip[]
            {
                deathSound1,
                deathSound2,
            };
            return deadAudioClips[Random.Range(0, deadAudioClips.Length)];
        }
    }
}