using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRailV2 : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [Header("Target Offset")]
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Vector3 positionOffset;
    [Header("Rail Visualization")]
    [SerializeField] private bool visualizeRail;
    [SerializeField] [Min(0)] private float railSize = .5f;

    [HideInInspector] public Transform CharacterTransform;
    [HideInInspector] public Transform IkTargetTransform;

    private Vector3 _lastClosestPoint;


    public IEnumerator FixCharacterHandIKTargetToRail(Transform character_Transform, Transform handIkTarget_Transform) 
    {
        while (true) 
        {
            int firstSectionWaypointIndex = GetFirstSectionWaypointIndex(character_Transform.position);
            Vector3 closestPointInSection = GetClosestReferencePointBetween2Points(waypoints[firstSectionWaypointIndex].position, waypoints[firstSectionWaypointIndex + 1].position, character_Transform.position);
            if (Vector3.Distance(character_Transform.position, closestPointInSection) < Vector3.Distance(character_Transform.position, _lastClosestPoint)) 
            {
                _lastClosestPoint = closestPointInSection;
                handIkTarget_Transform.position = closestPointInSection + positionOffset;
                handIkTarget_Transform.rotation = Quaternion.LookRotation(Vector3.Cross(waypoints[firstSectionWaypointIndex + 1].position - waypoints[firstSectionWaypointIndex].position, waypoints[firstSectionWaypointIndex].transform.up));
                handIkTarget_Transform.rotation *= Quaternion.Euler(rotationOffset);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    #region Methods

    

    int GetFirstSectionWaypointIndex(Vector3 referencePoint) 
    {
        float closestDist = float.PositiveInfinity;
        int closestTrailSectionIndex = 0;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 pointInTrail = GetClosestReferencePointBetween2Points(waypoints[i].position, waypoints[i + 1].position, referencePoint);
            float distanceFromReference = Vector3.Distance(referencePoint, pointInTrail);

            if (distanceFromReference < closestDist)
            {
                closestDist = distanceFromReference;
                closestTrailSectionIndex = i;
            }
        }

        return closestTrailSectionIndex;
    }

    Vector3 GetClosestReferencePointBetween2Points(Vector3 pointA, Vector3 pointB, Vector3 referencePoint)
    {
        //https://stackoverflow.com/questions/42040183/how-to-find-a-point-x-between-two-vector3-in-unity3d
        Vector3 AB = pointB - pointA;
        Vector3 AC = referencePoint - pointA;
        Vector3 AX = Vector3.Project(AC, AB);
        return AX + pointA;
    }

    #endregion

    
    private void OnDrawGizmos()
    {
        if (visualizeRail) 
        {
            #region Draw Rail

            Gizmos.color = Color.red;

            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, railSize);
                if (i < waypoints.Length - 1)
                {
                    Gizmos.DrawLine(waypoints[i].position - Vector3.left * railSize, waypoints[i + 1].position - Vector3.left * railSize);
                    Gizmos.DrawLine(waypoints[i].position + Vector3.left * railSize, waypoints[i + 1].position + Vector3.left * railSize);
                }
            }

            #endregion
        }
    }
}
