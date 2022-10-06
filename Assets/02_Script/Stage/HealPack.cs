using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¸Þ¸ðÀå
// 1. Æ®¸®°Å name.ContainÀÌ player¸é Destroy


/// <summary>
/// ÀÛ¼ºÀÚ - ÀÌÁØ¼®
/// ÈúÆÑ ±¸Çö - ÇÃ·¹ÀÌ¾î°¡ ÈúÆÑÀ» ¸ÔÀ¸¸é Destroy
/// </summary>
public class HealPack : MonoBehaviour
{
    [SerializeField, Tooltip("Èú ¸ÔÀ¸¸é ³ªÅ¸³¯ ÀÌÆåÆ®")]
    private ParticleSystem healEffectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Player"))
        {
            var player = GameManager.player;
            var playerStatus = player.GetComponent<CharacterStatus>();
            playerStatus.ResetStatus();
            var playerMagic = player.GetComponent<PlayerMagic>();
            playerMagic.Reset();

            var healEffect = Instantiate(healEffectPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}