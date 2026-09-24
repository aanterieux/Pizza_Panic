using UnityEngine;

public class PlayerAudioController : PlayerComponent
{
    [SerializeField] private AudioClip m_footstepSound = null;
    [SerializeField] private AudioClip m_jumpSound = null;
    [SerializeField] private AudioClip m_landingSound = null;
    [SerializeField] private AudioClip m_itemPickupSound = null;
    [SerializeField] private AudioClip m_itemThrowSound = null;
    [SerializeField] private AudioClip m_gunEquipSound = null;
    [SerializeField] private AudioClip m_gunUnequipSound = null;
    [SerializeField] private AudioClip m_meleeAttackHitSound = null;
    [SerializeField] private AudioClip m_hurtSound = null;
    [SerializeField] private AudioClip m_deathSound = null;

    private void PlaySound(AudioClip _sound)
    {
        AudioManager.s_Instance.Play3D(
            _sound,
            transform.position,
            new AudioManager.AudioParams(
                1f,
                Random.Range(0.85f, 1.15f)
            )
        );
    }

    public void PlayFootstepSound()
    {
        PlaySound(m_footstepSound);
    }
    public void PlayJumpSound()
    {
        PlaySound(m_jumpSound);
    }
    public void PlayLandingSound()
    {
        PlaySound(m_landingSound);
    }
    public void PlayItemPickupSound()
    {
        PlaySound(m_itemPickupSound);
    }
    public void PlayGunEquipSound()
    {
        PlaySound(m_gunEquipSound);
    }
    public void PlayGunUnequipSound()
    {
        PlaySound(m_gunUnequipSound);
    }
    public void PlayItemThrowSound()
    {
        PlaySound(m_itemThrowSound);
    }
    public void PlayMeleeAttackHitSound()
    {
        PlaySound(m_meleeAttackHitSound);
    }
    public void PlayDamagedSound()
    {
        PlaySound(m_hurtSound);
    }
    public void PlayDeathSound()
    {
        PlaySound(m_deathSound);
    }
}
