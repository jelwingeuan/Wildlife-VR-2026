using UnityEngine;

public class SearchDrone : MonoBehaviour
{
    private Transform target;
    private MissionManager manager;

    public float moveSpeed = 2f;
    public float stopDistance = 0.5f;

    private bool hasFoundAnimal = false;

    public void Setup(Transform animalTarget, MissionManager missionManager)
    {
        target = animalTarget;
        manager = missionManager;
    }

    void Update()
    {
        if (target == null || manager == null || hasFoundAnimal)
            return;

        Vector3 targetPosition = target.position + Vector3.up * 1.5f;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        transform.LookAt(targetPosition);

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= stopDistance)
        {
            hasFoundAnimal = true;
            manager.AnimalFound();
        }
    }
}