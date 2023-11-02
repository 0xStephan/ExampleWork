using VSpaceParkers.Helpers;

namespace VSpaceParkers
{
    public class Constants
    {
        //public string BaseIPAddress = "192.168.0.135";
        //public string BasePortNumber = "44360";
        //public string BaseApiAddress => "https://" + BaseIPAddress + ":" + BasePortNumber + "/";
        //public static string BaseApiAddress => "https://192.168.0.125:44360/";

        public static string StringSplitRegex => "(?<!)";
        public static int MinPWLenth => 6;
        public static int MaxPWLength => 50;
        public static char SpotIDSeperator => '_';
        public static int MinUsername => 3;
        public static int MaxUsername => 25;
        public static int MinName => 3;
        public static int MaxName => 100;
        public static int MinPhoneNumber => 8;
        public static int MaxPhoneNumber => 25;
        public static int MinApartment => 3;
        public static int MaxApartment => 100;
        public static int MinCarrego => 1;
        public static int MaxCarrego => 25;
        public static int StartCycle => 1;
        public static int EndCycle => 0;
        public static string MainGate => "0_0";

        public static string ButtonColor => "#2E4A94";
        // #199eca #157efb 001e72    new color #2E4A94
        public static string MainGateColor => "#63e38a";

        // HTTP Status Codes
        public static string OK => "200";
        public static string Unauth => "401";
        public static string Banned => "403";
        public static string Offline => "503";
        public static string Error => "500";
        public static string BadPassword => "204";
        public static string Accepted => "202";
        public static string Custom => "220";
        public static string SessionClose => "301";
        public static string UserExists => "409";

        // IoT Status Codes
        public static string EmergencyStopPress => "501";
        public static string FrontSensorInterrupted => "505";
        public static string RearSensorInterrupted => "506";

        // System Number to Letter (make sure -1 as index starts at 0)
        public static string[] SystemLetter = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };
    }
}
