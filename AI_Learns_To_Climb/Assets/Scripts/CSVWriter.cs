using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.IO;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    private StreamWriter _writer;
    [SerializeField] private string _fileName = "test";
    private string _filePath;

    private void Awake()
    {
        _filePath = Path.Combine(Application.dataPath, "Data",  _fileName + ".csv");

        if (File.Exists(_filePath))
            File.Delete(_filePath);

        _writer = new StreamWriter(_filePath, false);
    }

    private void OnApplicationQuit()
    {
        if (_writer != null)
        {
            _writer.Close();
            _writer = null;
        }
    }

    public void SetTestingProperties(int pMaxEpisodeCount, int pAgentHealth, int pRockCount, int pRockDamage, float pRockPunishment, int pCollectibleCount, float pCollectibleReward)
    {
        if (_writer == null)
        {
            Debug.LogError("Streamwriter is not initialized.");
            return;
        }

        _writer.WriteLine($"Max episode count: {pMaxEpisodeCount}, Agent health: {pAgentHealth}, Rock count: {pRockCount}, Rock damage: {pRockDamage}, Rock punishment: {pRockPunishment}, Collectible count: {pCollectibleCount}, Collectible reward: {pCollectibleReward}");
        _writer.WriteLine("-------");
        _writer.WriteLine("Cumulative reward, Duration, Rock hit count, Collectibles found, Health potions found");
    }

    public void AddData(float pReward, float pDuration, int pRockHitCount, int pCollectibles, int pHealthPotions)
    {
        if (_writer == null)
        {
            Debug.LogError("Streamwriter is not initialized.");
            return;
        }

        _writer.WriteLine(pReward + "," + pDuration + "," + pRockHitCount + "," + pCollectibles + "," + pHealthPotions);
        _writer.Flush();
    }
}
