﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;

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
        /// Gets or sets the query parameters.
        /// </summary>
        IQueryParameters QueryParameters { get; set; }

        /// <summary>
        /// Sets the override query parameters to be used for specific processing logic.
        /// set to null to use request query parameters
        /// </summary>
        IQueryParameters OverrideQueryParameters { set; }

        /// <summary>
        /// Gets the get request data model version.
        /// </summary>
        Version GetRequestDataModelVersion(HttpRequest httpRequest);

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