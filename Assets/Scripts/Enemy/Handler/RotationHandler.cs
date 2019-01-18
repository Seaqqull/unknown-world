using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Sound.AI
{
    public class RotationHandler : UnknownWorld.Sound.Data.BaseSoundAIHandler
    {
        public void RotationEvent(string eventKey)
        {
            if (m_animation.State != Behaviour.Data.AnimationState.Waiting)
                return;

            if ((m_animation.RotationState == Behaviour.Data.RotationState.QuarterLeft) ||
                (m_animation.RotationState == Behaviour.Data.RotationState.QuarterRight))
                OnQuater(eventKey);
            else if ((m_animation.RotationState == Behaviour.Data.RotationState.HalfLeft) ||
                     (m_animation.RotationState == Behaviour.Data.RotationState.HalfRight))
                OnHalf(eventKey);
        }

        private void OnHalf(string eventKey)
        {
            if (m_animation.RotationState.ToString() == eventKey)
                m_audio.Play("InPlace");
        }

        private void OnQuater(string eventKey)
        {
            if (m_animation.RotationState.ToString() == eventKey)
                m_audio.Play("InPlace");
        }
    }
}
