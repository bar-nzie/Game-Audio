using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SaveManager : MonoBehaviour
{
    private string savePath;
    private static readonly byte[] key = Encoding.UTF8.GetBytes("dfjwocurjsorngos[wlf'3'r[c0rwid1"); // must be 32 bytes
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("shfbris0w[2=]'/."); // must be 16 bytes

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savefile.dat"); // use .dat instead of .json
    }

    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        byte[] encrypted = EncryptStringToBytes(json, key, iv);
        File.WriteAllBytes(savePath, encrypted);
        Debug.Log("Encrypted game saved to: " + savePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                byte[] encrypted = File.ReadAllBytes(savePath);
                string json = DecryptStringFromBytes(encrypted, key, iv);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Game loaded and decrypted.");
                return data;
            }
            catch
            {
                Debug.LogError("Failed to decrypt save file.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No save file found");
            return null;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");
        }
    }

    // AES encryption helpers
    private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
                swEncrypt.Close();
                return msEncrypt.ToArray();
            }
        }
    }

    private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
