using Atomus.Control.AutoGeneration.Models;
using Atomus.Service;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Atomus.Control.AutoGeneration.Controllers
{
    internal static class SearchControlController
    {
        internal static async Task<IResponse> SearchProcedureInfo(this ICore core, ProcedureInfoModel procedureInfo)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet {
                ServiceName = procedureInfo.ServiceName,
                TransactionScope = false
            };
            serviceDataSet["SearchProcedureInfo"].ConnectionName = procedureInfo.ConnectionName;
            serviceDataSet["SearchProcedureInfo"].CommandText = procedureInfo.Info;
            //serviceDataSet["SearchProcedureInfo"].SetAttribute("ProcedureID", procedureInfo.Info);
            serviceDataSet["SearchProcedureInfo"].AddParameter("@PROCEDURE_NAME", Database.DbType.NVarChar, 4000);

            serviceDataSet["SearchProcedureInfo"].NewRow();
            serviceDataSet["SearchProcedureInfo"].SetValue("@PROCEDURE_NAME", procedureInfo.ProcedureSearch);

            return await core.ServiceRequestAsync(serviceDataSet);
        }

        internal static async Task<IResponse> SearchSampleSearchData(this ICore core, ProcedureInfoModel procedureInfo)
        {
            IServiceDataSet serviceDataSet;
            AtomusControlArgs atomusControlArgs;
            string parameterName;
            string defautValue;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = procedureInfo.ServiceName,
                TransactionScope = false
            };
            serviceDataSet["SearchProcedureInfo"].ConnectionName = procedureInfo.ConnectionName;
            serviceDataSet["SearchProcedureInfo"].CommandText = procedureInfo.ProcedureSearch;
            //serviceDataSet["SearchProcedureInfo"].SetAttribute("ProcedureID", procedureInfo.ProcedureSearch);

            foreach (DataRow dataRow in procedureInfo.ProcedureParameterInfo.Rows)
            {
                serviceDataSet["SearchProcedureInfo"].AddParameter((string)dataRow["Parameter_name"], DbTypeConvert((string)dataRow["Type"]), (int)dataRow["Prec"]);
            }

            atomusControlArgs = new AtomusControlArgs();

            serviceDataSet["SearchProcedureInfo"].NewRow();
            foreach (DataRow dataRow in procedureInfo.ProcedureParameterInfo.Rows)
            {
                parameterName = (string)dataRow["Parameter_name"];

                atomusControlArgs.Action = string.Format("{0}.{1}", parameterName, "DefautValue");
                ((IAction)core).ControlAction(core, atomusControlArgs);

                defautValue = (string)atomusControlArgs.Value;

                if (defautValue != null & (string)defautValue != "")
                {
                    if (defautValue.StartsWith("Atomus.Config.Client.GetAttribute("))
                        serviceDataSet["SearchProcedureInfo"].SetValue(parameterName, Config.Client.GetAttribute(defautValue.Split("\"".ToCharArray())[1]));
                    else
                        serviceDataSet["SearchProcedureInfo"].SetValue(parameterName, (string)atomusControlArgs.Value);
                }
                else
                    serviceDataSet["SearchProcedureInfo"].SetValue(parameterName, DBNull.Value);
            }

            return await core.ServiceRequestAsync(serviceDataSet);
        }

        internal static async Task<IResponse> SearchhData(this ICore core, ProcedureInfoModel procedureInfo)
        {
            IServiceDataSet serviceDataSet;
            string parameterName;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = procedureInfo.ServiceName,
                TransactionScope = false
            };
            serviceDataSet["SearchProcedureInfo"].ConnectionName = procedureInfo.ConnectionName;
            serviceDataSet["SearchProcedureInfo"].CommandText = procedureInfo.ProcedureSearch;
            //serviceDataSet["SearchProcedureInfo"].SetAttribute("ProcedureID", procedureInfo.ProcedureSearch);

            foreach (DataRow dataRow in procedureInfo.ProcedureParameterInfo.Rows)
            {
                serviceDataSet["SearchProcedureInfo"].AddParameter((string)dataRow["Parameter_name"], DbTypeConvert((string)dataRow["Type"]), (int)dataRow["Prec"]);
            }
            
            serviceDataSet["SearchProcedureInfo"].NewRow();
            for (int i = 0; i < procedureInfo.ProcedureParameterInfo.Rows.Count; i++)
            {
                parameterName = (string)procedureInfo.ProcedureParameterInfo.Rows[i]["Parameter_name"];
                serviceDataSet["SearchProcedureInfo"].SetValue(parameterName, procedureInfo.ParameterValue[i]);
            }

            return await core.ServiceRequestAsync(serviceDataSet);
        }

        internal static Database.DbType DbTypeConvert(string dbType)
        {
            switch (dbType)
            {
                case "bigint":
                    return Database.DbType.BigInt;

                case "binary":
                    return Database.DbType.Binary;

                case "bit":
                    return Database.DbType.Bit;

                case "char":
                    return Database.DbType.Char;

                case "date":
                    return Database.DbType.Date;

                case "datetime":
                    return Database.DbType.DateTime;

                case "datetime2":
                    return Database.DbType.DateTime2;

                case "datetimefffset":
                    return Database.DbType.DateTimeOffset;

                case "decimal":
                    return Database.DbType.Decimal;

                case "float":
                    return Database.DbType.Float;

                case "image":
                    return Database.DbType.Image;

                case "int":
                    return Database.DbType.Int;

                case "money":
                    return Database.DbType.Money;

                case "nchar":
                    return Database.DbType.NChar;

                case "ntext":
                    return Database.DbType.NText;

                case "nvarchar":
                    return Database.DbType.NVarChar;

                case "real":
                    return Database.DbType.Real;

                case "smalldatetime":
                    return Database.DbType.SmallDateTime;

                case "smallint":
                    return Database.DbType.SmallInt;

                case "smallmoney":
                    return Database.DbType.SmallMoney;

                case "structured":
                    return Database.DbType.Structured;

                case "text":
                    return Database.DbType.Text;

                case "time":
                    return Database.DbType.Time;

                case "timestamp":
                    return Database.DbType.Timestamp;

                case "tinyint":
                    return Database.DbType.TinyInt;

                case "udt":
                    return Database.DbType.Udt;

                case "iniqueIdentifier":
                    return Database.DbType.UniqueIdentifier;

                case "varbinary":
                    return Database.DbType.VarBinary;

                case "varchar":
                    return Database.DbType.VarChar;

                case "variant":
                    return Database.DbType.Variant;

                case "xml":
                    return Database.DbType.Xml;

                default:
                    return Database.DbType.Variant;
            }
        }
    }
}