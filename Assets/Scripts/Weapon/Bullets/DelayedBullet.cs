using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnknownWorld.Weapon.Ammo
{
    class DelayedBullet : Bullet
    {
        [SerializeField] [Range(0, ushort.MaxValue)] private float m_lifetime = 0.5f;

        private List<UnknownWorld.Area.Target.TracingArea> m_affectedTargets;

        public float Lifetime
        {
            get { return this.m_lifetime; }
            set { this.m_lifetime = value; }
        }


        protected override void OnTriggerEnter(Collider other)
        {
            if ((!m_isLaunched) ||
                (((1 << other.gameObject.layer) & m_targetMask) == 0)) return;

            UnknownWorld.Area.Target.TracingArea affectedArea = other.GetComponent<UnknownWorld.Area.Target.TracingArea>();

            if (affectedArea)
                m_affectedTargets.Add(affectedArea);
        }        

        protected override void DestroyBullet()
        {
            foreach (var item in m_affectedTargets.Distinct(new Utility.Methods.AreaEqualityComparer()))
            {
                item.PerformDamage(m_damage * m_damageScale);
            }
            OnBulletDestroy();

            base.DestroyBullet();
        }

        protected override void OnBulletStart()
        {
            base.OnBulletStart();

            m_affectedTargets = new List<UnknownWorld.Area.Target.TracingArea>();
            Invoke("DestroyBullet", m_lifetime);
        }        
    }
}
