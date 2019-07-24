using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    AttachPoint pointAttachedTo;

    Collider2D collider;
    public bool grabbed = false;
    public float jumpHeight = 1;
    public float distanceToGrab = 2f;
    public LayerMask playerMask;
    public LayerMask grabbablesMask;
    public LayerMask collisionsMask;

    private void Start()
    {
        collider = gameObject.GetComponent<Collider2D>();
    }

    private void LateUpdate()
    {
        if (grabbed && pointAttachedTo)
        {
            transform.position = pointAttachedTo.transform.position;
        }
    }
    public void Grabbed()
    {
        if (!grabbed)
        {
            RaycastHit2D hit;
            if (hit = Physics2D.CircleCast(transform.position, distanceToGrab, Vector2.left, 0, playerMask))
            {
                grabbed = true;
                gameObject.layer += 1;
                StartCoroutine(AttachToPlayer(nearestAttachPoint(hit.transform.GetComponent<Player>())));
            }
            else
            {
                grabbed = true;
                StartCoroutine(FailGrab());
            }
        }

        else
        {
            if (pointAttachedTo)
            {
                Drop();
            }
        }
    }

    public AttachPoint nearestAttachPoint (Player player)
    {
        AttachPoint[] attachPoints = player.attachPoints;
        AttachPoint nearestAP = attachPoints[0];
        float distToNearestAP = Vector2.Distance(transform.position, nearestAP.transform.position);

        for (int i = 1; i < attachPoints.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, attachPoints[i].transform.position);
            if (dist < distToNearestAP)
            {
                nearestAP = attachPoints[i];
                distToNearestAP = dist;
            }
        }
        return nearestAP;
    }

    public void Drop()
    {
        grabbed = false;
        pointAttachedTo = null;        

        RaycastHit2D hit;
        if (hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down*collider.bounds.size.magnitude, Vector2.down))
        {         
            StartCoroutine(FallToGround(hit));
        }

    }

    public IEnumerator FallToGround(RaycastHit2D groundHit)
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos = groundHit.point;
        endPos.y += (groundHit.collider.bounds.size.y/2) + (collider.bounds.size.y/2);
        Vector2 startPos = transform.position;


        while ((Vector2)transform.position != endPos)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        gameObject.layer -= 1;
        yield return null;
    }

    public IEnumerator AttachToPlayer(AttachPoint attachPoint)
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos = attachPoint.transform.position;
        Vector2 startPos = transform.position;
        

        while (timer <= lerpTime)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        pointAttachedTo = attachPoint;        
        yield return null;
    }

    public IEnumerator FailGrab()
    {
        float lerpTime = 0.15f;
        float timer = 0;
        float perc;
        Vector2 endPos;
        Vector2 startPos = endPos = transform.position;
        endPos.y += jumpHeight;

        while (timer <= ((lerpTime / 3) * 2))
        {
            timer += Time.deltaTime;
            perc = timer / ((lerpTime / 3) * 2);
            transform.position = Vector2.Lerp(startPos, endPos, perc);
            yield return null;
        }
        endPos = transform.position;
        lerpTime -= timer;
        timer = 0;
        while (timer <= lerpTime)
        {
            timer += Time.deltaTime;
            perc = timer / lerpTime;
            transform.position = Vector2.Lerp(endPos, startPos, perc);
            yield return null;

        }
        grabbed = false;
        yield return null;
    }
}
