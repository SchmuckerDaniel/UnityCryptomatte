
/*
-----------------------------------------------------------------------------
This source file has been developed within the scope of the
Technical Director course at Filmakademie Baden-Wuerttemberg.
http://technicaldirector.de
 
Written by Daniel Schmucker
Copyright (c) 2020 Animationsinstitut of Filmakademie Baden-Wuerttemberg
-----------------------------------------------------------------------------
*/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Crypto_Manifest_Creator))]
public class CryptoHashEditor : Editor
{
    // unity cryptomatte icon
    private Texture icon;

    // global layouy width: adjust to make fields wider or smaller
    private float labelWidth = 160;

    // used for the greyed out fields
    private bool disabledField = true;

    // take dropdown menu directly from manifest creator script
    SerializedProperty display;

    public void OnEnable(){
        // load header image
        icon = (Texture)Resources.Load("UnityManagerHeader");
        // link dropdown menu called display
        display = serializedObject.FindProperty("display");
    }

    public override void OnInspectorGUI()
    {
        Crypto_Manifest_Creator CryptomatteManager = (Crypto_Manifest_Creator)target;

        // Icon
        GUILayout.Label(icon);
        
        // start UI
        GUILayout.BeginVertical();
        GUILayout.Space(10);



        //
        // SETUP CRYPTOMATTE
        //

        // Header
        EditorGUILayout.LabelField("SETUP", EditorStyles.boldLabel);

        // Labels
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Cryptomatte Layer",EditorStyles.boldLabel, GUILayout.MaxWidth(labelWidth));
        EditorGUILayout.LabelField("name",GUILayout.MaxWidth(labelWidth));
        EditorGUILayout.LabelField("hash",GUILayout.MaxWidth(labelWidth));
        GUILayout.EndHorizontal();

        // Object Cryptomatte
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object Cryptomatte", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.crypto_layer_name = EditorGUILayout.TextField(CryptomatteManager.crypto_layer_name, GUILayout.MaxWidth(labelWidth));
        EditorGUI.BeginDisabledGroup(disabledField==true);
        CryptomatteManager.crypto_layer_hash = EditorGUILayout.TextField(CryptomatteManager.crypto_layer_hash, GUILayout.MaxWidth(labelWidth));
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        // Material Cryptomatte
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Material Cryptomatte", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.crypto_material_layer_name = EditorGUILayout.TextField(CryptomatteManager.crypto_material_layer_name, GUILayout.MaxWidth(labelWidth));
        EditorGUI.BeginDisabledGroup(disabledField==true);
        CryptomatteManager.crypto_material_layer_hash = EditorGUILayout.TextField(CryptomatteManager.crypto_material_layer_hash, GUILayout.MaxWidth(labelWidth));
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        // Asset Cryptomatte
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset Cryptomatte", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.crypto_asset_layer_name = EditorGUILayout.TextField(CryptomatteManager.crypto_asset_layer_name, GUILayout.MaxWidth(labelWidth));
        EditorGUI.BeginDisabledGroup(disabledField==true);
        CryptomatteManager.crypto_asset_layer_hash = EditorGUILayout.TextField(CryptomatteManager.crypto_asset_layer_hash, GUILayout.MaxWidth(labelWidth));
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        // add bit of space btween layers and other options
        GUILayout.Space(7);

        // Asset Hierarchie Level
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Asset Hierarchie Level", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.assetHierarchieLevel = EditorGUILayout.IntField(CryptomatteManager.assetHierarchieLevel, GUILayout.MaxWidth(labelWidth));
        GUILayout.EndHorizontal();

        // Only Hash Active Gameobjects Checkbox
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Hash only active Objects", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.onlyHashActive = EditorGUILayout.Toggle(CryptomatteManager.onlyHashActive, GUILayout.MaxWidth(labelWidth));
        GUILayout.EndHorizontal();

        // Setup Cryptomatte BUTTON
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Setup Cryptomatte", GUILayout.MaxWidth(labelWidth*3/2+3)))
        {
            CryptomatteManager.SetupCryptomatte();
        }

        // Cleanup Scene BUTTON
        if (GUILayout.Button("Cleanup Scene", GUILayout.MaxWidth(labelWidth*3/2+3)))
        {
            CryptomatteManager.CleanupScene();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();



        //
        // RENDER OPTIONS AND MANIFEST EXPORT
        //
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        // HEADER
        EditorGUILayout.LabelField("RENDER OPTIONS & MANIFEST EXPORT", EditorStyles.boldLabel);

        // active cryptomatte
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Active (Cryptomatte) Layer", GUILayout.MaxWidth(labelWidth));
        EditorGUILayout.PropertyField(display, GUIContent.none, GUILayout.MaxWidth(labelWidth*2+5));
        GUILayout.EndHorizontal();

        // Render Folder Text Field
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Render Folder", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.RenderFolder = EditorGUILayout.TextField(CryptomatteManager.RenderFolder, GUILayout.MaxWidth(labelWidth*2+5));
        GUILayout.EndHorizontal();

        // Manifest Name Text Field
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Manifest Name", GUILayout.MaxWidth(labelWidth));
        CryptomatteManager.FileName = EditorGUILayout.TextField(CryptomatteManager.FileName, GUILayout.MaxWidth(labelWidth*2+5));
        GUILayout.EndHorizontal();

        // Export Manifest BUTTON
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(labelWidth));
        if (GUILayout.Button("Export Manifest", GUILayout.MaxWidth(labelWidth*2+5)))
        {
            CryptomatteManager.ExportManifest();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30);



        //
        // PREVIEW
        //

        // HEADER 
        EditorGUILayout.LabelField("CRYPTOMATTE PREVIEW", EditorStyles.boldLabel);

        // Preview Active Checkbox
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Preview Active", GUILayout.MaxWidth(labelWidth));
        EditorGUI.BeginDisabledGroup(disabledField==true);
        CryptomatteManager.previewActive = EditorGUILayout.Toggle(CryptomatteManager.previewActive, GUILayout.MaxWidth(labelWidth));
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        // Preview BUTTONS
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Cryptomatte Preview", GUILayout.MaxWidth(labelWidth));
        if (GUILayout.Button("Enable/Update Preview", GUILayout.MaxWidth(labelWidth)))
        {
            CryptomatteManager.EnablePreview();
        }
        if (GUILayout.Button("Disable Preview",GUILayout.MaxWidth(labelWidth)))
        {
            CryptomatteManager.DisablePreview();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(30);

        
        // Apply Adjustments on serialized objects (dropdown menu)
        serializedObject.ApplyModifiedProperties();

    }
}
