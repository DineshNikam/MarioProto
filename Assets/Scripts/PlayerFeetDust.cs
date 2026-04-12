using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerFeetDust : MonoBehaviour
{
    [SerializeField] ParticleSystem dustParticles;

    PlayerController _player;

    void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        if (_player != null)
        {
            _player.Jumped += OnJump;
            _player.Landed += OnLand;
        }
    }

    void OnDisable()
    {
        if (_player != null)
        {
            _player.Jumped -= OnJump;
            _player.Landed -= OnLand;
        }
    }

    void OnJump()
    {
        if (dustParticles != null)
            dustParticles.Play();
    }

    void OnLand()
    {
        if (dustParticles != null)
            dustParticles.Play();
    }
}
