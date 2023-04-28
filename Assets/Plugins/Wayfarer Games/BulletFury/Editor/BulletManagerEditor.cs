using UnityEditor;
using UnityEngine;

namespace BulletFury.Editor
{
    [CustomEditor(typeof(BulletManager))]
    public class BulletManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _manualFire;
        private SerializedProperty _rotateSpeed;
        private SerializedProperty _bulletSettings;
        private SerializedProperty _spawnSettings;
        private SerializedProperty _currentActiveBullets;
        private SerializedProperty _maxActiveBullets;
        private SerializedProperty _onBulletDied;
        private SerializedProperty _onBulletSpawned;
        private SerializedProperty _onBulletCancelled;
        private SerializedProperty _onWeaponFired;
        private SerializedProperty _renderCamera;
        private SerializedProperty _seed;
        private SerializedProperty _randomiseSeedOnAwake;

        private void OnEnable()
        {
            _manualFire = serializedObject.FindProperty("manualFire");
            _rotateSpeed = serializedObject.FindProperty("rotateSpeed");
            _bulletSettings = serializedObject.FindProperty("bulletSettings");
            _renderCamera = serializedObject.FindProperty("renderCamera");
            _spawnSettings = serializedObject.FindProperty("spawnSettings");
            _currentActiveBullets = serializedObject.FindProperty("currentActiveBullets");
            _maxActiveBullets = serializedObject.FindProperty("maxActiveBullets");
            _onBulletDied = serializedObject.FindProperty("OnBulletDied");
            _onBulletSpawned = serializedObject.FindProperty("OnBulletSpawned");
            _onBulletCancelled = serializedObject.FindProperty("OnBulletCancelled");
            _onWeaponFired = serializedObject.FindProperty("OnWeaponFired");
            _seed = serializedObject.FindProperty("seed");
            _randomiseSeedOnAwake = serializedObject.FindProperty("randomiseSeedOnAwake");
        }

        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            BulletFuryEditorUtils.DrawProperty(_currentActiveBullets);
            BulletFuryEditorUtils.DrawProperty(_maxActiveBullets);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            BulletFuryEditorUtils.DrawProperty(_renderCamera);
            BulletFuryEditorUtils.DrawProperty(_manualFire);
            BulletFuryEditorUtils.DrawProperty(_rotateSpeed);

            BulletFuryEditorUtils.DrawProperty(_bulletSettings);
            BulletFuryEditorUtils.DrawProperty(_spawnSettings);
            EditorGUILayout.Space();
            BulletFuryEditorUtils.DrawProperty(_onBulletDied);            
            BulletFuryEditorUtils.DrawProperty(_onBulletSpawned);
            BulletFuryEditorUtils.DrawProperty(_onBulletCancelled);
            BulletFuryEditorUtils.DrawProperty(_onWeaponFired);
            BulletFuryEditorUtils.DrawProperty(_seed);
            BulletFuryEditorUtils.DrawProperty(_randomiseSeedOnAwake);
            
            EditorGUI.BeginDisabledGroup(((BulletManager)target).playingEditorAnimation);
            if (GUILayout.Button("Preview"))
                ((BulletManager) target).AnimateAlongPath();
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}