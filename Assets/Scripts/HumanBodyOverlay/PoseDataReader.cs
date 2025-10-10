//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using UnityEngine;

//public class PoseDataReader : MonoBehaviour
//{
//    [Header("File Settings")]
//    public string jsonFilePath = @"C:\Users\hp\pose_data.json"; // Use consistent path

//    [Header("Tracking Settings")]
//    public Transform targetObject;
//    public string jointToTrack = "LEFT_SHOULDER";
//    public float updateInterval = 0.1f; // In seconds

//    [Header("Position Settings")]
//    public float positionScale = 1f;
//    public Vector3 positionOffset = Vector3.zero;

//    private float timer;
//    private string lastFileContent = "";
//    private System.DateTime lastFileWriteTime;

//    void Start()
//    {
//        if (updateInterval <= 0f)
//            updateInterval = 0.1f;

//        // Validate file path
//        if (!File.Exists(jsonFilePath))
//        {
//            Debug.LogError("JSON file does not exist at: " + jsonFilePath);
//            return;
//        }

//        // Start reading pose data
//        InvokeRepeating("ReadAndApplyPose", 0f, updateInterval);
//    }

//    void ReadAndApplyPose()
//    {
//        if (!File.Exists(jsonFilePath))
//        {
//            Debug.LogWarning("JSON file does not exist at: " + jsonFilePath);
//            return;
//        }

//        try
//        {
//            // Check if file has been modified
//            var currentWriteTime = File.GetLastWriteTime(jsonFilePath);
//            if (currentWriteTime == lastFileWriteTime)
//            {
//                return; // File hasn't been updated
//            }
//            lastFileWriteTime = currentWriteTime;

//            // Read file with proper file sharing to avoid locking issues
//            string json;
//            using (var fileStream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//            using (var reader = new StreamReader(fileStream))
//            {
//                json = reader.ReadToEnd();
//            }

//            // Skip if content hasn't changed
//            if (json == lastFileContent)
//                return;

//            lastFileContent = json;

//            // Parse JSON
//            var data = JsonConvert.DeserializeObject<Dictionary<string, JointData>>(json);

//            if (data != null && data.ContainsKey(jointToTrack))
//            {
//                var joint = data[jointToTrack];

//                // Apply position with scaling and offset
//                Vector3 pos = new Vector3(
//                    (joint.x - 0.5f) * positionScale + positionOffset.x,
//                    (1f - joint.y) * positionScale + positionOffset.y,
//                    -joint.z * positionScale + positionOffset.z
//                );

//                Debug.Log($"Updating position for {jointToTrack}: {pos}");
//                targetObject.localPosition = pos;
//            }
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("Error reading pose data: " + e.Message);
//        }
//    }

//    void OnDestroy()
//    {
//        CancelInvoke("ReadAndApplyPose");
//    }

//    [System.Serializable]
//    public class JointData
//    {
//        public float x;
//        public float y;
//        public float z;
//        public float visibility;
//    }
//}





using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

public class UDPPoseReceiver : MonoBehaviour
{
    [Header("UDP Configuration")]
    public int udpPort = 5005;
    public string targetJoint = "LEFT_SHOULDER";

    [Header("Transform Settings")]
    public Transform targetObject;
    public float positionScale = 5.0f;
    public Vector3 positionOffset = new Vector3(-0.5f, 1.0f, 0f);

    [Header("Smoothing")]
    public bool enableSmoothing = true;
    public float smoothingFactor = 0.1f;

    // UDP Components
    private UdpClient udpClient;
    private Thread udpThread;
    private volatile bool isReceiving = false;

    // Thread-safe data exchange
    private ConcurrentQueue<Dictionary<string, JointData>> dataQueue;
    private Dictionary<string, JointData> latestPoseData;

    // Position tracking
    private Vector3 targetPosition;
    private Vector3 smoothedPosition;

    // Debug info (main thread only)
    [Header("Debug")]
    public bool showDebugInfo = true;
    private float lastUpdateTime;
    private volatile int packetsReceived = 0;

