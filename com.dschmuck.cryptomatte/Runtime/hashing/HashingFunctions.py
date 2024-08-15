
# 
# Copyright (c) 2014, 2015, 2016, 2017 Psyop Media Company, LLC 
# All rights reserved.
# 
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#     * Redistributions of source code must retain the above copyright
#       notice, this list of conditions and the following disclaimer.
#     * Redistributions in binary form must reproduce the above copyright
#       notice, this list of conditions and the following disclaimer in the
#       documentation and/or other materials provided with the distribution.
#     * Neither the name of the <organization> nor the
#       names of its contributors may be used to endorse or promote products
#       derived from this software without specific prior written permission.
# 
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
# DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
# DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
# ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
# 

# -----------------------------------------------------------------------------
# Some of the functions in this file were copied from the Nuke Cryptomatte implementation
# By using the same function it guaranteed compatibility of the Unity Cryptomatte 
# Renderings and the Nuke Cryptomatte Gizmo.
# The original code can be found here:
# https://github.com/Psyop/Cryptomatte/blob/master/nuke/cryptomatte_utilities.py
# 
# Additional functions have been developed within the scope of the
# Technical Director course at Filmakademie Baden-Wuerttemberg.
# http://technicaldirector.de
#
# -----------------------------------------------------------------------------

import pymmh3 as mmh3
import sys
import struct
import math
 
CRYPTO_METADATA_LEGAL_PREFIX = ["exr/cryptomatte/", "cryptomatte/"]
CRYPTO_METADATA_DEFAULT_PREFIX = CRYPTO_METADATA_LEGAL_PREFIX[1]
 
def mm3hash_float(name):
    hash_32 = mmh3.hash(name)
    print "Raw Hash = {}".format(hash_32)
    exp = hash_32 >> 23 & 255 
    if (exp == 0) or (exp == 255):
        hash_32 ^= 1 << 23 
 
    print "2nd Hash = {}".format(hash_32)
 
    packed = struct.pack('<L', hash_32 & 0xffffffff)
    return struct.unpack('<f', packed)[0]
 
def id_to_rgb(id):
    # This takes the hashed id and converts it to a preview color
    import ctypes
    bits = ctypes.cast(ctypes.pointer(ctypes.c_float(id)), ctypes.POINTER(ctypes.c_uint32)).contents.value
 
    mask = 2 ** 32 - 1
    return [0.0, float((bits << 8) & mask) / float(mask), float((bits << 16) & mask) / float(mask)]

def mimic_nuke_preview_color(id):
    # Adapted Nuke expression to generate same preview colors as in Nuke
    red = (float)((math.frexp(abs(id))[0] * 1 % 0.25))
    green = (float)((math.frexp(abs(id))[0] * 16 % 0.25))
    blue = (float)((math.frexp(abs(id))[0] * 64 % 0.25))
    return [red, green, blue]
 
def id_to_hex(id):
    return "{0:08x}".format(struct.unpack('<I', struct.pack('<f', id))[0])
 
def layer_hash(layer_name):
    return id_to_hex(mm3hash_float(layer_name))[:-1]
 
 
if __name__ == "__main__":
    # test functions to test compatibility of hashing
    matte_name = "Testobject"
    id_value = mm3hash_float(matte_name)
    id_hex = id_to_hex(id_value)
    preview_color = id_to_rgb(id_value)
    
    print "Matte Name: {}, ID value: {}, ID-Hex: {}, Preview Color: {}".format(
        matte_name, id_value, id_hex, preview_color)