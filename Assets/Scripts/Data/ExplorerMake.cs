using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorerMake
{
    public void MakeNewFile(string dataPath)
    {
        if (!File.Exists(dataPath))
        {
            File.Create(dataPath);
        }
    }

    public void MakeNewDirectory(string dataPath)
    {
        if(!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }
    }
}
