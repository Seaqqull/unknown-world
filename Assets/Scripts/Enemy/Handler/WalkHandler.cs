using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Sound.AI
{
    public class WalkHandler : UnknownWorld.Sound.Data.BaseSoundAIHandler
    {
        public void WalkEvent(string eventKey)
        {
            if (m_animation.State != Behaviour.Data.AnimationState.Walk)//if (m_animation.State == Behaviour.Data.AnimationState.Run)
                return;

            if ((m_animation.State == Behaviour.Data.AnimationState.Walk) &&
                (m_animation.RotationState == Behaviour.Data.RotationState.Unknown))
                OnForward(eventKey);
            else if ((m_animation.RotationState == Behaviour.Data.RotationState.HalfLeft) ||
                     (m_animation.RotationState == Behaviour.Data.RotationState.HalfRight))
                OnHalf(eventKey);
        }

        private void OnHalf(string eventKey)
        {
            if (m_animation.RotationState.ToString() == eventKey)
                m_audio.Play("Walk");
        }

        private void OnForward(string eventKey)
        {
            if (m_animation.State.ToString() == eventKey)
                m_audio.Play("Walk");
        }
    }
}
