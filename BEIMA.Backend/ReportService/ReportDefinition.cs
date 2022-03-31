using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIMA.Backend.ReportService
{
    /// <summary>
    /// Helps to store the instance of the report writer being used.
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to allow us to switch out the default
    /// report writer with a mock report writer for testing.
    /// </remarks>
    public static class ReportDefinition
    {
        /// <summary>
        /// Gets and sets the currently defined instance of the report writer
        /// </summary>
        public static IReportService ReportInstance { get; set; } = ReportWriter.Instance;
    }
}
