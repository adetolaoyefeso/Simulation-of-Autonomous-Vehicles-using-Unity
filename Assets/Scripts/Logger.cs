using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;


public class Logger : MonoBehaviour
{
    /// <summary>
    /// Log data from the car/car camera
    /// See Nvidia : https://github.com/llSourcell/How_to_simulate_a_self_driving_car/blob/master/self-driving-car.ipynb
    /// Save data in a *.csv file
    /// Save data in a folder :: /log/*.csv
    /// Create our file save labelling system and output folder
    /// Currently saving image and steering data 
    /// </summary>

    public CameraSensor CamSensor1;

    public int curFrameCount = 0;
    public int maxFrameCount;
    //[SerializeField] GameObject car;
    private Car carControl;
    private float timeSinceLastLog = 0.0f;
 

    private float FPS = 30.0f;

    private StreamWriter sw;

    private string filename = "driving_log.csv";

    class ImageSaveJob
    {
        public string filename;
        public byte[] bytes;
    }

    List<ImageSaveJob> imagesToSave;

    Thread thread;



    [SerializeField] private bool isLog = true;

    //private string filepath = Application.dataPath + "/../log/";

    // Start is called before the first frame update
    void Start()
    {

        
        Debug.Log(Application.dataPath);
        
    }

    private string GetFilePath()
    {
        //store data in a folder called "log" in the "assets" folder
        return Application.dataPath + "/log/";
    }

    private void Awake()
    {
        //maxFrameCount = 10000;

        carControl = gameObject.GetComponentInParent<Car>();
        

        if(isLog && carControl != null)
        {
            //TODO

            string filePath = GetFilePath() + filename;
            //initialize filewriter
            sw = new StreamWriter(filePath);
        }



        imagesToSave = new List<ImageSaveJob>();

        thread = new Thread(SaverThread);
        thread.Start();
    }

    // Update is called once per frame
    // FPS = 50
    void Update()
    {

        if(!isLog)
        {
            return;
        }
        
        timeSinceLastLog += Time.deltaTime;
        //gameObject.transform.Rotate(90.0f/50.0f, 0.0f/50.0f, 90.0f/50.0f, Space.Self);
        if (timeSinceLastLog < 1.0f / FPS)
        {
            return;
            //TODO
        }

        timeSinceLastLog -= (1.0f / FPS);

        LogDataToCSV();
        SaveCamSensor(CamSensor1, "");

        if (curFrameCount >= maxFrameCount)
        {
            Debug.Log("Finish");
            Close();
            gameObject.SetActive(false);
        }

        curFrameCount += 1;


    



    }

    private void LogDataToCSV()
    {
        //data we wish to collect
        float curSteerAngle = carControl.GetSteering();
        float curThrottle = carControl.GetThrottle();
        //float curSpeed = carControl.GetVelocity();
        float curBrake = carControl.GetHandBrake();
        string camImage = GetFilePath() + "CenterCam_" + curFrameCount + ".png";

        Debug.Log("Steering Angle: " + curSteerAngle.ToString() + " Throttle: " + curThrottle.ToString()
            + " Brake: " + curBrake.ToString() + " Image Link: " + camImage);
        //if (!(curSteerAngle == 0 || curThrottle == 0 || curBrake == 0))
        //{

        //}
        sw.WriteLine(string.Format("{0},{1},{2},{3}", camImage, curSteerAngle.ToString(), curThrottle.ToString(), curBrake.ToString()));
        sw.Flush();
    }

    //Save the camera sensor to an image. Use the suffix to distinguish between cameras.
    void SaveCamSensor(CameraSensor cs, string prefix)
    {
        if (cs != null)
        {
            Texture2D image = cs.GetImage();
            //image.GetPixelData

            ImageSaveJob ij = new ImageSaveJob();


            ij.filename = GetFilePath() + "CenterCam_" + curFrameCount+ ".png"; //string.Format("{0}_{1,8:D8}.png", prefix, "CenterCam_" + curFrameCount);
            Debug.Log(ij.filename);

            ij.bytes = image.EncodeToPNG();

            lock (this)
            {
                imagesToSave.Add(ij);
            }
        }
    }

    public void SaverThread()
    {
        while (true)
        {
            int count = 0;

            lock (this)
            {
                count = imagesToSave.Count;
            }

            if (count > 0)
            {
                ImageSaveJob ij = imagesToSave[0];

                //Debug.Log("saving: " + ij.filename);

                File.WriteAllBytes(ij.filename, ij.bytes);

                lock (this)
                {
                    imagesToSave.RemoveAt(0);
                }
            }
        }
    }


    public void Togglelogger()
    {
        //if isLog is true; isLog = false
        //if isLog is false; isLog = true;

        isLog = !isLog;
    }

    //helper function to close instances
    private void Close()
    {
        if(sw != null)
        {
            sw.Close();
        }


    }


    
}
