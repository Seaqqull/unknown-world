using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnknownWorld
{
    [System.Serializable]
    public enum ObserverState
    {
        unactive,
        active,
        warning,
        danger
    }

    public interface AInteracitonAnnunciator<Ttype>
    {
        bool isTargetWithinArea(SimpleData annuncicator, Ttype target, bool[] affectionMask);
    }
}
