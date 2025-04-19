using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpoApp.Models.Entities
{
    public enum SaudaType
    {
        [Description("App")]
        App,
        [Description("Shares")]
        Shares,
        [Description("SubjectTo")]
        SubjectTo,
    }
    public enum TransactionType
    {
        [Description("BUY")]
        BUY,
        [Description("SELL")]
        SELL,
    }
    public enum AppType
    {
        [Description("Retail")]
        Retail,
        [Description("SHNI")]
        SHNI,
        [Description("BHNI")]
        BHNI,
    }
}