    void Start()
    {
        dataQueue = new ConcurrentQueue<Dictionary<string, JointData>>();
        InitializeUDP();

        if (targetObject == null)
            targetObject = transform;

        targetPosition = targetObject.localPosition;
        smoothedPosition = targetPosition;
        lastUpdateTime = Time.time; // Safe to call in main thread
    }

    void InitializeUDP()
    {
        try
        {
            udpClient = new UdpClient(udpPort);
            udpThread = new Thread(ReceiveUDPData);
            udpThread.IsBackground = true;
            udpThread.Start();
            isReceiving = true;

            Debug.Log($"UDP Pose Receiver started on port {udpPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize UDP: {e.Message}");
        }
    }

    void ReceiveUDPData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (isReceiving)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string jsonString = Encoding.UTF8.GetString(data);

                var poseData = JsonConvert.DeserializeObject<Dictionary<string, JointData>>(jsonString);

                // Thread-safe data queuing
                dataQueue.Enqueue(poseData);
                packetsReceived++;

                // Prevent queue overflow
                while (dataQueue.Count > 5)
                {
                    dataQueue.TryDequeue(out _);
                }
            }
            catch (Exception e)
            {
                if (isReceiving)
                {
                    // Cannot use Debug.Log here - it's not thread-safe
                    Console.WriteLine($"UDP receive error: {e.Message}");
                }
            }
        }
    }

    void Update()
    {
        ProcessQueuedData();
        UpdatePoseFromLatestData();
        ApplyPositionToTarget();

        if (showDebugInfo)
        {
            DisplayDebugInfo();
        }
    }

    void ProcessQueuedData()
    {
        // Process all queued data on main thread
        while (dataQueue.TryDequeue(out Dictionary<string, JointData> poseData))
        {
            latestPoseData = poseData;
            lastUpdateTime = Time.time; // Safe to call in main thread
        }
    }

    void UpdatePoseFromLatestData()
    {
        if (latestPoseData != null && latestPoseData.ContainsKey(targetJoint))
        {
            var joint = latestPoseData[targetJoint];

            Vector3 newPosition = new Vector3(
                (joint.x + positionOffset.x) * positionScale,
                (1f - joint.y + positionOffset.y) * positionScale,
                (joint.z + positionOffset.z) * positionScale
            );

            targetPosition = newPosition;
        }
    }

    void ApplyPositionToTarget()
    {
        if (enableSmoothing)
        {
            smoothedPosition = Vector3.Lerp(smoothedPosition, targetPosition, smoothingFactor);
            targetObject.localPosition = smoothedPosition;
        }
        else
        {
            targetObject.localPosition = targetPosition;
        }
    }

    void DisplayDebugInfo()
    {
        if (GUI.skin == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"UDP Pose Receiver Debug", GUI.skin.box);
        GUILayout.Label($"Port: {udpPort}");
        GUILayout.Label($"Target Joint: {targetJoint}");
        GUILayout.Label($"Packets Received: {packetsReceived}");
        GUILayout.Label($"Last Update: {Time.time - lastUpdateTime:F2}s ago");
        GUILayout.Label($"Current Position: {targetObject.localPosition}");

        if (latestPoseData != null && latestPoseData.ContainsKey(targetJoint))
        {
            var joint = latestPoseData[targetJoint];
            GUILayout.Label($"Raw Coordinates: ({joint.x:F3}, {joint.y:F3}, {joint.z:F3})");
            GUILayout.Label($"Visibility: {joint.visibility:F3}");
        }
        GUILayout.EndArea();
    }

    void OnDestroy()
    {
        StopUDP();
    }

    void OnApplicationQuit()
    {
        StopUDP();
    }

    void StopUDP()
    {
        isReceiving = false;

        if (udpThread != null && udpThread.IsAlive)
        {
            udpThread.Join(1000);
        }

        if (udpClient != null)
        {
            udpClient.Close();
            udpClient.Dispose();
        }

        Debug.Log("UDP Pose Receiver stopped");
    }

    [System.Serializable]
    public class JointData
    {
        public float x;
        public float y;
        public float z;
        public float visibility;
    }
}


