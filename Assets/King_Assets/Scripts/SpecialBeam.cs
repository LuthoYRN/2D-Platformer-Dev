using UnityEngine;

public class SpecialBeam : MonoBehaviour
{
    private Transform firePoint;
    private Vector3 originalScale;
    private float currentDirection = 1f;

    public void Initialize(Transform source)
    {
        firePoint = source;
        originalScale = transform.localScale;
        UpdatePositionAndDirection();
    }

    private void Update()
    {
        if (firePoint != null)
        {
            UpdatePositionAndDirection();
        }
    }
    private void DamagePlayer(){
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().TakeDamage(20);   
    }
 
    public void SetDirection(float _direction)
    {
        currentDirection = _direction;
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * _direction, originalScale.y, originalScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void UpdatePositionAndDirection()
    {
        if (firePoint == null) return;

        // 🔹 Ensure beam follows firePoint position
        transform.position = firePoint.position;

        // 🔹 Always get player direction dynamically
        float playerDirection = Mathf.Sign(firePoint.lossyScale.x);

        // 🔹 Update only if direction has changed
        if (Mathf.Sign(currentDirection) != playerDirection)
        {
            SetDirection(playerDirection);
        }
    }
}
