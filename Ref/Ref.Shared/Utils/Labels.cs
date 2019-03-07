namespace Ref.Shared.Utils
{
    public static class Labels
    {
        public static string App => "[REF]";

        public static string ErrorMsgTitle => $"{App} Error";
        public static string BannedMsgTitle => $"{App} Banned";
        public static string BannedMsg(string site) => $"Zablokowano dostęp na: {site}.";

        public static string NoRecordsMsgTitle => $"{App} Pusto";
        public static string NoRecordsMsg(string site) => $"Brak wyników na: {site}.";

        public static string ExceptionMsgTitle => $"{App} Exception";
        public static string ExceptionMsg(string site, string message) => $"Site: {site}. Message: {message}.";

        public static string ErrorEmailMsg(string client) => $"Nie wysłano wiadomości do: {client}";

        public static string NoNewRecordsMsgTitle => $"{App} Brak nowych ogłoszeń";
        public static string NoNewRecordsMsgContent => "Może przy następnym sprawdzaniu coś się pojawi.";

        public static string RecordsFoundTitle => $"{App} Nowe ogłoszenia";
        public static string RecordsFoundPushoverContent => "linki do ogłoszeń przesłane będą w mailu.";

        public static string FilterDescPrice => "Cena (zł)";
        public static string FilterDescArea => "Powierzchnia (m2)";

        public static string EmptyEmailError => "Błąd! Próba wysłania pustej wiadomości e-mail.";
    }
}