using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Weapon.Ammo
{
    public class SimpleBullet : Bullet
    {
        public override void Launch()
        {
            base.Launch();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            OnBulletDestroy();
            if (((1 << other.gameObject.layer) & m_targetMask) == 0)
            {
                UnknownWorld.Behaviour.PersonBehaviour affectedTarget = other.GetComponent<UnknownWorld.Behaviour.PersonBehaviour>();//.DoDamage(m_damage * m_damageScale);
                if (affectedTarget)
                {
                    Debug.Log("Bullet hit target");
                }                
            }
            Destroy(gameObject);
        }
    }
}
