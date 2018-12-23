using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld
{
    public abstract class BaseSoundCharacterHandler : MonoBehaviour
    {
        protected UnknownWorld.Behaviour.CharacterAnimationController m_animation;
        protected UnknownWorld.Sound.SoundContainer m_audio;
        
        protected virtual void Start()
        {
            m_animation = GetComponent<UnknownWorld.Behaviour.CharacterAnimationController>();
            m_audio = GetComponent<UnknownWorld.Sound.SoundContainer>();            
        }

    }
}