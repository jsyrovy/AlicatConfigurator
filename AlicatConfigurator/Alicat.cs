using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Threading;

namespace AlicatConfigurator
{
    /// <summary>
    /// Base class for all Alicats
    /// </summary>
    class Alicat
    {
        protected SerialPort sp;
        public string Device { get; private set; }
        public string PortName { get; private set; }
        public int PortBaudRate { get; private set; }
        public char Id { get; private set; }
        public float Setpt { get; private set; }
        public bool IsConnected { get; private set; }
        public string CurrentMsg { get; protected set; }
        public float CurrentSetpt { get; protected set; }
        public float CurrentPress { get; protected set; }
        public string Status { get; set; }
        public DateTime LastUpdate { get; private set; }

        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }

        public static char[] GetIds()
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        }

        public static int[] GetBaudRates()
        {
            return new int[] { 2400, 9600, 19200, 38400, 57600 };
        }

        public virtual void ReadValues()
        {
            // Base class does nothing
        }

        public virtual int GetReqMsgLength()
        {
            // Base class returns zero
            return 0;
        }

        public virtual string[] GetValueNames()
        {
            // Base class returns null
            return null;
        }

        public void Config(string device, string portName, int portBaudRate, char id)
        {
            Device = device;
            PortName = portName;
            PortBaudRate = portBaudRate;
            Id = id;
        }

        public void Connect()
        {
            sp = new SerialPort
            {
                PortName = PortName,
                BaudRate = PortBaudRate
            };
            sp.Open();
            IsConnected = sp.IsOpen;
        }

        public void Disconnect()
        {
            sp.Close();
            IsConnected = false;
        }

        public void DisconnectWithTimer(System.Windows.Threading.DispatcherTimer dispatcherTimer)
        {
            Disconnect();
            dispatcherTimer.Stop();
        }

        protected void WriteSerial(string msg)
        {
            Debug.WriteLine($"WriteSerial.Msg: {msg}");
            sp.Write(msg);
            Thread.Sleep(100);
        }

        protected bool ReadSerial()
        {
            CurrentMsg = "";
            WriteSerial($"{Id}\r");
            CurrentMsg = sp.ReadExisting();
            Debug.WriteLine($"ReadSerial.CurrentMsg: {CurrentMsg}");
            Debug.WriteLine($"ReadSerial.CurrentMsgLength: {GetCurrentMsgLength()}");
            if (CurrentMsg == "")
                return false;
            else
                return true;
        }

        protected float StrToFloat(string value)
        {
            return float.Parse(value, new NumberFormatInfo() { NumberDecimalSeparator = "." });
        }

        protected string[] SplitLine()
        {
            LastUpdate = DateTime.Now;
            string lastMsg = CurrentMsg.Split('\r')[CurrentMsg.Split('\r').Length - 2];
            return lastMsg.Split(' ');
        }

        public void SetSetpt(float setpt)
        {
            WriteSerial($"{Id}S{setpt.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." })}\r");
        }

        public int GetCurrentMsgLength()
        {
            return CurrentMsg.Split('\r')[0].Length;
        }

        public bool TryReadSerial(int seconds = 1)
        {
            return System.Threading.SpinWait.SpinUntil(ReadSerial, TimeSpan.FromSeconds(seconds));
        }
    }
}
