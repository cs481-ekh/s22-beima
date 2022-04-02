using BEIMA.Backend.MongoService;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="httpStatusCode">The status code for a failed validation.</param>
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

        /// <summary>
        /// Verifies that a given device type has valid properties.
        /// </summary>
        /// <param name="deviceType">Device type to verify.</param>
        /// <param name="message">The error message for a failed validation.</param>
        /// <param name="httpStatusCode">The status code for a failed validation.</param>
        /// <returns>True if the device type is valid, otherwise false.</returns>
        public static bool IsDeviceTypeValid(DeviceType deviceType, out string message, out HttpStatusCode httpStatusCode)
        {
            bool isValid = true;
            message = string.Empty;
            httpStatusCode = HttpStatusCode.OK;
            // TODO: Check max string lengths?
            var fields = deviceType.Fields.ToDictionary();
            if (fields.Values.Distinct().Count() != fields.Count)
            {
                isValid = false;
                message = "Cannot have matching field names";
                httpStatusCode = HttpStatusCode.BadRequest;
            }
            return isValid;
        }

        /// <summary>
        /// Verifies that a given building has valid properties.
        /// </summary>
        /// <param name="device">Building to verify.</param>
        /// <param name="message">The error message for a failed validation.</param>
        /// <param name="httpStatusCode">The status code for a failed validation.</param>
        /// <returns>True if the building is valid, otherwise false.</returns>
        public static bool IsBuildingValid(Building building, out string message, out HttpStatusCode httpStatusCode)
        {
            bool isValid = true;
            message = string.Empty;
            httpStatusCode = HttpStatusCode.OK;

            if (building is null)
            {
                message = "Building is null.";
                httpStatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            return isValid;
        }

        /// <summary>
        /// Verifies that a given user has valid properties.
        /// </summary>
        /// <param name="user">User to verify.</param>
        /// <param name="message">The error message for a failed validation.</param>
        /// <param name="httpStatusCode">The status code for a failed validation.</param>
        /// <returns>True if the user is valid, otherwise false.</returns>
        public static bool IsUserValid(User user, out string message, out HttpStatusCode httpStatusCode)
        {
            bool isValid = true;
            message = string.Empty;
            httpStatusCode = HttpStatusCode.OK;

            if (user is null)
            {
                message = "User is null.";
                httpStatusCode = HttpStatusCode.BadRequest;
                isValid = false;
            }

            return isValid;
        }
    }
}
