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
    
    public OptionsData () {
        sens = OptionsMenu.sens;
        qualityIndex = OptionsMenu.qualityIndex;
    }
}
