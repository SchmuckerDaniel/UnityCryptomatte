
# -----------------------------------------------------------------------------
# This source file has been developed within the scope of the
# Technical Director course at Filmakademie Baden-Wuerttemberg.
# http://technicaldirector.de
#
# Written by Daniel Schmucker
# Copyright (c) 2020 Animationsinstitut of Filmakademie Baden-Wuerttemberg
# -----------------------------------------------------------------------------

import os
import nuke
import json

def relink_exr_and_meta():
    input_path = nuke.thisNode().input(0)["file"].getValue()
    dir_path = os.path.dirname(input_path)
    file_name = os.path.basename(input_path)
    print dir_path, file_name
    object_file_path = dir_path + "/" + file_name.split("_")[0]+"_cryptoObject00.exr"
    asset_file_path = dir_path + "/" + file_name.split("_")[0]+"_cryptoAsset00.exr"
    material_file_path = dir_path + "/" + file_name.split("_")[0]+"_cryptoMaterial00.exr"

    # read json
    metadata_file_path = dir_path + "/" + file_name.split("_")[0]+"_metadata.json"


    with open(metadata_file_path) as json_file:  
        data = json.load(json_file)

    with nuke.thisNode():
        nuke.toNode("Read_CryptoObject")["file"].setValue(object_file_path)
        nuke.toNode("Read_CryptoAsset")["file"].setValue(asset_file_path)
        nuke.toNode("Read_CryptoMaterial")["file"].setValue(material_file_path)

        meta_data_string = ""
        for meta in data:
            meta_data_string += "{set %s %s}\n" % (meta, data[meta])
        
        nuke.toNode("ModifyMetaData")['metadata'].fromScript(meta_data_string)

def relink_metadata():
    metadata_file_path = nuke.thisNode()["metadata_file"].getValue()
    # read JSON file
    with open(metadata_file_path) as json_file:
        data = json.load(json_file)
    object_metadata_string = ""
    material_metadata_string = ""
    asset_metadata_string = ""

    for entry in data:
        if "28322e9" in entry:
            asset_metadata_string += "{set %s %s}\n" % (entry, data[entry])
        elif "bda530a" in entry:
            material_metadata_string += "{set %s %s}\n" % (entry, data[entry])
        else:
            object_metadata_string += "{set %s %s}\n" % (entry, data[entry])
    
    # edit metadatanode inside this gizmo/group
    with nuke.thisNode():
        nuke.toNode("ModifyMetaData_Object")['metadata'].fromScript(object_metadata_string)
        nuke.toNode("ModifyMetaData_Asset")['metadata'].fromScript(asset_metadata_string)
        nuke.toNode("ModifyMetaData_Material")['metadata'].fromScript(material_metadata_string)
