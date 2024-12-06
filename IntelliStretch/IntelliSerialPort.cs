using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace IntelliStretch
{
    public class IntelliSerialPort
    {
        #region Variables & Delegates

        SerialPort sp;
        List<SerialPort> scanList;
        System.Timers.Timer scanTimer;
        string dataQueue;
        AnkleData ankleData;
        StreamWriter dataWriter;

        public struct AnkleData
        {
            public float anklePos;
            public float ankleTorque;
            public float ankleAm;
            public float targetPos;
            public string revCmd;
        }
        public delegate void DelegateUpdateStatus(string msg);
        public DelegateUpdateStatus UpdateStatus;
        public delegate void DelegateUpdateUI(bool isDone, bool isReady);
        public DelegateUpdateUI UpdateUI;
        public delegate void DelegateUpdateData(AnkleData newAnkleData);
        public DelegateUpdateData UpdateData;

        #endregion

        #region Properties
        public bool IsConnected { get; set; }
        public bool IsUpdating { get; set; }
        public bool IsSaving { get; set; }
        public string DataDir { get; set; }
        public string DataFilePrefix { get; set; }
        #endregion

        public IntelliSerialPort()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Initialization
            sp = new SerialPort();
            sp.DataBits = 8;
            sp.Parity = Parity.Even;
            sp.StopBits = StopBits.One;
            sp.BaudRate = 38400;
            sp.WriteBufferSize = 8192;
            sp.ReadBufferSize = 8192;
            //sp.ReadTimeout = 100;
            sp.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        public void Connect(string portName)
        {
            sp.PortName = portName;

            if (sp.IsOpen) sp.Close();
            try
            {
                sp.Open();
            }
            catch (Exception ex)
            {
                UpdateStatus("Error: " + ex.ToString());
                return;
            }
            UpdateStatus("Successfully Connected!");
            UpdateUI(true, true);
            IsConnected = true;
            sp.DiscardInBuffer();
        }

        public void Close()
        {
            if (sp.IsOpen) sp.Close();
        }

        public bool IsOpen()
        {
            if (sp.IsOpen) return true;
            else return false;
        }


        public void AutoConnect()
        {
            ScanPort();
        }

        public void ScanPort()
        {
            // Scan port
            UpdateStatus("Scanning port ...");
            UpdateUI(false, false);
            scanTimer = new System.Timers.Timer();
            scanTimer.Interval = 5000;
            scanTimer.AutoReset = false;
            scanTimer.Elapsed += new System.Timers.ElapsedEventHandler(scanTimer_Elapsed);

            string[] portList = Format_PortNames(SerialPort.GetPortNames());
            if (scanList == null) scanList = new List<SerialPort>();
            foreach (string port in portList)
            {
                SerialPort newSP = new SerialPort(port, 38400, Parity.Even, 8, StopBits.One);
                newSP.DataReceived += new SerialDataReceivedEventHandler(newSP_ScanDataReceived);
                scanList.Add(newSP);
            }

            foreach (SerialPort port in scanList)
            {
                if (!port.IsOpen)
                {
                    try
                    {
                        port.Open();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    port.DiscardInBuffer();
                }
            }

            scanTimer.Start();
        }

        public void readLine()
        {
            if(sp.IsOpen) sp.ReadChar();
        }

        public void WriteCmd(string cmd)
        {
            //int numS=0;
            //int numTT = 0;
            if (sp.IsOpen)
            {
                sp.WriteLine(cmd);
                /*while (!String.Equals(cmd, ankleData.revCmd, StringComparison.Ordinal))
                {
                    if (this.IsUpdating && UpdateData != null) UpdateData(ankleData);
                    //System.Threading.Thread.Sleep(50);
                    //Application.DoEvents();
                    numS++;
                    if (numS >10)
                    {
                        MessageBox.Show("Resent:" + cmd + "," + ankleData.revCmd); 
                        sp.WriteLine(cmd);
                        
                        numS = 0;
                        numTT++;
                    }
                    if (numTT > 100) { MessageBox.Show("Failed:" + cmd + "," + ankleData.revCmd); break; }
                }*/

            }
        }

        public void Start_SaveData(string mode)
        {
            string dataFile = DataDir + DataFilePrefix + "_" + mode + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            dataWriter = new StreamWriter(dataFile);
            this.IsSaving = true;
        }

        public void Stop_SaveData()
        {
            this.IsSaving = false;
            // check if there is instance of datawriter and close it
            if (dataWriter != null) dataWriter.Close();
        }

        private void Close_AllPorts()
        {
            foreach (SerialPort port in scanList)
            {
                port.Close();
            }
        }

        private void scanTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // if there is no data received when scanning
            scanTimer.Stop();
            Close_AllPorts();
            UpdateStatus("No available device found!");
            UpdateUI(true, false);
        }

        private void newSP_ScanDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // if there is data received when scanning
            scanTimer.Stop();  // stop timer
            scanTimer.Elapsed -= scanTimer_Elapsed;
            UpdateStatus("Device found! Connecting...");
            Close_AllPorts(); // close all scanning ports            

            System.Threading.Thread.Sleep(500);
            Connect((sender as SerialPort).PortName);
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataQueue += sp.ReadExisting(); // Read data, add to queue,08.22.2013.Yupeng
            
            //try
            {
               //dataQueue = sp.ReadLine();

                // Decoding data
                if (dataQueue.IndexOf("\r\n") >= 0)
                {
                    // If whole data line available
                    // Debug.Print(dataQueue);
                    int nP = dataQueue.IndexOf('P', 0); // Position tag
                    if (nP >= 0)
                    {
                        int nT = dataQueue.IndexOf('T', nP);  // Torque tag
                        if (nT > 0)
                        {
                            int nA = dataQueue.IndexOf('A', nT); // Current tag
                            if (nA > 0)
                            {
                                int nD = dataQueue.IndexOf('D', nA);  // Target tag, optional
                                if (nD > 0)
                                {
                                    int nW = dataQueue.IndexOf('W', nD); // received command, 08.22.2013.Yupeng
                                    //int nE = (nW > 0) ? dataQueue.IndexOf('E', nW) : dataQueue.IndexOf('E', nD); // End tag ,08.22.2013.Yupeng, Since DSp has EXn parameter
                                    int nR = (nW > 0) ? dataQueue.IndexOf('\r', nW) : dataQueue.IndexOf('\r', nD); // End tag ,08.22.2013.Yupeng

                                    if (nR > 0)
                                    {
                                        try
                                        {
                                            // Data format correct
                                            string strAnklePos = dataQueue.Substring(nP + 1, nT - nP - 1); // position string
                                            string strAnkleTorque = dataQueue.Substring(nT + 1, nA - nT - 1); // torque string
                                            string strAnkleAm = dataQueue.Substring(nA + 1, nD - nA - 1); // current string
                                            string strTargetPos = "";
                                            string strREVCMD = "";
                                            if (nW > 0)
                                            {
                                                strTargetPos =  dataQueue.Substring(nD + 1, nW - nD - 1); // target string
                                                if ((nR - nW - 2) > 0) strREVCMD = dataQueue.Substring(nW + 1, nR - nW - 2);//08.22.2013.Yupeng
                                            }
                                            else if (nW <= 0)
                                            {
                                                strTargetPos = dataQueue.Substring(nD + 1, nR - nD - 2); // target string
                                            }

                                            ankleData.anklePos = -Convert.ToSingle(strAnklePos) / 100f;  // position data
                                            ankleData.ankleTorque = -Convert.ToSingle(strAnkleTorque) / 100f; // torque data
                                            ankleData.ankleAm = Convert.ToSingle(strAnkleAm) / 100f; // current data
                                            ankleData.targetPos = -Convert.ToSingle(strTargetPos) / 100f;  // target position data
                                            ankleData.revCmd = strREVCMD; // received comd. 08.22.2013.Yupeng

                                            if (this.IsSaving)
                                            {
                                                //dataWriter.Write(dataQueue +"\n");  // save ankle data
                                                dataWriter.Write(dataQueue); 
                                                //dataWriter.WriteCmd(ankleData.anklePos.ToString() + " " + ankleData.ankleTorque.ToString() + " " + ankleData.ankleAm.ToString());  // save ankle data
                                            }
                                            if (this.IsUpdating && UpdateData != null) UpdateData(ankleData);  // update new ankle data

                                            dataQueue = ""; // Clear the data queue
                                            
                                        }
                                        catch (Exception)
                                        {
                                            //MessageBox.Show("Data Decoding failure.");
                                            sp.DiscardInBuffer();
                                            dataQueue = "";
                                        }

                                    }
                                }
                            }
                        }
                    }

                }
            }
            //catch (TimeoutException) { MessageBox.Show("Receiving Delay."); }
        }

        private string[] Format_PortNames(string[] portNames)
        {
            // Format port names
            for (int i = 0; i < portNames.Length; i++)
            {
                char[] nameChars = portNames[i].ToCharArray();
                int nameLen = nameChars.Length;
                int newLen = nameLen - 1;
                while (newLen > 3)
                {
                    if (char.IsNumber(nameChars[newLen])) break;
                    newLen--;
                }

                if (newLen != nameLen - 1) portNames[i] = portNames[i].Substring(0, newLen + 1);
            }

            return portNames;
        }

    }
}
