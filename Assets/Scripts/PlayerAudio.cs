using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip jumpClip;

    PlayerController _player;

    void Awake()
    {
        _player = GetComponent<PlayerController>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (_player != null)
            _player.Jumped += PlayJump;
    }

    void OnDisable()
    {
        if (_player != null)
            _player.Jumped -= PlayJump;
    }

    void PlayJump()
    {
        if (jumpClip != null && sfxSource != null)
            sfxSource.PlayOneShot(jumpClip);
    }
}
