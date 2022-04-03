﻿using BEIMA.Backend.MongoService;
using System.Globalization;
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
        public static bool IsDeviceValid(Device device, DeviceType deviceType, out string message, out HttpStatusCode httpStatusCode)
        {
            var isValid = true;
            message = string.Empty;
            httpStatusCode = HttpStatusCode.OK;

            // Check that device is not null
            if (device is null)
            {
                message = Resources.DeviceNullMessage;
                httpStatusCode = HttpStatusCode.BadRequest;
                return false;
            }

            // Check year manufactured
            if (device.YearManufactured is not null && (
                device.YearManufactured < 0 ||
                device.YearManufactured.ToString().Length != 4)
                )
            {
                message += Resources.DeviceYearManufacturedInvalidMessage;
                httpStatusCode = HttpStatusCode.BadRequest;
                isValid = false;
            }

            // Check location
            if ((!string.IsNullOrEmpty(device.Location.Latitude) && !ValidateLatitude(device.Location.Latitude)) ||
                (!string.IsNullOrEmpty(device.Location.Longitude) && !ValidateLongitude(device.Location.Longitude)))
            {
                message += message.Length > 0 ? '\n' : string.Empty;
                message += Resources.InvalidLocationMessage;
                httpStatusCode = HttpStatusCode.BadRequest;
                isValid = false;
            }

            // Check mandatory field lengths
            foreach (var prop in device.GetType().GetProperties())
            {
                if (prop.PropertyType.Equals(typeof(string)) &&
                    prop.GetValue(device).ToString().Length > Constants.MAX_CHARACTER_LENGTH)
                {
                    message += message.Length > 0 ? '\n' : string.Empty;
                    message += string.Format(Resources.MaxCharacterLengthExceededMessage, prop.Name);
                    httpStatusCode = HttpStatusCode.BadRequest;
                    isValid = false;
                }
            }

            // Check device field lengths
            foreach (var field in device.Fields)
            {
                if (field.Value.Length > Constants.MAX_CHARACTER_LENGTH)
                {
                    message += message.Length > 0 ? '\n' : string.Empty;
                    message += string.Format(Resources.MaxCharacterLengthExceededMessage, deviceType.Fields[field.Key].AsString);
                    httpStatusCode = HttpStatusCode.BadRequest;
                    isValid = false;
                }
            }

            // Check location notes length
            if (device.Location.Notes is not null &&
                device.Location.Notes.Length > Constants.MAX_CHARACTER_LENGTH)
            {
                message += message.Length > 0 ? '\n' : string.Empty;
                message += string.Format(Resources.MaxCharacterLengthExceededMessage, "Location Notes");
                httpStatusCode = HttpStatusCode.BadRequest;
                isValid = false;
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

        /// <summary>
        /// Determines whether the given latitude value is valid.
        /// </summary>
        /// <param name="latitude">The latitude to validate.</param>
        /// <returns>True if the latitude was valid, otherwise false.</returns>
        private static bool ValidateLatitude(string latitude)
        {
            try
            {
                var lat = float.Parse(latitude, CultureInfo.InvariantCulture.NumberFormat);
                if (lat < -90.0 || lat > 90.0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the given longitude value is valid.
        /// </summary>
        /// <param name="longitude">The longitude to validate.</param>
        /// <returns>True if the longitude was valid, otherwise false.</returns>
        private static bool ValidateLongitude(string longitude)
        {
            try
            {
                var lon = float.Parse(longitude, CultureInfo.InvariantCulture.NumberFormat);
                if (lon < -180.0 || lon > 180.0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
