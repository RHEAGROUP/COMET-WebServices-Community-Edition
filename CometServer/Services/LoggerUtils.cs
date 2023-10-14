﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using CDP4Authentication;

    /// <summary>
    /// The logger utils helper class
    /// </summary>
    public static class LoggerUtils
    {
        /// <summary>
        /// The unauthenticated subject constant.
        /// </summary>
        public const string UnauthenticatedSubject = "unauthenticated";

        /// <summary>
        /// The log success constant.
        /// </summary>
        public const string SuccesLog = "success";

        /// <summary>
        /// The log failure constant.
        /// </summary>
        public const string FailureLog = "failure";

        /// <summary>
        /// Construct a log message.
        /// </summary>
        /// <param name="subject">
        /// The authenticated subject (user) that triggered the log entry.
        /// </param>
        /// <param name="subjectHostAddress">
        /// The subject host address.
        /// </param>
        /// <param name="success">
        /// Successful request or not.
        /// </param>
        /// <param name="message">
        /// The log message
        /// </param>
        /// <returns>
        /// The formatted log entry.
        /// </returns>
        public static string GetLogMessage(
            string subject,
            string subjectHostAddress,
            bool success,
            string message)
        {
            return $"[{subject}{(!string.IsNullOrWhiteSpace(subjectHostAddress) ? $"@{subjectHostAddress}" : string.Empty)}] [{(success ? SuccesLog : FailureLog)}]|{message}";
        }

        /// <summary>
        /// Construct a log message.
        /// </summary>
        /// <param name="authenticationPerson">
        /// The authenticated subject (user) that triggered the log entry.
        /// </param>
        /// <param name="subjectHostAddress">
        /// The subject host address.
        /// </param>
        /// <param name="success">
        /// Successful request or not.
        /// </param>
        /// <param name="message">
        /// The log message.
        /// </param>
        /// <returns>
        /// The formatted log entry.
        /// </returns>
        public static string GetLogMessage(
            AuthenticationPerson authenticationPerson,
            string subjectHostAddress,
            bool success, 
            string message)
        {
            var subjectString = authenticationPerson == null
                                    ? UnauthenticatedSubject
                                    : $"{authenticationPerson.UserName}({authenticationPerson.Iid})";
            return GetLogMessage(subjectString, subjectHostAddress, success, message);
        }
    }
}
