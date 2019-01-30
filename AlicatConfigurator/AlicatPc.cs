namespace AlicatConfigurator
{
    /// <summary>
    /// Inherited class for Alicat Pressure Controller
    /// </summary>
    class AlicatPc : Alicat
    {
        public override int GetReqMsgLength()
        {
            return 16;
        }

        public override string[] GetValueNames()
        {
            return new string[] { "Press", "Setpt", "Status", "LastUpdate" };
        }

        public override void ReadValues()
        {
            if (TryReadSerial())
            {
                string[] values = SplitLine();
                CurrentPress = StrToFloat(values[1]);
                CurrentSetpt = StrToFloat(values[2]);
                Status = "OK";
            }
            else
                Status = "No Data";
        }
    }
}
