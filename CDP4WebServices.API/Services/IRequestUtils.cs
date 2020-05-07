﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4Common.Helpers;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Protocol;

    using Nancy;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The RequestUtils interface.
    /// </summary>
    public interface IRequestUtils
    {
        /// <summary>
        /// Gets or sets the cache for this request.
        /// </summary>
        List<Thing> Cache { get; set; }

        /// <summary>
        /// Gets or sets the meta info provider for this request.
        /// </summary>
        IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        IQueryParameters QueryParameters { get; set; }

        /// <summary>
        /// Sets the override query parameters to be used for specific processing logic.
        /// set to null to use request query parameters
        /// </summary>
        IQueryParameters OverrideQueryParameters { set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultPermissionProvider"/> for this request
        /// </summary>
        IDefaultPermissionProvider DefaultPermissionProvider { get; set; }
        
        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        ICdp4RequestContext Context { get; set; }

        /// <summary>
        /// Gets the get request data model version.
        /// </summary>
        Version GetRequestDataModelVersion { get; }

        /// <summary>
        /// Convenience method to return a Http <see cref="Response"/> object with serialized JSON content.
        /// </summary>
        /// <param name="jsonArray">
        /// A <see cref="JArray"/> instance that is to be serialized into the <see cref="Response"/>.
        /// </param>
        /// <returns>
        /// A HTTP <see cref="Response"/> that can be returned by a web service API endpoint.
        /// </returns>
        Response GetJsonResponse(JArray jsonArray);

        /// <summary>
        /// Construct the engineering model partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        string GetEngineeringModelPartitionString(Guid engineeringModelIid);

        /// <summary>
        /// Construct the iteration partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineeringModel id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        string GetIterationPartitionString(Guid engineeringModelIid);
    }
}
