﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Enterable
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            base.OnTriggerEnter(other);
        }
    }
}
