namespace Ref.Shared.Utils
{
    public static class Labels
    {
        public static string App => "[REF]";

        public static string ErrorMsgTitle => $"{App} Error";

        public static string NoNewRecordsMsgTitle => $"{App} Brak nowych ogłoszeń";
        public static string NoNewRecordsMsgContent => "Może przy następnym sprawdzaniu coś się pojawi.";

        public static string RecordsFoundTitle => $"{App} Nowe ogłoszenia";
        public static string RecordsFoundPushoverContent => "linki do ogłoszeń przesłane będą w mailu.";
    }
}