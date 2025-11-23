using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;

    private Vector3 targetPosition;
    private bool hasTarget = false;

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

        // Limitar el ángulo vertical para evitar que el personaje se voltee
        currentY = Mathf.Clamp(currentY, -60f, 70f);

        this.transform.rotation = Quaternion.Euler(-currentY, currentX, 0);
    }

    private void Update()
    {
        if (hasTarget)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                movementSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                hasTarget = false;
        }
    }
}
