﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainmentService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
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

namespace CDP4WebServices.API.Services.CherryPick
{
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.Extensions;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The <see cref="ContainmentService" /> provides capabilities to retrieve containment on
    /// <see cref="Thing" />
    /// </summary>
    public class ContainmentService : IContainmentService
    {
        /// <summary>
        /// Queries contained <see cref="Thing" /> where the <see cref="ClassKind" /> is defined by one the
        /// <see cref="ClassKind" />
        /// </summary>
        /// <param name="containers">A <see cref="IReadOnlyList{T}" /> of <see cref="Thing" /> containers</param>
        /// <param name="source">A <see cref="IReadOnlyList{T}" /> of all <see cref="Thing" /></param>
        /// <param name="queryDeep">Value asserting that the query have to make deep search on containment</param>
        /// <param name="classKind">A collection of <see cref="ClassKind" /> that should matches</param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        public IEnumerable<Thing> QueryContainedThings(IReadOnlyList<Thing> containers, IReadOnlyList<Thing> source, bool queryDeep, params ClassKind[] classKind)
        {
            var allRetrievedThings = new List<Thing>();
            List<Thing> containedThings;

            do
            {
                containedThings = source.Where(x => classKind.Contains(x.ClassKind) && containers.Any(c => c.Contains(x))
                                                                                    && containers.All(c => c.Iid != x.Iid))
                    .DistinctBy(x => x.Iid)
                    .Where(x => allRetrievedThings.All(a => a.Iid != x.Iid))
                    .ToList();

                allRetrievedThings.AddRange(containedThings);
                containers = containedThings;
            } while (containedThings.Any() && queryDeep);

            return allRetrievedThings;
        }

        /// <summary>
        /// Retrieve the containers tree for a <see cref="Thing" />
        /// </summary>
        /// <param name="containedThing">A <see cref="Thing" /></param>
        /// <param name="allThings">A collection of <see cref="Thing" /> to retrieve the containers tree</param>
        /// <returns>The retrieved container tree</returns>
        public IEnumerable<Thing> QueryContainersTree(Thing containedThing, IReadOnlyList<Thing> allThings)
        {
            var tree = new List<Thing>();
            var container = allThings.FirstOrDefault(x => x.Contains(containedThing));

            while (container != null)
            {
                tree.Add(container);
                containedThing = container;
                container = allThings.FirstOrDefault(x => x.Contains(containedThing));
            }

            return tree;
        }

        /// <summary>
        /// Retrieve the containers tree for a collection of <see cref="Thing" />
        /// </summary>
        /// <param name="containedThings">A collection of <see cref="Thing" /></param>
        /// <param name="allThings">A collection of <see cref="Thing" /> to retrieve the containers tree</param>
        /// <returns>The retrieved container tree</returns>
        public IEnumerable<Thing> QueryContainersTree(IReadOnlyList<Thing> containedThings, IReadOnlyList<Thing> allThings)
        {
            var tree = new List<Thing>();

            foreach (var containedThing in containedThings)
            {
                tree.AddRange(this.QueryContainersTree(containedThing, allThings).Where(x => tree.All(t => x.Iid != t.Iid)));
            }

            return tree;
        }
    }
}
