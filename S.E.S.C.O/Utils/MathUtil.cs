using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public static class MathUtil
    {
        public static Vector2 GetRandomPosition(Vector3 center, float radius)
        {
            var randomDirection = Random.insideUnitSphere * radius;
            randomDirection += center;
            randomDirection = center + (randomDirection - center).normalized * radius;
            return randomDirection;
        }

        public static Vector3 GetRandomDirection(Vector3 direction, float angle)
        {
            var randomDirection = Quaternion.Euler(0, 0, Random.Range(-angle, angle)) * direction;
            return randomDirection;
        }

        public static Quaternion GetQuaternion(Vector3 direction)
        {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, angle);
        }
        
        public static Quaternion GetQuaternion(Vector3 start, Vector3 end)
        {
            var direction = end - start;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, angle);
        }
        
        public static Vector2 GetDirectionFromQuaternion(Quaternion rotation)
        {
            Vector3 forward = rotation * Vector3.right;
            return new Vector2(forward.x, forward.y).normalized;
        }
    }
}
