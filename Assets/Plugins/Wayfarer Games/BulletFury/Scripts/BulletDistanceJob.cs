using BulletFury.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace BulletFury
{
    /// <summary>
    /// A C# job that checks for collisions between bullet colliders and bullets
    /// </summary>
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct DistanceJob : IJob
    {
        [ReadOnly] public float3 CheckPosition;
        [ReadOnly] public float CheckSize;
        [ReadOnly] public float Distance;
        [ReadOnly] public float3 Position;
        [WriteOnly] public NativeArray<byte> Out;
        
        public void Execute()
        {
            var dist = math.distancesq(CheckPosition,Position);
            Out[0] = dist <= (Distance + CheckSize) * (Distance + CheckSize) ? (byte) 1 : (byte) 0;
        }
    }
    
    #if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct AABBJob : IJob
    {
        [ReadOnly] public float3 CheckPosition;
        [ReadOnly] public float CheckSize;
        [WriteOnly] public NativeArray<byte> Out;

        [ReadOnly] public float3 BoxMin;
        [ReadOnly] public float3 BoxMax;
        
        public void Execute()
        {
            var x = math.max(BoxMin.x, math.min(CheckPosition.x, BoxMax.x));
            var y = math.max(BoxMin.y, math.min(CheckPosition.y, BoxMax.y));
            var z = math.max(BoxMin.z, math.min(CheckPosition.z, BoxMax.z));

            var sqrDist = (x - CheckPosition.x) * (x - CheckPosition.x) +
                          (y - CheckPosition.y) * (y - CheckPosition.y) +
                          (z - CheckPosition.z) * (z - CheckPosition.z);
            
            Out[0] = sqrDist <= (CheckSize * CheckSize) ? (byte) 1 : (byte) 0;
        }
    }
    
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct OBBJob : IJob
    {
        [ReadOnly] public float3 CheckPosition;
        [ReadOnly] public float CheckSize;
        [WriteOnly] public NativeArray<byte> Out;
        
        [ReadOnly] public float3 BoxCentre;
        [ReadOnly] public NativeArray<float> BoxHalfExtents;
        [ReadOnly] public NativeArray<float3> BoxAxes;
        
        public void Execute()
        {
            var point = ClosestPoint(CheckPosition);
            float3 v = point - CheckPosition;
            Out[0] = math.dot(v, v) <= (CheckSize * CheckSize) ? (byte) 1 : (byte) 0;
        }

        private float3 ClosestPoint(float3 pos)
        {
            float3 d = pos - BoxCentre;
            var q = BoxCentre;

            for (int i = 0; i < 3; i++)
            {
                float dist = math.dot(d, BoxAxes[i]);
                var boxHalfExtent = BoxHalfExtents[i]/2f;
                if (dist > boxHalfExtent) dist = boxHalfExtent;
                if (dist < -boxHalfExtent) dist = -boxHalfExtent;

                q += dist * BoxAxes[i];
            }

            return q;
        }
    }
    
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct TriangleJob : IJob
    {
        [ReadOnly] public float3 CheckPosition;
        [ReadOnly] public float CheckSize;
        [WriteOnly] public NativeArray<byte> Out;

        [ReadOnly] public float3 a;
        [ReadOnly] public float3 b;
        [ReadOnly] public float3 c;

        public void Execute()
        {
            var point = ClosestPoint(CheckPosition);
            float3 v = point - CheckPosition;
            Out[0] = math.dot(v, v) <= (CheckSize * CheckSize) ? (byte)1 : (byte)0;
        }

// Finds the closest point on a triangle (defined by points a, b, and c) to a given point p
        private float3 ClosestPoint(float3 point)
        {
            // Edge vectors of the triangle
            var edgeAB = b - a;
            var edgeAC = c - a;
            var edgeBC = c - b;

            // Dot products for point projections onto triangle edges
            float dotProdPA_AB = math.dot(point - a, edgeAB);
            float dotProdPB_AB = math.dot(point - b, a - b);

            float dotProdPA_AC = math.dot(point - a, edgeAC);
            float dotProdPC_AC = math.dot(point - c, a - c);

            // Check if the closest point is vertex A
            if (dotProdPA_AB <= 0.0f && dotProdPA_AC <= 0.0f) return a;

            float dotProdPB_BC = math.dot(point - b, edgeBC);
            float dotProdPC_BC = math.dot(point - c, b - c);

            // Check if the closest point is vertex B
            if (dotProdPB_AB <= 0.0f && dotProdPB_BC <= 0.0f) return b;
            // Check if the closest point is vertex C
            if (dotProdPC_AC <= 0.0f && dotProdPC_BC <= 0.0f) return c;

            // Calculate the signed volume of the tetrahedron formed by point and triangle
            var normal = math.cross(edgeAB, edgeAC);
            var signedVolPAC = math.dot(normal, math.cross(a - point, b - point));

            // Check if the closest point is on edge AB
            if (signedVolPAC <= 0 && dotProdPA_AB >= 0.0f && dotProdPB_AB >= 0.0f)
                return a + dotProdPA_AB / (dotProdPA_AB + dotProdPB_AB) * edgeAB;

            float signedVolPAB = math.dot(normal, math.cross(b - point, c - point));

            // Check if the closest point is on edge BC
            if (signedVolPAB <= 0.0f && dotProdPB_BC >= 0.0f && dotProdPC_BC >= 0.0f)
                return b + dotProdPB_BC / (dotProdPB_BC + dotProdPC_BC) * edgeBC;

            float signedVolPBC = math.dot(normal, math.cross(c - point, a - point));

            // Check if the closest point is on edge AC
            if (signedVolPBC <= 0.0f && dotProdPA_AC >= 0.0f && dotProdPC_AC >= 0.0f)
                return a + dotProdPA_AC / (dotProdPA_AC + dotProdPC_AC) * edgeAC;

            // Calculate barycentric coordinates
            float u = signedVolPAB / (signedVolPAB + signedVolPAC + signedVolPBC);
            float v = signedVolPAC / (signedVolPAB + signedVolPAC + signedVolPBC);
            float w = 1.0f - u - v;

            // Calculate the closest point using barycentric coordinates
            return u * a + v * b + w * c;
        }

    }



    /// <summary>
    /// A C# job that checks for collisions between bullet colliders and bullets
    /// </summary>
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct BulletDistanceJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float Distance;
        [ReadOnly] public float3 Position;
        
        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var dist = math.distancesq(Position, container.Position); 

            container.Collided = dist <= (Distance + container.ColliderSize) * (Distance + container.ColliderSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }
    }

