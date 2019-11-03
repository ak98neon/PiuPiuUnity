using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherPlayerShooting : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip firePistol;
    public AudioClip reloadAudio;

    public void shoot(Vector3 target)
    {
        audio.PlayOneShot(firePistol);
        StartCoroutine(ShootingUtil.SphereIndicator(target));
    }
}
