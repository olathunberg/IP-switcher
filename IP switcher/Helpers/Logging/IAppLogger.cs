using System;

namespace TTech.IP_Switcher.Helpers.Logging;

internal interface IAppLogger
{
    void LogInfo(string context, string message);

    void LogError(string context, string message, Exception exception = null);
}
