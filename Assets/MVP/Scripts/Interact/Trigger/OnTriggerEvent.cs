using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public class OnTriggerEvent : MonoBehaviour
{
  [Tag]
  [BoxGroup("Trigger Events")]
  public string hitTag;
  [BoxGroup("Trigger Events")]
  public UnityEvent onEnter, onStay, onExit;

  [HideInInspector]
  public Collider otherCollider;

  public virtual void Reset()
  {
    Collider col = GetComponent<Collider>();
    if (col)
    {
      col.isTrigger = true;
    }
    else
    {
      Debug.LogWarning(string.Format("The GameObject {0} does not have a collider. This will not work with trigger events.", name));
    }
  }

  public virtual void OnTriggerEnter(Collider other)
  {
    if (other.tag == hitTag || hitTag == "")
    {
      otherCollider = other;
      onEnter.Invoke();
    }
  }

  public virtual void OnTriggerStay(Collider other)
  {
    if (other.tag == hitTag || hitTag == "")
    {
      otherCollider = other;
      onStay.Invoke();
    }
  }

  public virtual void OnTriggerExit(Collider other)
  {
    if (other.tag == hitTag || hitTag == "")
    {
      otherCollider = other;
      onExit.Invoke();
    }
  }
}
