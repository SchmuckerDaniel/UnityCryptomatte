
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
using UnityEngine.UI;

public class CryptoHashContainer : MonoBehaviour
{

    // Material Property Block
    private static MaterialPropertyBlock propertyBlock;

    // Crypto Hashing Shader Property IDs
    static int colorID = Shader.PropertyToID("_Color"); // Optimization
    static int colorCryptoID = Shader.PropertyToID("_ColorCrypto"); // additional Color for Crypto
    static int hashFloatCryptoID = Shader.PropertyToID("_HashFloatCrypto"); // additional hash float for Crypto

    // additional Property ID for alphaCutoffValue
    static int alphaCutoffValueID = Shader.PropertyToID("_AlphaCutoffValue");

    // expose the hashing properties
    // default Object Crypto
    public string Hex_Hash = "";
    public float Float_Hash = 0.0f;
    public Color Preview_Color = Color.white;
    // asset Crypto
    public string Hex_Hash_asset = "";
    public float Float_Hash_asset = 0.0f;
    public Color Preview_Color_asset = Color.white;
    // material Crypto
    public string Hex_Hash_material = "";
    public float Float_Hash_material = 0.0f;
    public Color Preview_Color_material = Color.white;
    //expose the alpha cutoff value
    public float Alpha_Cutoff_Value = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        // set some default color for color
        Color color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        // get dropdown value from CryptomatteManager to know which value to set
        GameObject crypto_manager = GameObject.Find("CryptomatteManager");
        // Object Cryptomatte - set hashed float and preview color
        if (crypto_manager.GetComponent<Crypto_Manifest_Creator>().display == Crypto_Manifest_Creator.Display.ObjectCrypto)
        {
            propertyBlock.SetFloat(hashFloatCryptoID, Float_Hash);
            propertyBlock.SetColor(colorCryptoID, Preview_Color);
        }
        // Asset Cryptomatte - set hashed float and preview color
        if (crypto_manager.GetComponent<Crypto_Manifest_Creator>().display == Crypto_Manifest_Creator.Display.AssetCrypto)
        {
            propertyBlock.SetFloat(hashFloatCryptoID, Float_Hash_asset);
            propertyBlock.SetColor(colorCryptoID, Preview_Color_asset);
        }
        // Material Cryptomatte - set hashed float and preview color
        if (crypto_manager.GetComponent<Crypto_Manifest_Creator>().display == Crypto_Manifest_Creator.Display.MaterialCrypto)
        {
            propertyBlock.SetFloat(hashFloatCryptoID, Float_Hash_material);
            propertyBlock.SetColor(colorCryptoID, Preview_Color_material);
        }

        // set property block for alpha cutoff value
        propertyBlock.SetFloat(alphaCutoffValueID, Alpha_Cutoff_Value);

        // set finished property block
        GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }


    public void UpdatePropertyBlock(){
        // Called to Update Cryptomatte preview (when changing Layer, etc.)
        Start();
    }

}
