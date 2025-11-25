using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    private Rigidbody _rb;
    private Bot _bot;
    private Vector3 _lastPosition;
    private float _movementThreshold = 0.5f; // Distancia mínima para contar como movimiento

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _bot = GetComponent<Bot>();
        _lastPosition = transform.position;
    }

    public void MoveTo(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        hasTarget = true;
    }

    public bool HasArrived()
    {
        return !hasTarget;
    }

    private float currentX = 0, currentY = 0;
    public void Rotate(float visionX, float visionY)
    {
        currentX += visionX * rotationSpeed;
        currentY += visionY * rotationSpeed;

        // Limitar el �ngulo vertical para evitar que el personaje se voltee
        currentY = Mathf.Clamp(currentY, -60f, 70f);

        this.transform.rotation = Quaternion.Euler(-currentY, currentX, 0);
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 newPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                movementSpeed * Time.fixedDeltaTime
            );

            // Usar MovePosition del Rigidbody para mejor manejo de físicas
            if (_rb != null)
            {
                _rb.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }
            
            // Registrar movimiento si supera el umbral
            if (_bot != null && _bot.metrics != null)
            {
                float distanceMoved = Vector3.Distance(_lastPosition, transform.position);
                if (distanceMoved >= _movementThreshold)
                {
                    _bot.metrics.RecordMovement(_bot.id);
                    _lastPosition = transform.position;
                }
            }

            // Rotar hacia la dirección de movimiento
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                hasTarget = false;
        }
    }
}
