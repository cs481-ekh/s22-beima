using BEIMA.Backend.MongoService;
using System.Net;

namespace BEIMA.Backend
{
    /// <summary>
    /// Class for verifying and validating request data.
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Verifies that a given device has valid properties.
        /// </summary>
        /// <param name="device">Device to verify.</param>
        /// <param name="message">The error message for a failed validation.</param>
        /// <param name="httpStatusCode">The status code for a faild validation.</param>
        /// <returns>True if the device is valid, otherwise false.</returns>
        public static bool IsDeviceValid(Device device, out string message, out HttpStatusCode httpStatusCode)
        {
            bool isValid = true;
            message = string.Empty;
            httpStatusCode = HttpStatusCode.OK;

            // TODO: Assert that deviceTypeId is valid.
            if (device is null)
            {
                message = "Device is null.";
                httpStatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            return isValid;
        }
    }
}
