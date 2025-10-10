using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq; // needed for dictionary operations

public class UdpPoseReceiver : MonoBehaviour
{
    public int listenPort = 5005;
    private UdpClient udpClient;
    private Thread receiveThread;
    private Dictionary<string, JointData> jointData = new Dictionary<string, JointData>();
    private object lockObj = new object();

    public Transform jointPrefab; // Assign a small sphere prefab for visualization
    private Dictionary<string, Transform> jointVisuals = new Dictionary<string, Transform>();

    //For Bone Line Visualization and Color Visualization
    public Material lineMaterial;
    private List<LineRenderer> boneLines = new List<LineRenderer>();

    private readonly (string, string)[] bonePairs = new (string, string)[]
    {
    ("LEFT_SHOULDER", "RIGHT_SHOULDER"),
    ("LEFT_SHOULDER", "LEFT_ELBOW"),
    ("LEFT_ELBOW", "LEFT_WRIST"),
    ("RIGHT_SHOULDER", "RIGHT_ELBOW"),
    ("RIGHT_ELBOW", "RIGHT_WRIST"),
    ("LEFT_HIP", "RIGHT_HIP"),
    ("LEFT_SHOULDER", "LEFT_HIP"),
    ("RIGHT_SHOULDER", "RIGHT_HIP"),
    ("LEFT_HIP", "LEFT_KNEE"),
    ("LEFT_KNEE", "LEFT_ANKLE"),
    ("RIGHT_HIP", "RIGHT_KNEE"),
    ("RIGHT_KNEE", "RIGHT_ANKLE"),
    ("NOSE", "LEFT_EYE"),
    ("NOSE", "RIGHT_EYE"),
    ("NOSE", "LEFT_SHOULDER"),
    ("NOSE", "RIGHT_SHOULDER")
    };


    void Start()
    {
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveLoop));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // Optionally spawn 33 joint spheres in the scene
        foreach (string joint in JointMapping.Keys)
        {
            Transform visual = Instantiate(jointPrefab, Vector3.zero, Quaternion.identity);
            visual.name = joint;
            jointVisuals[joint] = visual;

            if (jointColors.TryGetValue(joint, out Color color))
            {
                Renderer renderer = visual.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }

        

        foreach (var pair in bonePairs)
        {
            GameObject lineObj = new GameObject("Bone_" + pair.Item1 + "_" + pair.Item2);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.positionCount = 2;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.useWorldSpace = true;
            boneLines.Add(lr);
        }
    }

    void Update()
    {
        lock (lockObj)
        {
            foreach (var pair in jointData)
            {
                if (jointVisuals.ContainsKey(pair.Key))
                {
                    Vector3 pos = new Vector3(pair.Value.x - 0.5f, 1 - pair.Value.y, -pair.Value.z);
                    jointVisuals[pair.Key].localPosition = pos * 5f;
                }
            }

            // Draw Bone lines between connected joints
            for (int i = 0; i < bonePairs.Length; i++)
            {
                var (a, b) = bonePairs[i];
                if (jointVisuals.ContainsKey(a) && jointVisuals.ContainsKey(b))
                {
                    LineRenderer lr = boneLines[i];
                    lr.SetPosition(0, jointVisuals[a].position);
                    lr.SetPosition(1, jointVisuals[b].position);
                    lr.enabled = true;
                }
                else
                {
                    boneLines[i].enabled = false;
                }
            }
        }


        
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string json = Encoding.UTF8.GetString(data);
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, JointData>>(json);
                lock (lockObj)
                {
                    jointData = parsed;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("UDP receive error: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }

    public class JointData
    {
        public float x;
        public float y;
        public float z;
        public float visibility;
    }

    public static Dictionary<string, int> JointMapping = new Dictionary<string, int>
    {
        { "NOSE", 0 }, { "LEFT_EYE_INNER", 1 }, { "LEFT_EYE", 2 }, { "LEFT_EYE_OUTER", 3 },
        { "RIGHT_EYE_INNER", 4 }, { "RIGHT_EYE", 5 }, { "RIGHT_EYE_OUTER", 6 },
        { "LEFT_EAR", 7 }, { "RIGHT_EAR", 8 },
        { "MOUTH_LEFT", 9 }, { "MOUTH_RIGHT", 10 },
        { "LEFT_SHOULDER", 11 }, { "RIGHT_SHOULDER", 12 },
        { "LEFT_ELBOW", 13 }, { "RIGHT_ELBOW", 14 },
        { "LEFT_WRIST", 15 }, { "RIGHT_WRIST", 16 },
        { "LEFT_PINKY", 17 }, { "RIGHT_PINKY", 18 },
        { "LEFT_INDEX", 19 }, { "RIGHT_INDEX", 20 },
        { "LEFT_THUMB", 21 }, { "RIGHT_THUMB", 22 },
        { "LEFT_HIP", 23 }, { "RIGHT_HIP", 24 },
        { "LEFT_KNEE", 25 }, { "RIGHT_KNEE", 26 },
        { "LEFT_ANKLE", 27 }, { "RIGHT_ANKLE", 28 },
        { "LEFT_HEEL", 29 }, { "RIGHT_HEEL", 30 },
        { "LEFT_FOOT_INDEX", 31 }, { "RIGHT_FOOT_INDEX", 32 }
    };

    private Dictionary<string, Color> jointColors = new Dictionary<string, Color>()
{
    // Head
    { "NOSE", Color.yellow }, { "LEFT_EYE", Color.yellow }, { "RIGHT_EYE", Color.yellow },
    { "LEFT_EYE_INNER", Color.yellow }, { "LEFT_EYE_OUTER", Color.yellow },
    { "RIGHT_EYE_INNER", Color.yellow }, { "RIGHT_EYE_OUTER", Color.yellow },
    { "LEFT_EAR", Color.yellow }, { "RIGHT_EAR", Color.yellow },
    { "MOUTH_LEFT", Color.yellow }, { "MOUTH_RIGHT", Color.yellow },

    // Torso
    { "LEFT_SHOULDER", Color.green }, { "RIGHT_SHOULDER", Color.green },
    { "LEFT_HIP", Color.green }, { "RIGHT_HIP", Color.green },

    // Arms
    { "LEFT_ELBOW", Color.blue }, { "RIGHT_ELBOW", Color.blue },
    { "LEFT_WRIST", Color.blue }, { "RIGHT_WRIST", Color.blue },
    { "LEFT_PINKY", Color.blue }, { "RIGHT_PINKY", Color.blue },
    { "LEFT_INDEX", Color.blue }, { "RIGHT_INDEX", Color.blue },
    { "LEFT_THUMB", Color.blue }, { "RIGHT_THUMB", Color.blue },

    // Legs
    { "LEFT_KNEE", Color.red }, { "RIGHT_KNEE", Color.red },
    { "LEFT_ANKLE", Color.red }, { "RIGHT_ANKLE", Color.red },
    { "LEFT_HEEL", Color.red }, { "RIGHT_HEEL", Color.red },
    { "LEFT_FOOT_INDEX", Color.red }, { "RIGHT_FOOT_INDEX", Color.red }
};

}
