using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
//using System.Runtime.Serialization.Formatters.Binary;
//using System;

namespace Player {
    public static class SaveSystem {
        /*public static void SavePlayer(Player.PlayerMovement player) {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.dat";
            FileStream stream = new FileStream(path, FileMode.Create);

            PlayerData data = new PlayerData(player);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        */
        /*public static PlayerData LoadPlayer() {
            string path = Application.persistentDataPath + "/player.dat";
            if (File.Exists(path)) {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                
                

                PlayerData data = formatter.Deserialize(stream) as PlayerData;
                stream.Close();

                return data;
                
            }else {
                Debug.LogError("Save file not found in " + path);
                return null;
            }
        }*/

        public static void SavePlayer(Player.PlayerMovement player) {
            Debug.Log(Application.persistentDataPath);
            string path = Application.persistentDataPath + "/player.dat";

            PlayerData data = new PlayerData(player);
            
            var serializer = new DataContractSerializer(typeof(PlayerData));
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
            };
            var writer = XmlWriter.Create(path, settings);
            serializer.WriteObject(writer, data);
            writer.Close();
        }

        public static PlayerData LoadPlayer() {
            string path = Application.persistentDataPath + "/player.dat";
            if (File.Exists(path)) {
                var fileStream = new FileStream(path, FileMode.Open);
                var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
                var serializer = new DataContractSerializer(typeof(PlayerData));
                PlayerData serializableObject = (PlayerData)serializer.ReadObject(reader, true);
                reader.Close();
                fileStream.Close();
                return serializableObject;
                
            }else {
                Debug.LogError("Save file not found in " + path);
                return null;
            }
        }
    }
}