using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

public static class OptionsSaveSystem {
    public static void SaveSettings() {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/settings.dat";

        OptionsData data = new OptionsData();
        
        var serializer = new DataContractSerializer(typeof(OptionsData));
        var settings = new XmlWriterSettings()
        {
            Indent = true,
            IndentChars = "\t",
        };
        var writer = XmlWriter.Create(path, settings);
        serializer.WriteObject(writer, data);
        writer.Close();
    }

    public static OptionsData LoadSettings() {
        string path = Application.persistentDataPath + "/settings.dat";
        if (File.Exists(path)) {
            var fileStream = new FileStream(path, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
            var serializer = new DataContractSerializer(typeof(OptionsData));
            OptionsData serializableObject = (OptionsData)serializer.ReadObject(reader, true);
            reader.Close();
            fileStream.Close();
            return serializableObject;
            
        }else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}