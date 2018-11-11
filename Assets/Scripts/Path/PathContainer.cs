using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path
{
    public abstract class PathContainer : MonoBehaviour
    {
        [SerializeField] protected List<UnknownWorld.Path.Data.PathPoint> m_points;
        [SerializeField] protected Color m_impactColor = Color.yellow;
        [SerializeField] protected Color m_accuracyColor = Color.red;
        [SerializeField] protected Color m_lineColor = Color.green;
        [SerializeField] protected Transform m_ownerTransform;
        [SerializeField] protected bool m_isRandom = false;        
        [SerializeField] protected int m_startupPoint = 0;

        public List<UnknownWorld.Path.Data.PathPoint> Points
        {
            get
            {
                return this.m_points ??
                    (this.m_points = new List<UnknownWorld.Path.Data.PathPoint>());
            }
            set { this.m_points = value; }
        }
        public float CurrentTransferDelay
        {
            get { return this.m_points[m_previousPoint].TransferDelay; }
        }
        public bool IsRandom
        {
            get { return this.m_isRandom; }
            set { this.m_isRandom = value; }
        }
        public int Length
        {
            get { return Points.Count; }
        }



        //protected int m_prePreviousPoint = -1;
        private int m_destinationPoint = 0;
        private int m_previousPoint = -1;
        protected System.Random m_random;


        protected virtual void Awake()
        {
            if (m_ownerTransform == null)
                m_ownerTransform = GetComponent<Transform>();
            m_random = new System.Random();
            m_destinationPoint = m_startupPoint;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (Points.Count == 0) return;

            Gizmos.color = m_lineColor;

            int i;
            for (i = 0; i < m_points.Count - 1; i++)
            {
                if ((m_points[i].Transform != null) &&
                   (m_points[i + 1].Transform != null))
                {
                    Gizmos.DrawLine(m_points[i].Transform.position,
                        m_points[i + 1].Transform.position);

                    UnityEditor.Handles.color = m_impactColor;
                    UnityEditor.Handles.DrawWireArc(m_points[i].Transform.position,
                        Vector3.up, Vector3.forward, 360, m_points[i].ImpactRadius);
                    UnityEditor.Handles.color = m_accuracyColor;
                    UnityEditor.Handles.DrawWireArc(m_points[i].Transform.position,
                        Vector3.up, Vector3.forward, 360, m_points[i].AccuracyRadius);

                }
            }
            UnityEditor.Handles.color = m_impactColor;
            UnityEditor.Handles.DrawWireArc(m_points[i].Transform.position,
                Vector3.up, Vector3.forward, 360, m_points[i].ImpactRadius);
            UnityEditor.Handles.color = m_accuracyColor;
            UnityEditor.Handles.DrawWireArc(m_points[i].Transform.position,
                Vector3.up, Vector3.forward, 360, m_points[i].AccuracyRadius);
        }


        public void Clear()
        {
            m_points.Clear();
            ResetToZero();
        }

        public void ResetToZero()
        {
            m_destinationPoint = 0;
            m_previousPoint = -1;
        }

        public virtual void RemoveAt(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception("wrong path index");

            Points[index].ClearPoint();
            Points.RemoveAt(index);
        }

        public virtual void RemoveAt(int index, ref int relIndex)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception("wrong path index");

            Points[index].ClearPoint();
            Points.RemoveAt(index);

            if (index == relIndex)
                relIndex = -1;
            else if (index < relIndex)
                relIndex--;            
        }

        public virtual Vector3 GetPathPosition(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception("wrong path index");

            return m_points[index].Transform.position;
        }

        public UnknownWorld.Path.Data.PathPoint GetPoint(int index)
        {
            if ((index < 0) ||
                (Points.Count == 0) ||
                (index >= Points.Count))
                throw new System.Exception("wrong path index");

            return m_points[index];
        }

        public virtual Vector3 GetClosestPoint(ref int destination)
        {
            if (Points.Count == 0)
                throw new System.Exception("empty path");

            float distanceMin = int.MaxValue;
            float distanceTemp = 0.0f;

            for (int i = 0; i < m_points.Count; i++)
            {
                distanceTemp = Vector3.Distance(m_ownerTransform.position,
                    m_points[i].Transform.position);
                if (distanceTemp < distanceMin)
                {
                    distanceMin = distanceTemp;
                    destination = i;
                }
            }

            m_previousPoint = destination;
            m_destinationPoint = (m_previousPoint + 1) % m_points.Count;

            return m_points[destination].Transform.position;
        }

        public virtual Vector3 GetNextPoint(ref int destinationIndex)
        {
            if (Points.Count == 0)
                throw new System.Exception("empty path");

            Vector3 destination = m_points[m_destinationPoint].Transform.position;
            destinationIndex = m_destinationPoint;

            //m_prePreviousPoint = m_previousPoint;
            m_previousPoint = m_destinationPoint;
            m_destinationPoint = (m_destinationPoint + 1) % m_points.Count;

            return destination;
        }

        public virtual Vector3 GetRandomPoint(ref int destinationIndex)
        {
            if (Points.Count == 0)
                throw new System.Exception("empty path");

            int randomPoint;

            do
            {
                randomPoint = m_random.Next(0, m_points.Count);
            }
            while ((randomPoint == m_previousPoint) ||
                   (randomPoint == m_destinationPoint));


            Vector3 destination = m_points[m_destinationPoint].Transform.position;
            destinationIndex = m_destinationPoint;
            //m_prePreviousPoint = m_previousPoint;
            m_previousPoint = m_destinationPoint;
            m_destinationPoint = randomPoint;

            return destination;
        }

        public virtual void Add(UnknownWorld.Path.Data.PathPoint point)
        {
            Points.Add(point);
        }

        public virtual void Add(List<UnknownWorld.Path.Data.PathPoint> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                m_points.Add(points[i]);
            }
            points.Clear();
        }

        public virtual void Add(UnknownWorld.Path.Data.PathPoint point, ref int index)
        {
            index = m_points.Count;
            m_points.Add(point);            
        }

        public void CalculateSpeedOnPath(ref float speed, Vector3 position)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                CalculateSpeedOnPoint(ref speed, position, i);
            }
        }

        public void CalculateSpeedOnPoint(ref float speed, Vector3 position, int index)
        {
            float distance = Vector3.Distance(position, m_points[index].Transform.position);

            if (distance <= m_points[index].ImpactRadius)
            {
                speed = Mathf.Lerp(speed * m_points[index].MinImpactSpeed, speed,
                        distance / m_points[index].ImpactRadius);
            }
        }


        public abstract Vector3 GetDestination(ref int destinationIndex);
    }
}
