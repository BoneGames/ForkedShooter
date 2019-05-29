using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHitBox : InvulTotem
{
  public override void Start()
  {
    currentHealth = maxHealth;
  }

  void OnDestroy()
  {
    Destroy(transform.parent.gameObject, 1);
  }
}
