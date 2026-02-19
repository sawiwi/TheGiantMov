using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using Newtonsoft.Json.Linq;

public class FaceReceiver : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;
    public float faceX, faceY; // valores normalizados
    bool running = true;

    void Start()
    {
        Debug.Log("UDP Listener Starting FaceReceiver in port 5005");
        client = new UdpClient(5005);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        while (running)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                JObject obj = JObject.Parse(text);
                faceX = (float)obj["cx"];
                faceY = (float)obj["cy"];
                Debug.Log(faceX + ", " + faceY);

            }
            catch (System.Exception e) { 

                Debug.Log("Error receiving data: " + e.Message);

                // Debug.Log(faceX + ", " + faceY + " - Error: " + e.Message  );

            }
        }
    }

    void OnApplicationQuit()
    {
        // if (receiveThread != null) receiveThread.Abort();
        running = false;
        if (client!= null)
            client.Close();
    }

    
}
