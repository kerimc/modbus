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

namespace client
{
    public partial class Form1 : Form
    {
        modbus mb = new modbus();
        SerialPort sp = new SerialPort();
        System.Timers.Timer timer = new System.Timers.Timer();
        string dataType;
        bool isPolling = false;
        int pollCount;

        public Form1()
        {
            InitializeComponent();
            LoadListboxes();
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
                btnScan.Enabled = false;
               
                //Set polling flag:
                isPolling = true;

               
            }

            //Create array to accept read values:
            short[] values = new short[Convert.ToInt32("1")];
            ushort pollStart;
            ushort pollLength;

           
                pollStart = 0;
           
                pollLength = 1;

            //Read registers and display data in desired format:
            try
            {
                while (!mb.SendFc3(Convert.ToByte("1"), pollStart, pollLength, ref values)) ;
            }
            catch (Exception err)
            {
               // DoGUIStatus("Error in modbus read: " + err.Message);
            }
            mb.Close();

            //blStatus.Text = mb.modbusStatus;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            Scan1();
        }
    }
 }

