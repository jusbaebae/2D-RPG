using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemSO))]
public class ItemSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //기본 필드
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemDescription"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemCategory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemType"));

        EditorGUILayout.Space();

        //타입 가져오기
        ItemSO item = (ItemSO)target;

        //장비별 스프라이트분리
        switch (item.itemType)
        {
            case ItemType.armor:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armorSprites"), true);
                break;

            case ItemType.bottom:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomSprites"), true);
                break;
        }

        EditorGUILayout.Space();

        //나머지 공통 필드
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isGold"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stackSize"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentHealth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));

        serializedObject.ApplyModifiedProperties();
    }
}
