using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Timers;

namespace client
{
    public partial class Form1 : Form
    {
        modbus mb = new modbus();
        SerialPort sp = new SerialPort();
        System.Timers.Timer timer = new System.Timers.Timer();
        as410 ucModule = new as410();
        bool isPolling = false;
        int pollCount;
        #region GUI Delegate Declarations
        public delegate void GUIDelegate(string paramString);
        public delegate void GUIClear();
        public delegate void GUIStatus(string paramString);
        #endregion

        public Form1()
        {
            InitializeComponent();
            LoadListboxes();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        #region Delegate Functions
        public void DoGUIClear()
        {
            if (this.InvokeRequired)
            {
                GUIClear delegateMethod = new GUIClear(this.DoGUIClear);
                this.Invoke(delegateMethod);
            }
            else
                this.textBox1.Clear();
        }
        public void DoGUIStatus(string paramString)
        {
            if (this.InvokeRequired)
            {
                GUIStatus delegateMethod = new GUIStatus(this.DoGUIStatus);
                this.Invoke(delegateMethod, new object[] { paramString });
            }
            else
                this.statusStrip1.Text = paramString;
        }
        public void DoGUIUpdate(string paramString)
        {
            if (this.InvokeRequired)
            {
                GUIDelegate delegateMethod = new GUIDelegate(this.DoGUIUpdate);
                this.Invoke(delegateMethod, new object[] { paramString });
            }
            else
               // this.lstRegisterValues.Items.Add(paramString);
             this.textBox1.Text = paramString;
        }
        #endregion

        #region Poll Function
        private void PollFunction()
        {
            //Update GUI:
            //DoGUIClear();
            pollCount++;
            DoGUIStatus("Poll count: " + pollCount.ToString());
          

            //Read registers and display data in desired format:
            try
            {
               mb.SendFc3(1, 0, 50, ref ucModule.holdingRegisters);
            }
            catch (Exception err)
            {
                DoGUIStatus("Error in modbus read: " + err.Message);
            }

            ucModule.updateState();
            DoGUIUpdate(ucModule.R_01.ToString());

        }
        #endregion

        #region Start and Stop Procedures
        private void StartPoll()
        {
            pollCount = 0;

            //Open COM port using provided settings:
            if (mb.Open(lstPorts.SelectedItem.ToString(), Convert.ToInt32(lstBaudrate.SelectedItem.ToString()),
                8, Parity.None, StopBits.One))
            {
                //Disable double starts:
                btnStart.Enabled = false;
               

                //Set polling flag:
                isPolling = true;

                //Start timer using provided values:
                timer.AutoReset = true;
                if (txtSampleRate.Text != "")
                    timer.Interval = Convert.ToDouble(txtSampleRate.Text);
                else
                    timer.Interval = 1000;
                timer.Start();
            }

           // lblStatus.Text = mb.modbusStatus;
        }
        private void StopPoll()
        {
            //Stop timer and close COM port:
            isPolling = false;
            timer.Stop();
            mb.Close();

            btnStart.Enabled = true;

         //   lblStatus.Text = mb.modbusStatus;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartPoll();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPoll();
        }
        #endregion


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PollFunction();
        }
        private void LoadListboxes()
        {
            //Three to load - ports, baudrates, datetype.  Also set default textbox values:
            //1) Available Ports:
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                lstPorts.Items.Add(port);
            }

            lstPorts.SelectedIndex = 0;

            //2) Baudrates:
            string[] baudrates = { "230400", "115200", "57600", "38400", "19200", "9600" };

            foreach (string baudrate in baudrates)
            {
                lstBaudrate.Items.Add(baudrate);
            }

            lstBaudrate.SelectedIndex = 1;

            //3 Parity
            Parity[] parities = {0, (Parity)1, (Parity)2, (Parity)3 };

            foreach (Parity parityMode in parities)
            {
                lstParity.Items.Add(parityMode);
            }
            lstParity.SelectedIndex = 2;
            //4 DataBits
            int[] dataBits = { 7,8};

            foreach (int dataBit in dataBits)
            {
                lstDataBits.Items.Add(dataBit);
            }
            lstDataBits.SelectedIndex = 1;
           //4 Stop bits
           StopBits[] stopBits = { 0, (StopBits)1, (StopBits)2, (StopBits)3 };

            foreach (int stopBit in stopBits)
            {
                lstStopBits.Items.Add(stopBit);
            }
            lstStopBits.SelectedIndex = 1;
            //3) Datatype:

            /*
            string[] dataTypes = { "Decimal", "Hexadecimal", "Float", "Reverse" };

            foreach (string dataType in dataTypes)
            {
                lstDataType.Items.Add(dataType);
            }

            lstDataType.SelectedIndex = 0;

            //Textbox defaults:
            txtRegisterQty.Text = "20";
            txtSampleRate.Text = "1000";
            txtSlaveID.Text = "1";
            txtStartAddr.Text = "0";
            */
        }

        private void Scan1()
        {
            //Open COM port using provided settings:
            if (mb.Open(lstPorts.SelectedItem.ToString(), 
                Convert.ToInt32(lstBaudrate.SelectedItem.ToString()),
                Convert.ToInt32(lstDataBits.SelectedItem.ToString()),
                (Parity)Enum.Parse(typeof(Parity), lstParity.SelectedItem.ToString()),
                (StopBits)Enum.Parse(typeof(StopBits), lstStopBits.SelectedItem.ToString())));
            
            {
                //Disable double starts:
               // btnScan.Enabled = false;
               
                //Set polling flag:
                isPolling = true;

               
            }

            //Create array to accept read values:
            as410 ucModule = new as410();


           
             
            //Read registers and display data in desired format:
            try
            {
                 mb.SendFc3(Convert.ToByte("1"), 1, 50, ref ucModule.holdingRegisters) ;
            }
            catch (Exception err)
            {
               // DoGUIStatus("Error in modbus read: " + err.Message);
            }
            mb.Close();
            ucModule.updateState();
            DoGUIUpdate(ucModule.R_01.ToString());
            //Set polling flag:
            isPolling = false;
        //    btnScan.Enabled = true;
            //blStatus.Text = mb.modbusStatus;
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            StartPoll();
        }

        private void btnStop_Click_1(object sender, EventArgs e)
        {
            StopPoll();
        }
    }
 }

