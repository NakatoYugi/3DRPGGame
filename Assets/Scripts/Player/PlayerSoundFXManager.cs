using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }
}