using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class WalkHandler : BaseSoundCharacterHandler
    {
        public void WalkEvent()
        {
            if (m_animation.State == Behaviour.Data.AnimationState.Run)
                return;
            //m_container.Play("Walk");
            if ((m_animation.State == Behaviour.Data.AnimationState.Walk) &&
                (m_animation.RotationState == Behaviour.Data.RotationState.Unknown))
                Debug.Log("Walk");
            else if (m_animation.RotationState != Behaviour.Data.RotationState.Unknown)
                Debug.Log("Walk");
        }

    }
}
