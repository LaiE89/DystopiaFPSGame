using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;

[DataContract]
public class OptionsData {

    [DataMember]
    public float sens;
    [DataMember]
    public int qualityIndex;
    [DataMember]
    public float volume;
    [DataMember]
    public int resolutionIndex;
    [DataMember]
    public bool isFullscreen;
    
    public OptionsData () {
        sens = OptionsMenu.sens;
        qualityIndex = OptionsMenu.qualityIndex;
        volume = OptionsMenu.volume;
        resolutionIndex = OptionsMenu.resolutionIndex;
        isFullscreen = OptionsMenu.isFullscreen;
    }
}
