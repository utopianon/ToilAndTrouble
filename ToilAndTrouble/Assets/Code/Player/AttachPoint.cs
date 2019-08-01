﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public List<GrabbableObject> attachedItems;
    public bool unloading = false;
    public Player Owner { get; set; }

    private void Awake()
    {
        attachedItems = new List<GrabbableObject>();
    }

    public AttachPoint Attach(GrabbableObject ingredient)
    {
        if (Owner.heldItems < Owner.maxHeldItems)
        {
            if (Owner.heldItems <1)
            {
                return this;
            }
            else
            {
                if (ingredient.type == Owner.activeAttachPoint.attachedItems[0].type)
                {
                    return Owner.activeAttachPoint;
                }                
            }
        }

        return null;
    }

    public Vector2 FreeAttachPoint(GrabbableObject ingredient)
    {
        if (attachedItems.Count == 0)
        {
            attachedItems.Add(ingredient);
            Owner.heldItems++;
            Owner.activeAttachPoint = this;
            return transform.position;
        }
        else
        {
            Vector2 newPoint = transform.position;
            newPoint.y += (attachedItems[attachedItems.Count - 1].collider.bounds.size.y * attachedItems.Count);
            attachedItems.Add(ingredient);
            Owner.heldItems++;
            return newPoint;
        }

    }

    public void Detach(GrabbableObject ingredient)
    {
        attachedItems.Remove(ingredient);
        Owner.heldItems--;

        if (attachedItems.Count >0 && !unloading)
        {
            unloading = true;
            StartCoroutine(Unload());
        }

    }

    public IEnumerator Unload()
    {
        GrabbableObject[] temp = attachedItems.ToArray();
        foreach (GrabbableObject i in temp)
        {
            i.Drop();
        }
        unloading = false;
        yield return null;
    }
}
