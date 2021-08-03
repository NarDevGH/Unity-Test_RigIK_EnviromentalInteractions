using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRail : MonoBehaviour
{
    public GameObject player;
    public GameObject ikTarget;
    public bool interactingWithPlayer = false;

    [SerializeField] [Min(0)] private float railSize = .5f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform handReference;

    private bool pointFound = false;
    private Vector3 lastClosestPoint = Vector3.zero;

    private void Update()
    {
        if (interactingWithPlayer) 
        {
            ikTarget.transform.position = GetClosesReferencePointOnTrail(player.transform.position);
        }

    }


    Vector3 GetClosesReferencePointOnTrail(Vector3 referencePoint) 
    {
        Vector3 closestPoint = Vector3.positiveInfinity;
        float closestDis = float.PositiveInfinity;

        Vector3 pointInTrail;
        float distanceFromReference;
        pointFound = false;
        for (int i = 0; i < waypoints.Length-1; i++)
        {
            pointInTrail = GetClosestReferencePointBetween2Points(waypoints[i].position, waypoints[i + 1].position, referencePoint);
            if (validPoint(waypoints[i].position, waypoints[i + 1].position, pointInTrail)) 
            {
                pointFound = true;

                distanceFromReference = Vector3.Distance(referencePoint, pointInTrail);
                if (distanceFromReference < closestDis) 
                {
                    closestDis = distanceFromReference;
                    closestPoint = pointInTrail;
                }
            }
        }

        if (closestDis > Vector3.Distance(referencePoint, lastClosestPoint)) 
        {
            pointFound = false;
        } 

        return closestPoint;
    }


    bool validPoint(Vector3 pointA, Vector3 pointB, Vector3 pointToValidate) 
    {
        if ((pointToValidate.x > pointA.x && pointToValidate.x > pointB.x) || (pointToValidate.x < pointA.x && pointToValidate.x < pointB.x) ||
            (pointToValidate.y > pointA.y && pointToValidate.y > pointB.y) || (pointToValidate.y < pointA.y && pointToValidate.y < pointB.y) ||
            (pointToValidate.z > pointA.z && pointToValidate.z > pointB.z) || (pointToValidate.z < pointA.z && pointToValidate.z < pointB.z))
        {
            return false;
        }
        return true;
    }

    Vector3 GetClosestReferencePointBetween2Points(Vector3 pointA, Vector3 pointB, Vector3 referencePoint) 
    {
        //https://stackoverflow.com/questions/42040183/how-to-find-a-point-x-between-two-vector3-in-unity3d
        Vector3 AB = pointB - pointA;
        Vector3 AC = referencePoint - pointA;
        Vector3 AX = Vector3.Project(AC, AB);
        return AX + pointA;
    }
    
    private void OnDrawGizmos()
    {
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

        if (handReference != null) 
        {
            Vector3 pointInTrail = GetClosesReferencePointOnTrail(handReference.position);

            if (pointFound) 
            {
                lastClosestPoint = pointInTrail;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pointInTrail, railSize * 2);
            }
        }
    }
}
