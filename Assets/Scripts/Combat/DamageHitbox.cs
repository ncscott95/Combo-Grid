using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageHitbox : MonoBehaviour
{
    protected int _damage;
    protected LayerMask _targetLayers;
    private Collider2D _collider;

    public virtual void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void InitializeHitbox(int damage, LayerMask targetLayers)
    {
        _damage = damage;
        _targetLayers = targetLayers;
    }

    public void SetHitboxActive(bool active)
    {
        if (_collider == null) Debug.LogError("Hitbox Missing Collider");
        _collider.enabled = active;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{other.gameObject.name} hit, {other.gameObject.layer}, {Mathf.Log(_targetLayers.value, 2)}, {((1 << other.gameObject.layer) & _targetLayers) != 0}, {other.GetComponent<IDamageable>() != null}");
        if (((1 << other.gameObject.layer) & _targetLayers) != 0 && other.TryGetComponent<IDamageable>(out var hit))
        {
            hit.TakeDamage(_damage);
        }
    }
}
