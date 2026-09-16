using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using SysRandom = System.Random;

public class WordBuffer : MonoBehaviour
{
    [Header("Buffer Config")] [SerializeField]
    private int numBuffers;

    [SerializeField] private int stringsPerBuffer;
    
    // The word lists
    private string[] baseWords;
    private int baseWordCount;
    private string[] addedWords;
    
    // The buffers
    private string[,] internalBuffers;
    private bool[] buffersReady;
    private int activeBuffer = 0;
    private int indexInBuffer = 0;

    private Task[] bufferFillTasks;

    // I don't know if System.Random is necessarily thread safe so I'm using one random per buffer to be safe
    private SysRandom[] bufferRandomizers;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        Debug.Log("Reading file asynchronously");

        bool success = await ReadWordsAsync(Path.Combine(Application.streamingAssetsPath, "en.txt"));
        
        Debug.Log("Read files successfully");

        internalBuffers = new string[numBuffers, stringsPerBuffer];
        buffersReady = new bool[numBuffers];
        bufferRandomizers = new SysRandom[numBuffers];
        bufferFillTasks = new Task[numBuffers];

        for (int i = 0; i < numBuffers; i++)
        {
            // Ensure random seed? I'm sure it's not that deep tbh
            bufferRandomizers[i] = new SysRandom(UnityRandom.Range(int.MinValue, int.MaxValue));
        }
        
        for (int i = 0; i < stringsPerBuffer; i++)
        {
            int randomIdx = UnityRandom.Range(0, baseWordCount);
            internalBuffers[activeBuffer, i] = baseWords[randomIdx];
        }

        buffersReady[activeBuffer] = true;
        CheckBuffers();
        
        Debug.Log("Loaded some words");
    }

    private async Task<bool> ReadWordsAsync(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File of words not found: " + path);
            return false;
        }
        
        string content = File.ReadAllText(path);
        baseWords = content.Split(new string[] {"\n", "\r", "\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        baseWordCount = baseWords.Length;

        return true;

    }

    private async Task FillBuffer(int bufferIndex)
    {
        for (int i = 0; i < stringsPerBuffer; i++)
        {
            int randomIdx = bufferRandomizers[bufferIndex].Next(0, baseWordCount);
            internalBuffers[activeBuffer, i] = baseWords[randomIdx];
        }

        buffersReady[bufferIndex] = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetNextString()
    {
        string word = internalBuffers[activeBuffer, indexInBuffer++];
        Debug.Log($"Word is {word}");
        if (indexInBuffer >= stringsPerBuffer)
        {
            indexInBuffer = 0;
            buffersReady[activeBuffer++] = false;
            if (activeBuffer > numBuffers) activeBuffer = 0;
            CheckBuffers();
        }

        return word;
    }

    public void CheckBuffers()
    {
        for (int i = 0; i < numBuffers; i++)
        {
            Debug.Log($"Buffer {i} is {bufferFillTasks[i]} and status is {buffersReady[i]}");
            if (i == activeBuffer || (!(bufferFillTasks[i] is null) && !bufferFillTasks[i].Status.Equals(TaskStatus.RanToCompletion))) continue;
            // This will probably run into a race condition
            if (!buffersReady[i])
            {
                Debug.Log($"Filling buffer {i}");
                bufferFillTasks[i] = Task.Run(() => FillBuffer(i));
            }
        }
    }
}
