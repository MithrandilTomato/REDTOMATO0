using System.Reflection;
using UnityEngine;

/// <summary>
/// Arac�n �n�ne veya tekerle�ine ekleyece�in, IsTrigger a��k bir Collider2D
/// �zerine ataca��n script. �arp��t��� d��man� yok ederken �nce d��man�n kendi
/// Die() metodunu, sonra ek sesi �alar ve opsiyonel z�platma uygular.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CarEnemyDestroyer : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("T�m d��manlar ayn� tag'e sahipse buraya yaz; �zel controller'lar i�in interface/bile�en alg�lamas� kullan�l�r.")]
    public string enemyTag = "Enemy";

    [Header("Extra Hit Sound")]
    [Tooltip("Arac�n �arpma sesi i�in AudioSource")]
    public AudioSource audioSource;
    [Tooltip("Arac�n �arpma ses klibi")]
    public AudioClip hitSound;

    [Header("Bounce")]
    [Tooltip("�arp�lan d��man� z�platmak istersen de�eri ayarla, 0 = z�plama yok")]
    public float bounceForce = 5f;

    void Reset()
    {
        // Inspector'da Is Trigger a��k gelsin
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) �zel worm d��man�
        var worm = other.GetComponent<WormEnemyController>();
        if (worm != null)
        {
            HandleDeath(worm);
            return;
        }

        // 2) Genel tag kontrol�
        if (other.CompareTag(enemyTag))
        {
            // reflection ile varsa Die() metodunu �a��r
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
        // D��man�n kendi Die() rutinini �al��t�r
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
