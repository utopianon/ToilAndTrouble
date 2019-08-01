using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public List<GrabbableObject> attachedItems;

    private void Awake()
    {
        attachedItems = new List<GrabbableObject>();
    }

    public AttachPoint Attach(GrabbableObject ingredient)
    {
        if (attachedItems.Count == 0)
        {
            return this;
        }
        else
        {
            if (ingredient.type == attachedItems[0].type)
            {
                return this;
            }
        }
        return null;
    }

    public Vector2 FreeAttachPoint(GrabbableObject ingredient)
    {
        if (attachedItems.Count == 0)
        {
            attachedItems.Add(ingredient);
            return transform.position;
        }
        else
        {
            Vector2 newPoint = transform.position;
            newPoint.y += (attachedItems[attachedItems.Count - 1].collider.bounds.size.y * attachedItems.Count);
            attachedItems.Add(ingredient);
            return newPoint;
        }

    }

    public void Detach(GrabbableObject ingredient)
    {
        attachedItems.Remove(ingredient);
    }
}
