using UnityEngine;

namespace UnknownWorld.Behaviour.Data
{
    [System.Serializable]
    public enum AnimationState
    {
        Unknown,
        Waiting,
        Walk,
        Run,
        Jump,
        InAir,
        Crouch,
        Attacking,
        Reloading,        
        Grounded
    }


    public interface IPersonAction
    {
        bool IsActionPerfomerable();

        void Attack(float animationDuration);

        void Reload(float animationDuration);

        void Move(Vector3 move, bool crouch, bool jump);        
    }
}
