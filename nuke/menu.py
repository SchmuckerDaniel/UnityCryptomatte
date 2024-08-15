
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

# add icons and gizmo folder to path
nuke.pluginAddPath('./icons/')
nuke.pluginAddPath('./gizmos/')
nuke.pluginAddPath('./python/')

def addGizmos(path, menu_main=None, menu_name="Gizmos"):
	"""
	Traverses "path" folder and adds found gizmos as commands to "menu_name" menu
	"""
	m = menu_main.addMenu(menu_name)
	for root, dirs, files in os.walk(path):
		for fileNameCompl in files:
			fileName, fileExt = os.path.splitext(fileNameCompl)
			if fileExt == ".gizmo":
				folder = (root + "/")[len(path)+1:]
				m.addCommand(folder + fileName, "nuke.createNode('" + fileName + "')")


menu_nodes = nuke.menu('Nodes')
unity_crypto_toolbar = menu_nodes.addMenu("Unity Cryptomatte", icon='UnityCryptomatteIcon128.png')
unity_crypto_toolbar.addCommand("Cryptomatte Metadata Relinker", "nuke.createNode('Cryptomatte_Relink')", icon='UnityCryptomatteIcon24.png')
