using System;
using System.Linq;
using System.Xml.Linq;

namespace AlicatConfigurator
{
    /// <summary>
    /// Inherited class for alicat Mass Flow Controller
    /// </summary>
    class AlicatMc : Alicat
    {
        public readonly string gasesXmlPath = "AlicatMcGases.xml";
        public float CurrentTemp { get; private set; }
        public float CurrentVolFlow { get; private set; }
        public float CurrentMassFlow { get; private set; }
        public string CurrentGas { get; private set; }

        public override int GetReqMsgLength()
        {
            return 48;
        }

        public override string[] GetValueNames()
        {
            return new string[] { "Press", "Temp", "VolFlow", "MassFlow", "Setpt", "Gas", "Status", "LastUpdate" };
        }

        public override void ReadValues()
        {
            if (TryReadSerial())
            {
                string[] values = SplitLine();
                CurrentPress = StrToFloat(values[1]);
                CurrentTemp = StrToFloat(values[2]);
                CurrentVolFlow = StrToFloat(values[3]);
                CurrentMassFlow = StrToFloat(values[4]);
                CurrentSetpt = StrToFloat(values[5]);
                CurrentGas = values[values.Length - 1].Trim();
                Status = "OK";
            }
            else
                Status = "No Data";
        }

        public string[] GetGases()
        {
            string[] gases = XDocument.Load(gasesXmlPath)
                .Descendants("Gas")
                .Select(element => element.Attribute("Name").Value).ToArray();
            Array.Sort(gases);
            return gases;
        }

        private int GetGasNumber(string gas)
        {
            string[] gases = XDocument.Load(gasesXmlPath)
                .Descendants("Gas")
                .Where(element => element.Attribute("Name").Value == gas)
                .Select(element => element.Attribute("ID").Value).ToArray();
            return int.Parse(gases[0]);
        }

        public void SetGas(string gas)
        {
            WriteSerial($"{Id}G{GetGasNumber(gas)}\r");
        }
    }
}
