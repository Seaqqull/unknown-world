using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public class RunHandler : BaseSoundCharacterHandler
    {
        public void RunEvent()
        {
            if (m_animation.State == Behaviour.Data.AnimationState.Walk)
                return;
            //m_container.Play("Run");
            if ((m_animation.State == Behaviour.Data.AnimationState.Run) &&
                (m_animation.RotationState == Behaviour.Data.RotationState.Unknown))
                Debug.Log("Run");
            else if (m_animation.RotationState != Behaviour.Data.RotationState.Unknown)
                Debug.Log("Run");            
        }

    }
}
