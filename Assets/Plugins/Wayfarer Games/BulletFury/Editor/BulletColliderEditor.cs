using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BulletFury.Editor
{
    [CustomEditor(typeof(BulletCollider)), CanEditMultipleObjects]
    public class BulletColliderEditor : UnityEditor.Editor
    {
        private SerializedProperty _hitByBulletsList;
        private SerializedProperty _radius;
        private SerializedProperty _onCollide;
        private SerializedProperty _shape;
        private SerializedProperty _bounds;
        private SerializedProperty _offset;
        private SerializedProperty _pointA;
        private SerializedProperty _pointB;
        private SerializedProperty _pointC;
        private SerializedProperty _destroyBullet;
        private SerializedProperty _bulletLayersToBeHitBy;
        private SerializedProperty _bounce;
        private SerializedProperty _lifetimeLoss;
        private SerializedProperty _bounciness;
        
        private void OnEnable()
        {
            _hitByBulletsList = serializedObject.FindProperty("hitByBullets");
            _radius = serializedObject.FindProperty("radius");
            _onCollide = serializedObject.FindProperty("OnCollide");
            _shape = serializedObject.FindProperty("shape");
            _bounds = serializedObject.FindProperty("size");
            _offset = serializedObject.FindProperty("center");
            _pointA = serializedObject.FindProperty("pointA");
            _pointB = serializedObject.FindProperty("pointB");
            _pointC = serializedObject.FindProperty("pointC");
            _bulletLayersToBeHitBy = serializedObject.FindProperty("bulletLayersToBeHitBy");
            _destroyBullet = serializedObject.FindProperty("destroyBullet");
            
            _bounce = serializedObject.FindProperty("bounce");
            _lifetimeLoss = serializedObject.FindProperty("lifetimeLoss");
            _bounciness = serializedObject.FindProperty("bounciness");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_bulletLayersToBeHitBy);

            EditorGUILayout.PropertyField(_shape);

            if ((ColliderShape) _shape.enumValueIndex != ColliderShape.Triangle)
            {
                EditorGUILayout.PropertyField(_offset);
                EditorGUILayout.PropertyField((ColliderShape) _shape.enumValueIndex == ColliderShape.Sphere
                    ? _radius
                    : _bounds);
            }
            else
            {
                EditorGUILayout.PropertyField(_pointA);
                EditorGUILayout.PropertyField(_pointB);
                EditorGUILayout.PropertyField(_pointC);
            }

            EditorGUILayout.PropertyField(_onCollide);
            if ((ColliderShape)_shape.enumValueIndex == ColliderShape.OBB)
                EditorGUILayout.HelpBox("Hint: Do you need rotated boxes? This has significant performance implications, use axis-aligned bounding boxes where possible", MessageType.Warning);

            EditorGUILayout.PropertyField(_bounce);
            if (_bounce.boolValue)
            {
                EditorGUILayout.PropertyField(_bounciness);
                EditorGUILayout.PropertyField(_lifetimeLoss);
            }
            else
                EditorGUILayout.PropertyField(_destroyBullet);
            
            serializedObject.ApplyModifiedProperties();
        }

        private Vector3? _testPos;
        private Quaternion _testRot = Quaternion.identity;
        private void OnSceneGUI()
        {
            return;
            var collider = (BulletCollider) target;
            var scale = Vector3.Scale(collider.transform.localScale, _bounds.vector3Value);

            if (_testPos == null)
                _testPos = collider.transform.position + _offset.vector3Value + Vector3.up;

            _testRot = Handles.RotationHandle(_testRot, _testPos.Value);
            _testPos = Handles.PositionHandle(_testPos.Value, _testRot);
            
            var dir = _testPos.Value - (collider.transform.position + _offset.vector3Value);
            Handles.DrawLine(_offset.vector3Value + collider.transform.position, _testPos.Value);

            if ((ColliderShape) _shape.enumValueIndex == ColliderShape.AABB)
            {
                dir.x = Mathf.InverseLerp(-scale.x / 2f, scale.x / 2f, dir.x);
                dir.y = Mathf.InverseLerp(-scale.y / 2f, scale.y / 2f, dir.y);
                dir.z = Mathf.InverseLerp(-scale.z / 2f, scale.z / 2f, dir.z);
                Handles.Label(_testPos.Value + Vector3.right, $"Dir: {dir}");
                var bounds = new Bounds(collider.transform.position + _offset.vector3Value, Vector3.Scale(collider.transform.localScale, _bounds.vector3Value));

                var normal = ( collider.ComputeAABBNormal(_testPos.Value));

                Handles.DrawLine(bounds.ClosestPoint(_testPos.Value), bounds.ClosestPoint(_testPos.Value) + normal);
                Handles.DrawLine(bounds.ClosestPoint(_testPos.Value), bounds.ClosestPoint(_testPos.Value) + Vector3.Reflect(_testRot * Vector3.up, normal), 4);
                Handles.DrawLine(bounds.ClosestPoint(_testPos.Value),
                    bounds.ClosestPoint(_testPos.Value) - _testRot * Vector3.up);
                
            } else if ((ColliderShape) _shape.enumValueIndex == ColliderShape.OBB)
            {
                
                dir = Quaternion.Inverse(collider.transform.rotation) * dir;

                dir.x = (Mathf.InverseLerp(-scale.x / 2f, scale.x / 2f, dir.x) - 0.5f) * 2;
                dir.y = (Mathf.InverseLerp(-scale.y / 2f, scale.y / 2f, dir.y) - 0.5f) * 2;
                dir.z = (Mathf.InverseLerp(-scale.z / 2f, scale.z / 2f, dir.z) - 0.5f) * 2;
            
                dir = collider.transform.rotation * dir;
                Handles.Label(_testPos.Value + Vector3.right, $"Dir: {dir}");
                
                var scaledSize = Vector3.Scale(collider.transform.localScale, _bounds.vector3Value);

                var BoxHalfExtents = new[] {scaledSize.x, scaledSize.y, scaledSize.z};
                var BoxAxes = new[] {collider.transform.right, collider.transform.up, collider.transform.forward};
                var BoxCentre = collider.transform.position + _offset.vector3Value;
                
                var normal = ( collider.ComputeOBBNormal(_testPos.Value));

                Handles.color = Color.cyan;
                Handles.DrawLine(OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents), OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents) + normal);
                Handles.color = Color.white;

                Handles.DrawLine(OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents),
                    OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents) + Vector3.Reflect(_testRot * Vector3.up, normal), 4);
                Handles.DrawLine(OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents),
                    OBBClosestPoint(_testPos.Value, BoxCentre, BoxAxes, BoxHalfExtents) - _testRot * Vector3.up);
            }
            //Handles.Label(_testPos.Value + Vector3.right, $"Dot: {}");
        }

        private Vector3 OBBClosestPoint(Vector3 pos, Vector3 BoxCentre, Vector3[] BoxAxes, float[] BoxHalfExtents)
        {
            Vector3 d = pos - BoxCentre;
            var q = BoxCentre;

            for (int i = 0; i < 3; i++)
            {
                float dist = Vector3.Dot(d, BoxAxes[i]);
                var boxHalfExtent = BoxHalfExtents[i]/2f;
                if (dist > boxHalfExtent) dist = boxHalfExtent;
                if (dist < -boxHalfExtent) dist = -boxHalfExtent;

                q += dist * BoxAxes[i];
            }

            return q;
        }
    }
}