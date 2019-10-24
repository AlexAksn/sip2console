namespace Sip2Console
{
    public static class Sip2Command
    {
        //Message responses from ACS to SC
        public static readonly string AcsStatus = "98";
        public static readonly string EndPatronSession = "63";
        public static readonly string EndSessionResp = "36";
        public static readonly string FidCksum = "AZ";
        public static readonly string FidSeq = "AY";
        public static readonly string FidEndItem = "BQ";
        public static readonly string FidInstId = "AO";
        public static readonly string FidLoginPwd = "CO";
        public static readonly string FidLoginUid = "CN";

        //Field Identifiers
        public static readonly string FidPatronId = "AA";
        public static readonly string FidPatronPwd = "AD";
        public static readonly string FidPersonalName = "AE";
        public static readonly string FidStartItem = "BP";
        public static readonly string FidTerminalPwd = "AC";
        public static readonly string FidValidPatron = "BL";
        public static readonly string FidValidPatronPwd = "CQ";
        public static readonly string FidLocationCode = "CP";
        public static readonly string FidYes = "Y";
        public static readonly string Login = "93";
        public static readonly string LoginResp = "94";
        public static readonly string Ok = "1";
        public static readonly string PatronInfo = "63";
        public static readonly string PatronInfoResp = "64";
        //Messages from SC to ACS
        public static readonly string ScStatus = "99";
        public static readonly string Version = "2.00";

        public static readonly string SipDatetime = "yyyyMMdd    HHmmss";

        public static readonly string Summary = "          "; //10-char, fixed-length required field

        public static readonly string SipStatusRegex = $@"{FidInstId}(?<id>[^\|]*)\|";
    }
}
