﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryChangeNotificationFilter.cs" company="Starion Group S.A.">
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

namespace CometServer.ChangeNotification.Notification
{
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;

    using CometServer.ChangeNotification.Subscription;

    /// <summary>
    /// Implements logic for Change notification filtering for a <see cref="Category"/>.
    /// </summary>
    public class CategoryChangeNotificationFilter : ChangeNotificationFilter
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CategoryChangeNotificationFilter"/> class.
        /// </summary>
        /// <param name="changeNotificationSubscription">
        /// The <see cref="ChangeNotificationSubscription"/> that resulted into this <see cref="IChangeNotificationFilter"/>.
        /// </param>
        /// <param name="domainOfExpertises">
        /// The <see cref="IChangeNotificationFilter"/>s where to filter on.
        /// </param>
        public CategoryChangeNotificationFilter(ChangeNotificationSubscription changeNotificationSubscription, IEnumerable<DomainOfExpertise> domainOfExpertises)
            : base(changeNotificationSubscription, domainOfExpertises)
        {
        }

        /// <summary>
        /// Checks if a <see cref="LogEntryChangelogItem"/> has certain specifics related to the <see cref="ChangeNotificationFilter.Iid"/>.
        /// </summary>
        /// <param name="logEntryChangelogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <returns>
        /// True is the specifics of the <see cref="LogEntryChangelogItem"/> match certain criteria, otherwise false.
        /// </returns>
        public override bool CheckFilter(LogEntryChangelogItem logEntryChangelogItem)
        {
            return logEntryChangelogItem.AffectedReferenceIid.Intersect(this.DomainOfExpertises.Select(x => x.Iid)).Any()
                   && logEntryChangelogItem.AffectedReferenceIid.Contains(this.Iid);
        }
    }
}
