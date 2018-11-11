using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnknownWorld.Path.Data
{
    public class PathPointWrapper : MonoBehaviour
    {
        private PathPoint m_point;

        public PathPoint Point
        {
            get { return this.m_point; }
            set { m_point = value; }
        }
    }
}