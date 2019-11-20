using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;

public class Serial_filter : MonoBehaviour
{

    SerialPort stream;
    public GameObject target;
    float gx , gy, gz = 0;
    //float speedFactor = 7.0f;
    string port = "COM3";
    int baudrate = 9600;
    int readTimeout = 25;


    void Start()
    {
        stream = new SerialPort(port, baudrate);

        try
        {
            stream.ReadTimeout = readTimeout;
        }
        catch (System.IO.IOException ioe)
        {
            Debug.Log("IOException: " + ioe.Message);
        }

        stream.Open();

    }

    void Update()
    {

        string dataString = "null received";

        if (stream.IsOpen)
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (System.IO.IOException ioe)
            {
                Debug.Log("IOException: " + ioe.Message);
            }

        }
        else
            dataString = "NOT OPEN";
        Debug.Log("RCV_ : " + dataString);

        if (!dataString.Equals("NOT OPEN"))
        {
            char splitChar = ';' ;
            string[] dataRaw = dataString.Split(splitChar);
            Debug.Log(dataRaw[0]+";"+ dataRaw[1] + ";" + dataRaw[2] + ";");
            // normalized accelerometer values
            gx = float.Parse(dataRaw[0]);
            gy = float.Parse(dataRaw[1]);
            gz = float.Parse(dataRaw[2]);

        }
        //target.transform.rotation = Quaternion.Euler(gx * factor, gy * factor, gz * factor);
        target.transform.eulerAngles = new Vector3(gx, gy,gz);

    }

}

//42.522 //-1.274 //-1.715

    //67   // 42 //96
