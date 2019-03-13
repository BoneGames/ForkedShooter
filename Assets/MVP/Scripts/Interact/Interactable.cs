using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class Interactable : MonoBehaviour
    {

        public virtual void Interact()
        {
            Debug.Log("You hit the base class!");
        }
    }
}
