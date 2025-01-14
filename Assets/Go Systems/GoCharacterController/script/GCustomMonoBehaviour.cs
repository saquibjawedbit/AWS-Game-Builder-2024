using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace GoSystem
{
    public abstract class GoSystemsBehaviour : NetworkBehaviour
    {
          [HideInInspector, SerializeField]
            private bool isOpen;

    }
}