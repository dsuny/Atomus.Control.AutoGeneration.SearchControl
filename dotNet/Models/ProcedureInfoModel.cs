using System.Data;

namespace Atomus.Control.AutoGeneration.Models
{
    internal class ProcedureInfoModel : Mvc.Models
    {
        internal string ServiceName { get; set; }
        internal string ConnectionName { get; set; }
        internal string Info { get; set; }
        internal string ProcedureSearch { get; set; }
        internal DataTable ProcedureParameterInfo { get; set; }
        internal object[] ParameterValue { get; set; }
    }
}