using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnknownWorld.Path.Data
{
    public enum PointType
    {
        Undefined,
        PathFollowing,
        FollowingSuspicion,
        FollowingTarget
    }

    public enum PointAction
    {
        Undefined,
        ContinuePath,
        Stop,        
        Attack
    }


    [System.Serializable]
    public class PathPoint
    {
        [SerializeField] private PointType m_type = PointType.PathFollowing;
        [SerializeField] private PointAction m_action = PointAction.Undefined;
        [SerializeField] [Range(0, 255)] private ushort m_priority = 0;
        [SerializeField] private IntermediatePoint m_point;

        [SerializeField] [Range(0, ushort.MaxValue)] private float m_transferDelay = 0;

        [SerializeField] [Range(0.0f, 1.0f)] private float m_minImpactSpeed = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_animationSpeed = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_movementSpeed = 1.0f;
        [SerializeField] [Range(0, 255)] private float m_accuracyRadius = 0.5f;
        [SerializeField] [Range(0, 255)] private float m_impactRadius = 0.0f;
        

        public IntermediatePoint Point
        {
            get { return this.m_point; }
            set { this.m_point = value; }
        }
        public float AccuracyRadius
        {
            get { return this.m_accuracyRadius; }
            set { this.m_accuracyRadius = value; }
        }
        public float MinImpactSpeed
        {
            get { return this.m_minImpactSpeed; }
            set { this.m_minImpactSpeed = value; }
        }
        public float AnimationSpeed
        {
            get { return this.m_animationSpeed; }
            set { this.m_animationSpeed = value; }
        }
        public Transform Transform
        {
            get { return this.m_point.transform; }
        }
        public float MovementSpeed
        {
            get { return this.m_movementSpeed; }
            set { this.m_movementSpeed = value; }
        }
        public float TransferDelay
        {
            get { return this.m_transferDelay; }
            set { this.m_transferDelay = value; }
        }
        public float ImpactRadius
        {
            get { return this.m_impactRadius; }
            set { this.m_impactRadius = value; }
        }
        public ushort Priority
        {
            get { return this.m_priority; }
            set { this.m_priority = value; }
        }
        public PointType Type
        {
            get { return this.m_type; }
            set { this.m_type = value; }
        }
        public PointAction Action
        {
            get { return this.m_action; }
            set { this.m_action = value; }
        }


        public PathPoint()
        {
            m_transferDelay = 0;
            m_minImpactSpeed = 1.0f;
            m_type = PointType.PathFollowing;
            m_action = PointAction.Undefined;
            m_priority = 0;
            m_animationSpeed = 1.0f;
            m_movementSpeed = 1.0f;
            m_accuracyRadius = 0.5f;
            m_impactRadius = 0.0f;
        }

        public void ClearPoint()
        {
            if ((m_point) &&
                (m_type == PointType.FollowingSuspicion))
                Object.Destroy(this.m_point.gameObject);
        }

        public PathPoint(PathPoint point)
        {
            this.m_transferDelay = point.m_transferDelay;
            this.m_minImpactSpeed = point.m_minImpactSpeed;
            this.m_type = point.m_type;
            this.m_action = point.m_action;
            this.m_priority = point.m_priority;
            this.m_animationSpeed = point.m_animationSpeed;
            this.m_movementSpeed = point.m_movementSpeed;
            this.m_accuracyRadius = point.m_accuracyRadius;
            this.m_impactRadius = point.m_impactRadius;
            this.m_point = point.m_point;
        }
        
    }

    public static class PathHelper
    {
        public static GameObject PathSpawner;

        public static GameObject PathPrefab;

        public static float PathWaitTime;


        public static void ClearAll(List<PathPoint> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ClearPoint();
                list.RemoveAt(i--);
            }
        }

        public static void ClearExceptOne(List<PathPoint> list, ref int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i != index)
                {
                    list[i].ClearPoint();
                    list.RemoveAt(i);
                    if (i < index)
                    {
                        i--;
                        index--;
                    }
                }
            }
        }

        public static PointType ObservationToPoint(UnknownWorld.Area.Data.ObservationType dType)
        {
            switch (dType)
            {
                case Area.Data.ObservationType.Undefined:
                    return PointType.Undefined;
                case Area.Data.ObservationType.Sonar:
                    return PointType.FollowingSuspicion;
                case Area.Data.ObservationType.Sound:
                    return PointType.FollowingSuspicion;
                case Area.Data.ObservationType.View:
                    return PointType.FollowingTarget;
                default:
                    return PointType.Undefined;
            }
        }

        public static PointAction ObservationToAction(UnknownWorld.Area.Data.ObservationType dType)
        {
            switch (dType)
            {
                case Area.Data.ObservationType.Undefined:
                    return PointAction.Undefined;
                case Area.Data.ObservationType.Sonar:
                    return PointAction.Stop;
                case Area.Data.ObservationType.Sound:
                    return PointAction.Stop;
                case Area.Data.ObservationType.View:
                    return PointAction.Attack;
                default:
                    return PointAction.Undefined;
            }
        }

        public static bool IsTargetIn(Transform target, UnknownWorld.Path.Data.PointType dType, List<UnknownWorld.Path.Data.PathPoint> targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if ((target == targets[i].Transform) &&
                    (dType == targets[i].Type))
                    return true;
            }
            return false;
        }
        
    }
}