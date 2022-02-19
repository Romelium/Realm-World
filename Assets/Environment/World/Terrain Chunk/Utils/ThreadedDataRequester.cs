using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Modified from https://github.com/SebLague/Procedural-Landmass-Generation/blob/master/Proc%20Gen%20E21/Assets/Scripts/ThreadedDataRequester.cs
public class ThreadedDataRequester : MonoBehaviour
{
    public static ThreadedDataRequester global;
    readonly Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();
    readonly Queue<Action> requestQueue = new Queue<Action>();
    public int maxQueue = int.MaxValue;
    private int queueCount = 0;

    [RuntimeInitializeOnLoadMethod] //run method without gameObject
    static void CreateInstance()
    {
        var Object = new GameObject("ThreadedDataRequester");
        global = Object.AddComponent<ThreadedDataRequester>();
        global.maxQueue = SystemInfo.processorCount;
        DontDestroyOnLoad(Object);
    }

    public static void RequestData(Func<object> generateData, Action<object> callback, TaskCreationOptions options = TaskCreationOptions.None)
    {
        void request() => Task.Factory.StartNew(() => global.DataThread(generateData, callback), options);
        if (global.queueCount < global.maxQueue)
        {
            global.queueCount++;
            request();
        }
        else
        {
            global.requestQueue.Enqueue(request);
        }
    }
    public static void RequestData(Func<object> generateData, Action<object> callback, TaskScheduler taskScheduler, TaskCreationOptions options = TaskCreationOptions.None)
    {
        void request() => Task.Factory.StartNew(() => global.DataThread(generateData, callback), CancellationToken.None, options, taskScheduler);
        if (global.queueCount < global.maxQueue)
        {
            global.queueCount++;
            request();
        }
        else
        {
            global.requestQueue.Enqueue(request);
        }
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }


    void Update()
    {
        for (int i = 0; i < dataQueue.Count; i++)
        {
            ThreadInfo threadInfo = dataQueue.Dequeue();
            threadInfo.callback(threadInfo.parameter);
        }
        for (queueCount = 0; queueCount < maxQueue && requestQueue.Count > 0; queueCount++)
        {
            requestQueue.Dequeue().Invoke();
        }
    }

    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }
}
