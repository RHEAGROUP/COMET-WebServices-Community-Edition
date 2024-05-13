﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultsConfig.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The default properties configuration
    /// </summary>
    public class DefaultsConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultsConfig"/> class.
        /// </summary>
        public DefaultsConfig()
        {
            this.PersonPassword = "pass";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultsConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public DefaultsConfig(IConfiguration configuration)
        {
            this.PersonPassword = configuration["Defaults:PersonPassword"];
        }

        /// <summary>
        /// Gets or sets the default person password.
        /// </summary>
        /// <remarks>
        /// The default password to assign to new users
        /// </remarks>
        public string PersonPassword { get; set; }
    }
}