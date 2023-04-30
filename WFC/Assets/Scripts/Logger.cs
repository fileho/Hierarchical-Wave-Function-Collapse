using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes a data to a file
/// </summary>
public class Logger : MonoBehaviour
{
    private const string path = "/text.txt";

    public static void Write(string data)
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + path, true);
        writer.Write(data);
        writer.Close();
    }
}
