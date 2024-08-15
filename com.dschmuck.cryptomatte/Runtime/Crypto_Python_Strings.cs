
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

public class Crypto_Python_Strings : MonoBehaviour
{

// collection of python commands in strings needed to run python in unity with ironpython
// this is just the preliminary solution until Unity's internal python functionality is feasible

public static string layer_hash_string = @"
import HashingFunctions as mmh3tp
crypto_layer_hash = mmh3tp.CRYPTO_METADATA_DEFAULT_PREFIX + mmh3tp.layer_hash('{0}') + '/'
crypto_asset_layer_hash = mmh3tp.CRYPTO_METADATA_DEFAULT_PREFIX + mmh3tp.layer_hash('{1}') + '/'
crypto_material_layer_hash = mmh3tp.CRYPTO_METADATA_DEFAULT_PREFIX + mmh3tp.layer_hash('{2}') + '/'
";

public static string hash_object_string = @"
import HashingFunctions as mmh3tp

# object
matte_name = '{0}'
id_value = mmh3tp.mm3hash_float(matte_name)
id_hex = mmh3tp.id_to_hex(id_value)
preview_color = mmh3tp.mimic_nuke_preview_color(id_value)
preview_color_r = preview_color[0]
preview_color_g = preview_color[1]
preview_color_b = preview_color[2]

# asset
matte_name = '{1}'
id_value_asset = mmh3tp.mm3hash_float(matte_name)
id_hex_asset = mmh3tp.id_to_hex(id_value_asset)
preview_color_asset = mmh3tp.mimic_nuke_preview_color(id_value_asset)
preview_color_r_asset = preview_color_asset[0]
preview_color_g_asset = preview_color_asset[1]
preview_color_b_asset = preview_color_asset[2]

# material
matte_name = '{2}'
id_value_material = mmh3tp.mm3hash_float(matte_name)
id_hex_material = mmh3tp.id_to_hex(id_value_material)
preview_color_material = mmh3tp.mimic_nuke_preview_color(id_value_material)
preview_color_r_material = preview_color_material[0]
preview_color_g_material = preview_color_material[1]
preview_color_b_material = preview_color_material[2]
";

}
