namespace System.Alerts.Monitor.Common
{
    public enum ServiceOperationMode { CSV, SQLITE }

    public enum SystemAlertType
    {
        OK = 1,
        WARNING = 0,
        ERROR = -1,
        NA = -999,
    }
}
