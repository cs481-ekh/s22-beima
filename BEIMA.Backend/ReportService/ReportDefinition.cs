using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.ReportService
{
    public static class ReportDefinition
    {
        public static IReportService ReportInstance { get; set; } = ReportWriter.Instance;
    }
}
