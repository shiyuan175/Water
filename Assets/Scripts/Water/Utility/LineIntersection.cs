using UnityEngine;

namespace Game.Water
{
    public static class LineIntersection
    {
        /// <summary>
        /// 计算两条无限直线的交点
        /// </summary>
        /// <param name="line1Start">直线1上的一点</param>
        /// <param name="line1End">直线1上的另一点</param>
        /// <param name="line2Start">直线2上的一点</param>
        /// <param name="line2End">直线2上的另一点</param>
        /// <param name="intersection">输出交点</param>
        /// <returns>是否有交点（平行线返回false）</returns>
        public static bool GetLineIntersection(Vector2 line1Start, Vector2 line1End,
            Vector2 line2Start, Vector2 line2End,
            out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 dir1 = line1End - line1Start;
            Vector2 dir2 = line2End - line2Start;

            // 计算行列式，判断是否平行
            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;

            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                // 平行线，无交点
                return false;
            }

            Vector2 diff = line2Start - line1Start;
            float t = (diff.x * dir2.y - diff.y * dir2.x) / denominator;

            intersection = line1Start + t * dir1;
            return true;
        }

        /// <summary>
        /// 计算两条线段的交点
        /// </summary>
        /// <param name="seg1Start">线段1起点</param>
        /// <param name="seg1End">线段1终点</param>
        /// <param name="seg2Start">线段2起点</param>
        /// <param name="seg2End">线段2终点</param>
        /// <param name="intersection">输出交点</param>
        /// <returns>是否有交点</returns>
        public static bool GetSegmentIntersection(Vector2 seg1Start, Vector2 seg1End,
            Vector2 seg2Start, Vector2 seg2End,
            out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 dir1 = seg1End - seg1Start;
            Vector2 dir2 = seg2End - seg2Start;

            float denominator = dir1.x * dir2.y - dir1.y * dir2.x;

            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                // 平行线段，无交点
                return false;
            }

            Vector2 diff = seg2Start - seg1Start;
            float t1 = (diff.x * dir2.y - diff.y * dir2.x) / denominator;
            float t2 = (diff.x * dir1.y - diff.y * dir1.x) / denominator;

            // 检查交点是否在两条线段上
            if (t1 >= 0f && t1 <= 1f && t2 >= 0f && t2 <= 1f)
            {
                intersection = seg1Start + t1 * dir1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 计算直线和线段的交点
        /// </summary>
        /// <param name="lineStart">直线上的一点</param>
        /// <param name="lineEnd">直线上的另一点</param>
        /// <param name="segStart">线段起点</param>
        /// <param name="segEnd">线段终点</param>
        /// <param name="intersection">输出交点</param>
        /// <returns>是否有交点</returns>
        public static bool GetLineSegmentIntersection(Vector2 lineStart, Vector2 lineEnd,
            Vector2 segStart, Vector2 segEnd,
            out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 lineDir = lineEnd - lineStart;
            Vector2 segDir = segEnd - segStart;

            float denominator = lineDir.x * segDir.y - lineDir.y * segDir.x;

            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                // 平行，无交点
                return false;
            }

            Vector2 diff = segStart - lineStart;
            float t = (diff.x * segDir.y - diff.y * segDir.x) / denominator;
            float u = (diff.x * lineDir.y - diff.y * lineDir.x) / denominator;

            // 检查交点是否在线段上（u在[0,1]范围内）
            if (u >= 0f && u <= 1f)
            {
                intersection = lineStart + t * lineDir;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查两条线段是否相交（不需要交点坐标）
        /// </summary>
        public static bool DoSegmentsIntersect(Vector2 seg1Start, Vector2 seg1End,
            Vector2 seg2Start, Vector2 seg2End)
        {
            Vector2 intersection;
            return GetSegmentIntersection(seg1Start, seg1End, seg2Start, seg2End, out intersection);
        }

        /// <summary>
        /// 计算点到直线的距离
        /// </summary>
        public static float DistancePointToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            Vector2 lineDir = lineEnd - lineStart;
            Vector2 pointDir = point - lineStart;

            float lineLengthSq = lineDir.sqrMagnitude;
            if (lineLengthSq < Mathf.Epsilon)
                return Vector2.Distance(point, lineStart);

            float cross = pointDir.x * lineDir.y - pointDir.y * lineDir.x;
            return Mathf.Abs(cross) / Mathf.Sqrt(lineLengthSq);
        }

        /// <summary>
        /// 计算点到线段的最短距离
        /// </summary>
        public static float DistancePointToSegment(Vector2 point, Vector2 segStart, Vector2 segEnd)
        {
            Vector2 segDir = segEnd - segStart;
            Vector2 pointDir = point - segStart;

            float segLengthSq = segDir.sqrMagnitude;
            if (segLengthSq < Mathf.Epsilon)
                return Vector2.Distance(point, segStart);

            float t = Vector2.Dot(pointDir, segDir) / segLengthSq;
            t = Mathf.Clamp01(t);

            Vector2 projection = segStart + t * segDir;
            return Vector2.Distance(point, projection);
        }
    }
}
