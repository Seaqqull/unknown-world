using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Weapon.Ammo
{
    public abstract class Bullet : MonoBehaviour
    {
        [SerializeField] protected ParticleSystem m_explosionParticle;
        [SerializeField] protected ParticleSystem m_shotParticle;
        [SerializeField] protected ParticleSystem m_flyParticle;

        [SerializeField] protected AudioSource m_explosionAudio;        
        [SerializeField] protected AudioSource m_shotAudio;
        [SerializeField] protected AudioSource m_flyAudio;

        [SerializeField] protected float m_damageScale = 1.0f;
        [SerializeField] protected float m_speedScale = 1.0f;
        [SerializeField] protected float m_rangeScale = 1.0f;

        protected static uint m_idCounter = 0;        
        protected bool m_isLaunched = false;
        protected Vector3 m_startPosition;
        protected LayerMask m_targetMask;
        protected float m_damage;
        protected float m_speed;
        protected float m_range;
        protected uint m_id;

        public LayerMask TargetMask
        {
            get { return this.m_targetMask; }
            set { this.m_targetMask = value; }
        }
        public float DamageScale
        {
            get { return this.m_damageScale; }
            set { this.m_damageScale = value; }
        }
        public float SpeedScale
        {
            get { return this.m_speedScale; }
            set { this.m_speedScale = value; }
        }
        public float RangeScale
        {
            get { return this.m_rangeScale; }
            set { this.m_rangeScale = value; }
        }
        public float Damage
        {
            get { return this.m_damage; }
            set { this.m_damage = value; }
        }
        public float Speed
        {
            get { return this.m_speed; }
            set { this.m_speed = value; }
        }
        public float Range
        {
            get { return this.m_range; }
            set { this.m_range = value; }
        }
        public uint Id
        {
            get { return this.m_id; }
        }


        protected void Awake()
        {
            m_id = m_idCounter++;
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (!m_isLaunched) return;

            transform.position += Vector3.forward * (m_speed * m_speedScale * Time.deltaTime);

            if ((m_rangeScale != 0.0f) && 
                (Vector3.Distance(transform.position, m_startPosition) > (m_range * m_rangeScale)))
            {
                DestroyBullet();
            }
        }


        protected virtual void DestroyBullet()
        {
            Destroy(gameObject);
        }

        protected virtual void OnBulletStart()
        {
            // Start shot action
            if (m_shotParticle)
            {
                m_shotParticle.transform.parent = null;
                m_shotParticle.Play();
                Destroy(m_shotParticle.gameObject, m_shotParticle.main.duration);
            }

            if (m_shotAudio)
                m_shotAudio.Play();

            // Fly action
            if (m_flyParticle)
                m_flyParticle.Play();

            if (m_flyAudio)
                m_flyAudio.Play();
        }

        protected virtual void OnBulletDestroy()
        {
            // Fly action
            if (m_flyParticle)
                m_flyParticle.Stop();

            if (m_flyAudio)
                m_flyAudio.Stop();

            // Explosion action
            if (m_explosionParticle)
            {
                m_explosionParticle.transform.parent = null;
                m_explosionParticle.Play();
                Destroy(m_explosionParticle.gameObject, m_explosionParticle.main.duration);
            }

            if (m_explosionAudio)
                m_explosionAudio.Play();
        }


        public virtual void Launch()
        {
            m_startPosition = transform.position;
            m_isLaunched = true;            

            OnBulletStart();
        }

        public void SetBulletParams(float damage, float speed, float range, LayerMask targetMask)
        {
            TargetMask = targetMask;
            Damage = damage;
            Speed = speed;
            Range = range;
        }


        protected abstract void OnTriggerEnter(Collider other);
    }
}
