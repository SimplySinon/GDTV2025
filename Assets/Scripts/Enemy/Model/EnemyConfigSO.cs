using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "GDTV2025/Enemy/EnemyConfigSO", fileName = "EnemyTypeConfig")]
public class EnemyConfigSO : ScriptableObject
{
    /*
        IMPORTANT:
        Script has Editor Script below
    */
    public string ConfigName;
    public float MoveSpeed;
    public float ChargeSpeed;
    public float WanderSpeed;
    public float FollowSpeed;
    public Vector2 RelativeOffset;
    public float ChargeTime;
    public float ChargeCooldown;
    public float ProximityRange;
    public float BufferMinDistance = 1.5f;
    public float BufferMaxDistance = 3f;

    public float MaxHealth;
    public float MaxDamage;
    public float MinDamage;
    public float ValueDiviation;
    public EnemyMovementAIType MovementAIType;

}

[CustomEditor(typeof(EnemyConfigSO))]
public class EnemyConfigSOEditor : Editor
{

    SerializedProperty configName;
    SerializedProperty maxHealth;
    SerializedProperty maxDamage;
    SerializedProperty minDamage;
    SerializedProperty movementAIType;
    SerializedProperty moveSpeed;
    SerializedProperty chargeSpeed;
    SerializedProperty wanderSpeed;
    SerializedProperty followSpeed;
    SerializedProperty relativeOffset;
    SerializedProperty chargeTime;
    SerializedProperty chargeCooldown;
    SerializedProperty proximityRange;
    SerializedProperty bufferMinDistance;
    SerializedProperty bufferMaxDistance;
    SerializedProperty valueDiviation;

    void OnEnable()
    {
        configName = serializedObject.FindProperty("ConfigName");
        maxHealth = serializedObject.FindProperty("MaxHealth");
        maxDamage = serializedObject.FindProperty("MaxDamage");
        minDamage = serializedObject.FindProperty("MinDamage");
        moveSpeed = serializedObject.FindProperty("MoveSpeed");
        chargeSpeed = serializedObject.FindProperty("ChargeSpeed");
        wanderSpeed = serializedObject.FindProperty("WanderSpeed");
        followSpeed = serializedObject.FindProperty("FollowSpeed");
        relativeOffset = serializedObject.FindProperty("RelativeOffset");
        chargeTime = serializedObject.FindProperty("ChargeTime");
        chargeCooldown = serializedObject.FindProperty("ChargeCooldown");
        proximityRange = serializedObject.FindProperty("ProximityRange");
        bufferMinDistance = serializedObject.FindProperty("BufferMinDistance");
        bufferMaxDistance = serializedObject.FindProperty("BufferMaxDistance");
        movementAIType = serializedObject.FindProperty("MovementAIType");
        valueDiviation = serializedObject.FindProperty("ValueDiviation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Config Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(configName);
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUILayout.PropertyField(maxDamage);
        EditorGUILayout.PropertyField(minDamage);
        EditorGUILayout.PropertyField(valueDiviation);
        EditorGUILayout.Space(1);
        EditorGUILayout.PropertyField(movementAIType);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Movement Settings", EditorStyles.boldLabel);

        EnemyMovementAIType aiType = (EnemyMovementAIType)movementAIType.enumValueIndex;

        switch (aiType)
        {
            case EnemyMovementAIType.Charge:
                EditorGUILayout.PropertyField(chargeSpeed);
                EditorGUILayout.PropertyField(chargeTime);
                EditorGUILayout.PropertyField(chargeCooldown);
                break;

            case EnemyMovementAIType.FollowWithBuffer:
                EditorGUILayout.PropertyField(followSpeed);
                EditorGUILayout.PropertyField(bufferMinDistance);
                EditorGUILayout.PropertyField(bufferMaxDistance);
                break;

            case EnemyMovementAIType.KeepRelativeOffset:
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(relativeOffset);
                break;

            case EnemyMovementAIType.WanderAndCharge:
                EditorGUILayout.PropertyField(wanderSpeed);
                EditorGUILayout.PropertyField(proximityRange);
                EditorGUILayout.PropertyField(chargeSpeed);
                break;

            case EnemyMovementAIType.RandomPatrol:
                EditorGUILayout.PropertyField(moveSpeed);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

}