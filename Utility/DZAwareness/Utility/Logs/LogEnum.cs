using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TargetSelector = PortAIO.TSManager; namespace DZAwarenessAIO.Utility.Logs
{
    /// <summary>
    /// The Log Severity enum
    /// </summary>
    [DefaultValue(Medium)]
    enum LogSeverity
    {
        Warning, Error, Low, Medium, Severe
    }
}