#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct BulletAABBJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float3 BoxMin;
        [ReadOnly] public float3 BoxMax;

        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var x = math.max(BoxMin.x, math.min(container.Position.x, BoxMax.x));
            var y = math.max(BoxMin.y, math.min(container.Position.y, BoxMax.y));
            var z = math.max(BoxMin.z, math.min(container.Position.z, BoxMax.z));

            var sqrDist = (x - container.Position.x) * (x - container.Position.x) +
                          (y - container.Position.y) * (y - container.Position.y) +
                          (z - container.Position.z) * (z - container.Position.z);

            container.Collided = sqrDist <= (container.ColliderSize * container.ColliderSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }
    }
    
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct BulletOBBJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float3 BoxCentre;
        [ReadOnly] public NativeArray<float> BoxHalfExtents;
        [ReadOnly] public NativeArray<float3> BoxAxes;
        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var point = ClosestPoint(container.Position);
            float3 v = point - container.Position;
            container.Collided = math.dot(v, v) <= (container.ColliderSize * container.ColliderSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }

        private float3 ClosestPoint(float3 pos)
        {
            float3 d = pos - BoxCentre;
            var q = BoxCentre;

            for (int i = 0; i < 3; i++)
            {
                float dist = math.dot(d, BoxAxes[i]);
                var boxHalfExtent = BoxHalfExtents[i]/2f;
                if (dist > boxHalfExtent) dist = boxHalfExtent;
                if (dist < -boxHalfExtent) dist = -boxHalfExtent;

                q += dist * BoxAxes[i];
            }

            return q;
        }
    }
    
#if !UNITY_EDITOR
    [BurstCompile]
#endif
    public struct BulletTriangleJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletContainer> In;
        [WriteOnly] public NativeArray<BulletContainer> Out;
        [ReadOnly] public float3 a;
        [ReadOnly] public float3 b;
        [ReadOnly] public float3 c;
        
        public void Execute(int index)
        {
            var container = In[index];
            if (container.Dead == 1)
            {
                Out[index] = container;
                return;
            }

            var point = ClosestPoint(container.Position);
            float3 v = point - container.Position;
            container.Collided = math.dot(v, v) <= (container.ColliderSize * container.ColliderSize) ? (byte) 1 : (byte) 0;
            Out[index] = container;
        }

        private float3 ClosestPoint(float3 p)
        {
            var ab = b - a;
            var ac = c - a;
            var bc = c - b;

            float snom = math.dot(p - a, ab);
            float sdenom = math.dot(p - b, a - b);

            float tnom = math.dot(p - a, ac);
            float tdenom = math.dot(p - c, a - c);

            if (snom <= 0.0f && tnom <= 0.0f) return a;

            float unom = math.dot(p - b, bc);
            float udenom = math.dot(p - c, b - c);

            if (sdenom <= 0.0f && unom <= 0.0f) return b;
            if (tdenom <= 0.0f && udenom <= 0.0f) return c;

            var n = math.cross(b - a, c - a);
            var vc = math.dot(n, math.cross(a - p, b - p));

            if (vc <= 0 && snom >= 0.0f && sdenom >= 0.0f)
                return a + snom / (snom + sdenom) * ab;

            float va = math.dot(n, math.cross(b - p, c - p));

            if (va <= 0.0f && unom >= 0.0f && udenom >= 0.0f)
                return b + unom / (unom + udenom) * bc;

            float vb = math.dot(n, math.cross(c - p, a - p));

            if (vb <= 0.0f && tnom >= 0.0f && tdenom >= 0.0f)
                return a + tnom / (tnom + tdenom) * ac;

            float u = va / (va + vb + vc);
            float v = vb / (va + vb + vc);
            float w = 1.0f - u - v;
            return u * a + v * b + w * c;
        }
    }
}