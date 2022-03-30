using UnityEngine;

public class KnockbackTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player != null)
        {
            player.Knockback(transform);
        }
    }
}
