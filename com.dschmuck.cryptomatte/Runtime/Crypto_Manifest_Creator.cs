
/*
-----------------------------------------------------------------------------
This source file has been developed within the scope of the
Technical Director course at Filmakademie Baden-Wuerttemberg.
http://technicaldirector.de
 
Written by Daniel Schmucker
Copyright (c) 2020 Animationsinstitut of Filmakademie Baden-Wuerttemberg
-----------------------------------------------------------------------------
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronPython.Hosting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System.IO;
using static Crypto_Python_Strings;

public class Crypto_Manifest_Creator : MonoBehaviour
{

    // Default layer = Object Layer
    public string crypto_layer_name = "crypto_object";
    public string crypto_layer_hash = "";

    // Asset Layer 
    public string crypto_asset_layer_name = "crypto_asset";
    public string crypto_asset_layer_hash = "";

    // Material Layer
    public string crypto_material_layer_name = "crypto_material";
    public string crypto_material_layer_hash = "";

    // Dropdown UI for current Render/Display Layer
    public enum Display { Beauty, ObjectCrypto, AssetCrypto, MaterialCrypto }
    public Display display;

    // checkbox indicating active preview
    public bool previewActive = false;

    // integer specifying asset hierachie lelvel (0 means toplevel, higher numbers go down in hierarchie)
    public int assetHierarchieLevel = 0;

    // option to hash only active gameobjects (default is hashing all gameobjects in the scene that have a meshrenderer)
    public bool onlyHashActive = false;

    // Render Folder Path
    public string RenderFolder = "";
    
    // manifest file name
    public string FileName = "";

    // Start is called before the first frame update
    private void Start()
    {
        if (display != Display.Beauty)
        {
            // if any of the cryptomatte layers are chosen, adjustments are made to the scene
            TurnOffPostEffects();
            OverrideShaders();
            AdjustCameraSettings();
        }
    }


    // main function called to setup cryptomatte in a scene and to export the manifest (if export is true)
    public void RunCrypto(bool export=false)
    {
        // setup ironpython (preliminary until Unity python implementation is usable)
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();
        var paths = engine.GetSearchPaths();
        paths.Add("Packages/com.dschmuck.cryptomatte/Runtime/hashing");
        paths.Add("Packages/com.unity.ironpython/Lib");
        engine.SetSearchPaths(paths);

        // Create Crypto Layer Hash x3 (object, asset and material)
        string layer_hash_string = Crypto_Python_Strings.layer_hash_string;
        string code = System.String.Format(layer_hash_string, crypto_layer_name, crypto_asset_layer_name, crypto_material_layer_name);
        var source = engine.CreateScriptSourceFromString(code);
        source.Execute(scope);
        crypto_layer_hash = scope.GetVariable<string>("crypto_layer_hash");
        crypto_asset_layer_hash = scope.GetVariable<string>("crypto_asset_layer_hash");
        crypto_material_layer_hash = scope.GetVariable<string>("crypto_material_layer_hash");

        // setting up empty manifest x3 (object, asset and material)
        // Default Object Manifest
        string crypto_manifest_string = "{";
        // Asset Manifest
        string crypto_asset_manifest_string = "{";
        //Material Manifest
        string crypto_material_manifest_string = "{";

        // Iterates ofver all Gameobject and hashes object, material and asset names
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        int index = 0;
        foreach (GameObject go in allObjects)
        {
            // NOW WE ALSO LOOP OVER THE CHILDREN to get inactive gameobjects
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {   
                if (!onlyHashActive || child.gameObject.activeInHierarchy)
                {
                    if (child.GetComponent<MeshRenderer>())
                    {

                        // CRYPTO OBJECT NAME
                        string name = child.gameObject.name;
                        // CRYPTO ASSET NAME
                        string asset_name = getAssetName(child.gameObject, assetHierarchieLevel);
                        // CRYPTO MATERIAL NAME
                        var tmpMaterial = child.gameObject.GetComponent<Renderer>().sharedMaterial;
                        // Do additional check if objects have material (reflection probes somehow act like they have a mesh renderer but they are lacking material)
                        if (tmpMaterial == null)
                        {
                            continue;
                        }
                        string material_name = tmpMaterial.name;

                        // Python based hashing
                        string code_perObject = System.String.Format(Crypto_Python_Strings.hash_object_string, name, asset_name, material_name);
                        var source_perObject = engine.CreateScriptSourceFromString(code_perObject);
                        source_perObject.Execute(scope);

                        // Default Object Crypto Values
                        float hash_float = scope.GetVariable<float>("id_value");
                        string hash_hex = scope.GetVariable<string>("id_hex");
                        float preview_color_red = scope.GetVariable<float>("preview_color_r");
                        float preview_color_green = scope.GetVariable<float>("preview_color_g");
                        float preview_color_blue = scope.GetVariable<float>("preview_color_b");
                        Color preview_color = new Color(preview_color_red, preview_color_green, preview_color_blue);

                        // Asset Crypto Values
                        float hash_float_asset = scope.GetVariable<float>("id_value_asset");
                        string hash_hex_asset = scope.GetVariable<string>("id_hex_asset");
                        float preview_color_red_asset = scope.GetVariable<float>("preview_color_r_asset");
                        float preview_color_green_asset = scope.GetVariable<float>("preview_color_g_asset");
                        float preview_color_blue_asset = scope.GetVariable<float>("preview_color_b_asset");
                        Color preview_color_asset = new Color(preview_color_red_asset, preview_color_green_asset, preview_color_blue_asset);

                        // Material Crypto Values
                        float hash_float_material = scope.GetVariable<float>("id_value_material");
                        string hash_hex_material = scope.GetVariable<string>("id_hex_material");
                        float preview_color_red_material = scope.GetVariable<float>("preview_color_r_material");
                        float preview_color_green_material = scope.GetVariable<float>("preview_color_g_material");
                        float preview_color_blue_material = scope.GetVariable<float>("preview_color_b_material");
                        Color preview_color_material = new Color(preview_color_red_material, preview_color_green_material, preview_color_blue_material);

                        // add to manifest data
                        // default object crypto
                        crypto_manifest_string += "\"" + name + "\":\"" + hash_hex + "\",";

                        // asset crypto
                        string new_asset_entry = "\"" + asset_name + "\":\"" + hash_hex_asset + "\",";  // avoid duplicates
                        if (!crypto_asset_manifest_string.Contains(new_asset_entry))
                        {
                            crypto_asset_manifest_string += new_asset_entry;
                        }

                        // material crypto
                        string new_material_entry = "\"" + material_name + "\":\"" + hash_hex_material + "\",";
                        if (!crypto_material_manifest_string.Contains(new_material_entry))
                        {
                            crypto_material_manifest_string += new_material_entry;
                        }

                        // create component on object and add data
                        CryptoHashContainer crypto_container;
                        if (child.gameObject.GetComponent<CryptoHashContainer>())
                        {
                            crypto_container = child.gameObject.GetComponent<CryptoHashContainer>();
                        }
                        else
                        {
                            crypto_container = child.gameObject.AddComponent<CryptoHashContainer>();
                        }

                        // add info for default object layer
                        crypto_container.Hex_Hash = hash_hex;
                        crypto_container.Float_Hash = hash_float;
                        crypto_container.Preview_Color = preview_color;
                        // add info for asset layer
                        crypto_container.Hex_Hash_asset = hash_hex_asset;
                        crypto_container.Float_Hash_asset = hash_float_asset;
                        crypto_container.Preview_Color_asset = preview_color_asset;
                        // add info for material layer
                        crypto_container.Hex_Hash_material = hash_hex_material;
                        crypto_container.Float_Hash_material = hash_float_material;
                        crypto_container.Preview_Color_material = preview_color_material;

                        // additionally add alphacutoff
                        // crypto_container.Alpha_Cutoff_Value = cutoff_value;

                        index++;
                    }
                }
            }
        }

        if (export)
        {
            //
            // manifests are written to disk
            //

            // remove trailing comma
            crypto_manifest_string = crypto_manifest_string.Substring(0, crypto_manifest_string.Length - 1);
            crypto_asset_manifest_string = crypto_asset_manifest_string.Substring(0, crypto_asset_manifest_string.Length - 1);
            crypto_material_manifest_string = crypto_material_manifest_string.Substring(0, crypto_material_manifest_string.Length - 1);

            // closing brackets
            crypto_manifest_string += "}";
            crypto_asset_manifest_string += "}";
            crypto_material_manifest_string += "}";

            // Manifest Export Paths
            string path = RenderFolder + "/" + FileName + "_metadata.json";
            string object_manifest_path = RenderFolder + "/" + FileName + "_cryptoObject00_manifest.json";
            string asset_manifest_path = RenderFolder + "/" + FileName + "_cryptoAsset00_manifest.json";
            string material_manifest_path = RenderFolder + "/" + FileName + "_cryptoMaterial00_manifest.json";

            // Set up Streamwriters
            StreamWriter writer = new StreamWriter(path, false);
            StreamWriter object_manifest_writer = new StreamWriter(object_manifest_path, false);
            StreamWriter asset_manifest_writer = new StreamWriter(asset_manifest_path, false);
            StreamWriter material_manifest_writer = new StreamWriter(material_manifest_path, false);

            // WRITE Default Object Crypto
            object_manifest_writer.WriteLine(crypto_manifest_string);
            // WRITE Asset Crypto Manifest
            asset_manifest_writer.WriteLine(crypto_asset_manifest_string);
            // WRITE Material Crypto Manifest
            material_manifest_writer.WriteLine(crypto_material_manifest_string);

            // WRITE METADATA JSON
            string metadata_json = "{\"" + crypto_layer_hash + "name\":\"crypto_object\",\"" + crypto_asset_layer_hash + "name\":\"crypto_asset\",\"" + crypto_material_layer_hash + "name\":\"crypto_material\",\"" + crypto_layer_hash + "manif_file\":\"" + FileName + "_cryptoObject00_manifest.json\",\"" + crypto_asset_layer_hash + "manif_file\":\"" + FileName + "_cryptoAsset00_manifest.json\",\"" + crypto_material_layer_hash + "manif_file\":\"" + FileName + "_cryptoMaterial00_manifest.json\"}";
            writer.WriteLine(metadata_json);

            // CLOSE OUT
            writer.Close();
            object_manifest_writer.Close();
            asset_manifest_writer.Close();
            material_manifest_writer.Close();
        }
    }


    public void CleanupScene(){
        // this function turns off the preview and removes any hash containers from gameobjects in the scene

        // turn off preview
        DisablePreview();
        // find all gameobjects with Component Hash Container and remove it
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            // NOW WE ALSO LOOP OVER THE CHILDREN to get inactive gameobjects
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {   
                if (child.gameObject.GetComponent<CryptoHashContainer>())
                {
                    CryptoHashContainer crypto_container;
                    crypto_container = child.gameObject.GetComponent<CryptoHashContainer>();
                    DestroyImmediate(crypto_container);
                }
            }
        }

        Debug.Log("<color=green> Success! </color> The scene was cleared from hashed values.", this);
    }


    private void TurnOffPostEffects()
    {
        // find volume and postprocessing gameobjects and disable them
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
            if ((go.GetComponent("Volume")) != null)
            {
                go.SetActive(false);
            }
    }


    private void AdjustCameraSettings()
    {
        // in order to correctly render the cryptomattes we need to adjust some things on the render camera
        // we need to get all cameras, as we are handling more shots per scene
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            // NOW WE ALSO LOOP OVER THE CHILDREN
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if ((child.GetComponent("Camera")) != null)
                {
                    Camera cam = child.GetComponent<Camera>();
                    cam.backgroundColor = Color.black;
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                    cam.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = Color.black;
                    cam.GetComponent<HDAdditionalCameraData>().volumeLayerMask = 0;
                    Debug.Log("Adjusted Camera Settings", child);
                }
            }
        }
    }


    private void adjustLightingSettings()
    {
        //deprecated
    }


    private void OverrideShaders()
    {
        // when rendering (on entering play mode) the materials will be overriden with custom unlit shaders

        // Shader Declarations and Assignments
        Shader cryptoShader;
        Shader cryptoShaderTransparent;
        cryptoShader = Shader.Find("Unlit/HDRP_CryptoColor");
        cryptoShaderTransparent = Shader.Find("Unlit/HDRP_CryptoColor_Transparent");

        // Changing shader on object basis
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            // NOW WE ALSO LOOP OVER THE CHILDREN
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.GetComponent<MeshRenderer>())
                {
                    // there can be several materials assignt to the object. need to change all
                    int range = child.GetComponent<Renderer>().materials.Length;
                    for (int m = 0; m < range; m++)
                    {
                        Material current_material = child.GetComponent<Renderer>().materials[m];
                        child.GetComponent<Renderer>().materials[m].shader = cryptoShader;
                    }
                }
            }
        }
    }


    public void EnablePreview(){
        // enables cryptomatte preview (custom volume pass)
        
        // check if enabled or just updated
        bool justActivated = true;
        if (previewActive){
            justActivated = false;
        }

        // check preview checkbox
        previewActive = true;

        // activate Custom Pass Volume
        CustomPassVolume previewPass = gameObject.GetComponent<CustomPassVolume>();
        previewPass.enabled = true;

        // update preview colors on every hash container
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
                if (go.GetComponent<CryptoHashContainer>())
                {
                    CryptoHashContainer crypto_container;
                    crypto_container = go.GetComponent<CryptoHashContainer>();
                    crypto_container.UpdatePropertyBlock();
                }
        
        // Log
        if (justActivated){
            Debug.Log("<color=green> Preview Activated! </color> Cryptomatte Preview is now activated.", this);
        }else{
            Debug.Log("<color=green> Preview Updated! </color> Updated hashes and preview layer.", this);
        }
        
    }


    public void DisablePreview(){
        // Disables the Cryptomatte preview (turns off custom volume pass)
        
        // set preview checkbox to false
        previewActive = false;

        // deactivate Custom Pass Volume
        CustomPassVolume previewPass = gameObject.GetComponent<CustomPassVolume>();
        previewPass.enabled = false;

        Debug.Log("<color=blue> Preview Disabled! </color> Turned off Cryptomatte Preview.", this);
    }


    public string getAssetName(GameObject go, int level=0){
        // returns the name of the asset based on given gameobject and asset hierarchie level
        // level 0 means top level in the hierarchie, higher numbers go deeper into the hierarchie of the object

        string asset_name = "";
        List<string> asset_name_list = new List<string>();
        bool parent_found = false;
        Transform currently_highest_hierarchy = go.transform;

        // first add current level to asset list (lowest level/object level)
        asset_name_list.Add(currently_highest_hierarchy.name);

        while (parent_found == false)
        {
            if (currently_highest_hierarchy.parent)
            {
                currently_highest_hierarchy = currently_highest_hierarchy.parent;
                asset_name_list.Add(currently_highest_hierarchy.name);
            }
            else
            {
                parent_found = true;
                asset_name = currently_highest_hierarchy.name;
            }
        }

        int asset_index = asset_name_list.Count - level -1;
        // if level higher than length of list: take index 0
        if (asset_index<0)
        {
            asset_index=0;
        }
        asset_name = asset_name_list[asset_index];
        return asset_name;
    }


    public void SetupCryptomatte(){
        // called by 'Setup Cryptomatte' Button in Inspector UI
        // RunCrypto without exporting the manifest
        RunCrypto(false);
        Debug.Log("<color=green> Cyptomatte Setup Successful! </color>", this);
    }


    public void ExportManifest(){
        // called by 'Export Manifest' Button in Inspector UI
        // RunCrypto and export the manifest
        RunCrypto(true);
        Debug.Log("<color=green> Cyptomatte Setup and Manifest Export Successful! </color>", this);
    }

}
