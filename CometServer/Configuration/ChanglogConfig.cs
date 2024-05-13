﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangelogConfig.cs" company="Starion Group S.A.">
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
    using CDP4Common.DTO;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The change log configuration.
    /// </summary>
    public class ChangelogConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangelogConfig"/> class.
        /// </summary>
        public ChangelogConfig()
        {
            // set defaults
            this.CollectChanges = false;
            this.AllowEmailNotification = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangelogConfig"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        public ChangelogConfig(IConfiguration configuration)
        {
            this.CollectChanges = bool.Parse(configuration["Changelog:CollectChanges"]);
            this.AllowEmailNotification = bool.Parse(configuration["Changelog:AllowEmailNotification"]);
        }

        /// <summary>
        /// Gets or sets the Changelog collection setting.
        /// If set to true, <see cref="ModelLogEntry"/>s will automatically be created for specific changes to <see cref="Thing"/>s.
        /// </summary>
        /// <remarks>
        /// The default value is false
        /// </remarks>
        public bool CollectChanges { get; set; }

        /// <summary>
        /// Gets or sets the AllowEmailNotification setting.
        /// If set to true, periodical emails are automatically sent to person's that have subscriptions on them
        /// and have periodical emails set in their UserPreferences.
        /// </summary>
        /// <remarks>
        /// The default value is false
        /// </remarks>
        public bool AllowEmailNotification { get; set; }
    }
}
