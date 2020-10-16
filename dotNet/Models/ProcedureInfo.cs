using System.Data;

namespace Atomus.Control.AutoGeneration.Models
{
    internal class ProcedureInfo : Mvc.Models
    {
        internal string ServiceName { get; set; }
        internal string DatabaseName { get; set; }
        internal string Info { get; set; }
        internal string ProcedureSearch { get; set; }
        internal DataTable ProcedureParameterInfo { get; set; }
        internal object[] ParameterValue { get; set; }
    }
}