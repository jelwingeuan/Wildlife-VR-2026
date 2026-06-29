using UnityEngine;

public class RescueCarrier : MonoBehaviour
{
    private Transform tapir;
    private Transform nurseryPoint;
    private MissionManager manager;

    public float moveSpeed = 2f;
    public float pickupHeight = 2f;

    private bool pickedUp = false;
    private bool finished = false;

    public void Setup(Transform tapirTarget, Transform nurseryTarget, MissionManager missionManager)
    {
        tapir = tapirTarget;
        nurseryPoint = nurseryTarget;
        manager = missionManager;
    }

    void Update()
    {
        if (tapir == null || nurseryPoint == null || manager == null || finished)
            return;

        if (!pickedUp)
        {
            Vector3 pickupPosition = tapir.position + Vector3.up * pickupHeight;

            transform.position = Vector3.MoveTowards(
                transform.position,
                pickupPosition,
                moveSpeed * Time.deltaTime
            );

            transform.LookAt(pickupPosition);

            if (Vector3.Distance(transform.position, pickupPosition) < 0.3f)
            {
                pickedUp = true;

                tapir.SetParent(transform);
                tapir.localPosition = Vector3.down * pickupHeight;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                nurseryPoint.position,
                moveSpeed * Time.deltaTime
            );

            transform.LookAt(nurseryPoint.position);

            if (Vector3.Distance(transform.position, nurseryPoint.position) < 0.3f)
            {
                finished = true;

                tapir.SetParent(null);
                tapir.position = nurseryPoint.position;

                manager.RescueFinished();
            }
        }
    }
}