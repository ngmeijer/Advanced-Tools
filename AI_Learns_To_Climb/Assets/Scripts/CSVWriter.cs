using System.Collections;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private StreamWriter _writer;
    [SerializeField] private string _fileName = "test.csv";
    private string _filePath;

    private void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, _fileName);

        if (File.Exists(_filePath))
            File.Delete(_filePath);

        _writer = new StreamWriter(_filePath, false);
        _writer.WriteLine("Episode ID, Cumulative Reward, Duration, Collectible count");
    }

    private void OnApplicationQuit()
    {
        if (_writer != null)
        {
            _writer.Close();
            _writer = null;
        }
    }

    public void AddData(int pID, float pReward, float pDuration, int pCollectibles)
    {
        if (_writer == null)
        {
            Debug.LogError("StreamWriter is not initialized.");
            return;
        }

        _writer.WriteLine(pID + "," + pReward + "," + pDuration + "," + pCollectibles);
        _writer.Flush();
    }
}
