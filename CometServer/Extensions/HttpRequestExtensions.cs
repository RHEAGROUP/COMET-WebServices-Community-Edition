﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="RHEA System S.A.">
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

namespace CometServer.Extensions
{
    using System.Linq;

    using CometServer.Services;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Static extensioin methods for the <see cref="HttpRequest"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Queries the <see cref="ContentTypeKind"/>
        /// </summary>
        /// <param name="httpRequest">
        /// The subject <see cref="HttpRequest"/>
        /// </param>
        /// <returns>
        /// a <see cref="ContentTypeKind"/> value based on the <see cref="HttpRequest.ContentType"/>
        /// </returns>
        public static ContentTypeKind QueryContentTypeKind(this HttpRequest httpRequest)
        {
            var contentTypeKind = ContentTypeKind.JSON;

            if (httpRequest.Headers.Accept.Any(x => x.Contains(HttpConstants.MimeTypeMessagePack)))
            {
                contentTypeKind = ContentTypeKind.MESSAGEPACK;
            }

            return contentTypeKind;
        }
    }
}
