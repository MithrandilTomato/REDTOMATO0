using System.Reflection;
using UnityEngine;

/// <summary>
/// Aracýn önüne veya tekerleðine ekleyeceðin, IsTrigger açýk bir Collider2D
/// üzerine atacaðýn script. Çarpýþtýðý düþmaný yok ederken önce düþmanýn kendi
/// Die() metodunu, sonra ek sesi çalar ve opsiyonel zýplatma uygular.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CarEnemyDestroyer : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("Tüm düþmanlar ayný tag'e sahipse buraya yaz; özel controller'lar için interface/bileþen algýlamasý kullanýlýr.")]
    public string enemyTag = "Enemy";

    [Header("Extra Hit Sound")]
    [Tooltip("Aracýn çarpma sesi için AudioSource")]
    public AudioSource audioSource;
    [Tooltip("Aracýn çarpma ses klibi")]
    public AudioClip hitSound;

    [Header("Bounce")]
    [Tooltip("Çarpýlan düþmaný zýplatmak istersen deðeri ayarla, 0 = zýplama yok")]
    public float bounceForce = 5f;

    void Reset()
    {
        // Inspector'da Is Trigger açýk gelsin
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Özel worm düþmaný
        var worm = other.GetComponent<WormEnemyController>();
        if (worm != null)
        {
            HandleDeath(worm);
            return;
        }

        // 2) Genel tag kontrolü
        if (other.CompareTag(enemyTag))
        {
            // reflection ile varsa Die() metodunu çaðýr
            var mb = other.GetComponent<MonoBehaviour>();
            var dieMethod = mb?.GetType().GetMethod("Die", BindingFlags.Public | BindingFlags.Instance);
            if (dieMethod != null)
            {
                dieMethod.Invoke(mb, null);
            }
            else
            {
                // metot yoksa direkt destroy et
                Destroy(other.gameObject);
            }

            PlayHitSound();
            BounceOther(other);
        }
    }

    private void HandleDeath(WormEnemyController worm)
    {
        // Düþmanýn kendi Die() rutinini çalýþtýr
        worm.Die();

        PlayHitSound();
        BounceOther(worm.GetComponent<Collider2D>());
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

    private void BounceOther(Collider2D col)
    {
        if (bounceForce != 0f && col.attachedRigidbody != null)
        {
            var rb = col.attachedRigidbody;
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        }
    }
}
