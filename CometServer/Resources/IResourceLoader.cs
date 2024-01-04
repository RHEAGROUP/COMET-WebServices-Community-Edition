﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceLoader.cs" company="RHEA System S.A.">
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

namespace CometServer.Resources
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defnition of the interface used to load (embedded) resources
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Load an embedded resource
        /// </summary>
        /// <param name="path">
        /// The path of the embedded resource
        /// </param>
        /// <returns>
        /// a string containing the contents of the embedded resource
        /// </returns>
        string LoadEmbeddedResource(string path);

        /// <summary>
        /// queries the version number from the executing assembly
        /// </summary>
        /// <returns>
        /// a string representation of the version of the application
        /// </returns>
        string QueryVersion();

        /// <summary>
        /// queries the supported model versions from the executing assembly
        /// </summary>
        /// <returns>
        /// a collection of string representations of the supported model versions of the application
        /// </returns>
        IEnumerable<Version> QueryModelVersions();

        /// <summary>
        /// queries the version number from the CDP4Common library
        /// </summary>
        /// <returns>
        /// a string representation of the version of the CDP4-COMET SDK
        /// </returns>
        string QuerySDKVersion();

        /// <summary>
        /// queries the template HTML of the root page
        /// </summary>
        /// <returns>
        /// a string representation of the template HTML of the root page
        /// </returns>
        string QueryRootPage();

        /// <summary>
        /// Queries the logo with version info from the embedded resources
        /// </summary>
        /// <returns>
        /// the logo
        /// </returns>
        string QueryLogo();
    }
}
