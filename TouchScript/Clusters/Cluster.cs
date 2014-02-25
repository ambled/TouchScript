﻿/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TouchScript.Clusters
{
    /// <summary>
    /// Abstract base for points separated into clusters.
    /// </summary>
    public abstract class Cluster
    {
        #region Public properties

        /// <summary>
        /// Number of total points in clusters represented by this object.
        /// </summary>
        public int PointsCount
        {
            get { return points.Count; }
        }

        #endregion

        #region Private variables

        /// <summary>
        /// List of points in clusters.
        /// </summary>
        protected List<ITouchPoint> points = new List<ITouchPoint>();

        /// <summary>
        /// Indicates if clusters must be rebuilt.
        /// </summary>
        protected bool dirty { get; private set; }

        private static StringBuilder hashString = new StringBuilder();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Cluster"/> class.
        /// </summary>
        protected Cluster()
        {
            markDirty();
        }

        #region Static methods

        /// <summary>
        /// Returns a camera which was used to capture cluster's points.
        /// </summary>
        /// <param name="touches">List of touch points.</param>
        /// <returns>Camera instance.</returns>
        public static Camera GetClusterCamera(IList<ITouchPoint> touches)
        {
            if (touches.Count == 0) return Camera.main;
            var cam = touches[0].Layer.Camera;
            if (cam == null) return Camera.main;
            return cam;
        }

        /// <summary>
        /// Calculates the centroid of touch points' positions.
        /// </summary>
        /// <param name="touches">List of touch points.</param>
        /// <returns>Centroid of touch points' positions or <see cref="TouchManager.INVALID_POSITION"/> if cluster contains no points.</returns>
        public static Vector2 Get2DCenterPosition(IList<ITouchPoint> touches)
        {
            var length = touches.Count;
            if (length == 0) return TouchManager.INVALID_POSITION;
            if (length == 1) return touches[0].Position;

            var position = new Vector2();
            foreach (var point in touches) position += point.Position;
            return position/(float)length;
        }

        /// <summary>
        /// Calculates the centroid of previous touch points' positions.
        /// </summary>
        /// <param name="touches">List of touch points.</param>
        /// <returns>Centroid of previous touch point's positions or <see cref="TouchManager.INVALID_POSITION"/> if cluster contains no points.</returns>
        public static Vector2 GetPrevious2DCenterPosition(IList<ITouchPoint> touches)
        {
            var length = touches.Count;
            if (length == 0) return TouchManager.INVALID_POSITION;
            if (length == 1) return touches[0].PreviousPosition;

            var position = new Vector2();
            foreach (var point in touches) position += point.PreviousPosition;
            return position/(float)length;
        }

        /// <summary>
        /// Computes a unique hash for a list of touch points.
        /// </summary>
        /// <param name="touches">List of touch points.</param>
        /// <returns>A unique string for a list of touch points.</returns>
        public static String GetPointsHash(IList<ITouchPoint> touches)
        {
            hashString.Remove(0, hashString.Length);
            for (var i = 0; i < touches.Count; i++)
            {
                hashString.Append("#");
                hashString.Append(touches[i].Id);

            }
            return hashString.ToString();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a point to cluster.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns></returns>
        public void AddPoint(ITouchPoint point)
        {
            if (points.Contains(point)) return;

            points.Add(point);
            markDirty();
        }

        /// <summary>
        /// Adds a list of points to cluster.
        /// </summary>
        /// <param name="points">List of points.</param>
        public void AddPoints(IList<ITouchPoint> points)
        {
            foreach (var point in points) AddPoint(point);
        }

        /// <summary>
        /// Removes a point from cluster.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns></returns>
        public void RemovePoint(ITouchPoint point)
        {
            if (!points.Contains(point)) return;

            points.Remove(point);
            markDirty();
        }

        /// <summary>
        /// Removes a list of points from cluster.
        /// </summary>
        /// <param name="points">List of points.</param>
        public void RemovePoints(IList<ITouchPoint> points)
        {
            foreach (var point in points) RemovePoint(point);
        }

        /// <summary>
        /// Removes all points from cluster.
        /// </summary>
        public void RemoveAllPoints()
        {
            points.Clear();
            markDirty();
        }

        /// <summary>
        /// Invalidates cluster state.
        /// Call this method to recalculate cluster properties.
        /// </summary>
        public void Invalidate()
        {
            markDirty();
        }

        #endregion

        #region Protected functions

        /// <summary>
        /// Signals that clusters changed and must be rebuilt.
        /// </summary>
        protected void markDirty()
        {
            dirty = true;
        }

        /// <summary>
        /// Signals that clusters have just been rebuilt.
        /// </summary>
        protected void markClean()
        {
            dirty = false;
        }

        #endregion
    }
}