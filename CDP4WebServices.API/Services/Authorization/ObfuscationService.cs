﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObfuscationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Adrian Chivu, Cozmin Velciu, Alex Vorobiev
// 
//    This file is part of CDP4-Server-Administration-Tool.
//    The CDP4-Server-Administration-Tool is an ECSS-E-TM-10-25 Compliant tool
//    for advanced server administration.
// 
//    The CDP4-Server-Administration-Tool is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation; either version 3 of the
//    License, or (at your option) any later version.
// 
//    The CDP4-Server-Administration-Tool is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using CDP4WebServices.API.Services.Authentication;

    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterGroup = CDP4Common.DTO.ParameterGroup;
    using ParameterOverride = CDP4Common.DTO.ParameterOverride;
    using ParameterOverrideValueSet = CDP4Common.DTO.ParameterOverrideValueSet;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;
    using ParameterValueSetBase = CDP4Common.DTO.ParameterValueSetBase;

    /// <summary>
    /// The obfuscation service obscures properties and children of Element Definitions based on OrganizationalParticipation of an EngineeringModelSetup
    /// </summary>
    public class ObfuscationService : IObfuscationService
    {
        /// <summary>
        /// Obfuscates the entire response
        /// </summary>
        /// <param name="resourceResponse">The list of all <see cref="Thing" /> contained in the response.</param>
        /// <param name="credentials">The <see cref="Credentials" /></param>
        public void ObfuscateResponse(List<Thing> resourceResponse, Credentials credentials)
        {
            if (credentials.IsDefaultOrganizationalParticipant)
            {
                // if member of the defaultorganization for engineering model, do nothing
                return;
            }

            // gather all parts of the response once
            var elementDefinitions = resourceResponse.OfType<ElementDefinition>().ToList();
            var elementUsages = resourceResponse.OfType<ElementUsage>().ToList();
            var parameters = resourceResponse.OfType<Parameter>().ToList();
            var parameterOverrides = resourceResponse.OfType<ParameterOverride>().ToList();
            var parameterSubscriptions = resourceResponse.OfType<ParameterSubscription>().ToList();
            var parameterValueSets = resourceResponse.OfType<ParameterValueSet>().ToList();
            var parameterSubscriptionValueSets = resourceResponse.OfType<ParameterSubscriptionValueSet>().ToList();
            var parameterOverrideValueSets = resourceResponse.OfType<ParameterOverrideValueSet>().ToList();
            var parameterGroups = resourceResponse.OfType<ParameterGroup>().ToList();
            var definitions = resourceResponse.OfType<Definition>().ToList();
            var citations = resourceResponse.OfType<Citation>().ToList();

            if (credentials.OrganizationalParticipant == null)
            {
                // the member is not part of any included organizations and thus all element definitions are obscured
                foreach (var elementDefinition in elementDefinitions)
                {
                    this.ObfuscateElementDefinition(elementDefinition, parameters, parameterSubscriptions, parameterValueSets, parameterSubscriptionValueSets, elementUsages, parameterOverrides, parameterOverrideValueSets, parameterGroups, new List<Guid>(), definitions, citations);
                }

                return;
            }

            var forbiddenElementDefinitions = elementDefinitions.Where(ed => !ed.OrganizationalParticipant.Contains(credentials.OrganizationalParticipant.Iid));
            var allowedElementDefinitions = elementDefinitions.Where(ed => ed.OrganizationalParticipant.Contains(credentials.OrganizationalParticipant.Iid)).Select(ed => ed.Iid).ToList();

            foreach (var forbiddenElementDefinition in forbiddenElementDefinitions)
            {
                this.ObfuscateElementDefinition(forbiddenElementDefinition, parameters, parameterSubscriptions, parameterValueSets, parameterSubscriptionValueSets, elementUsages, parameterOverrides, parameterOverrideValueSets, parameterGroups, allowedElementDefinitions, definitions, citations);
            }
        }

        /// <summary>
        /// Obfuscates an <see cref="CDP4Common.DTO.ElementDefinition" /> object
        /// </summary>
        /// <param name="thing">The <see cref="CDP4Common.DTO.ElementDefinition" /> to obfuscate.</param>
        /// <param name="parameters">The list of all <see cref="Parameter" />s in the response</param>
        /// <param name="parameterSubscriptions">The list of all <see cref="ParameterSubscription" />s in the response</param>
        /// <param name="parameterValueSets">The list of all <see cref="ParameterValueSet" />s in the response</param>
        /// <param name="parameterSubscriptionValueSets">
        /// The list of all <see cref="ParameterSubscriptionValueSet" />s in the
        /// response
        /// </param>
        /// <param name="usages">The list of all <see cref="ElementUsage" />s in the response</param>
        /// <param name="parameterOverrides">The list of all <see cref="ParameterOverride" />s in the response</param>
        /// <param name="parameterOverrideValueSets">The list of all <see cref="ParameterOverrideValueSet" />s in the response</param>
        /// <param name="parameterGroups">The list of all <see cref="ParameterGroup" />s in the response</param>
        /// <param name="allowedElementDefinitions">The list of allowed <see cref="ElementDefinition" /> Iids</param>
        /// <param name="definitions">The list of all <see cref="Definition" />s in the response</param>
        /// <param name="citations">The list of all <see cref="Citation" />s in the response</param>
        private void ObfuscateElementDefinition(
            ElementDefinition thing,
            List<Parameter> parameters,
            List<ParameterSubscription> parameterSubscriptions,
            List<ParameterValueSet> parameterValueSets,
            List<ParameterSubscriptionValueSet> parameterSubscriptionValueSets,
            List<ElementUsage> usages,
            List<ParameterOverride> parameterOverrides,
            List<ParameterOverrideValueSet> parameterOverrideValueSets,
            List<ParameterGroup> parameterGroups,
            List<Guid> allowedElementDefinitions,
            List<Definition> definitions,
            List<Citation> citations)
        {
            // obfuscate own properties
            thing.Name = "Hidden Element Definition";
            thing.ShortName = "hiddenElementDefinition";

            // obfuscate all parameter values
            foreach (var paramIid in thing.Parameter)
            {
                var paramDto = parameters.FirstOrDefault(p => p.Iid.Equals(paramIid));

                if (paramDto == null)
                {
                    continue;
                }

                var subscriptions = paramDto.ParameterSubscription;
                var paramValueSetIids = paramDto.ValueSets;

                foreach (var paramValueSet in paramValueSetIids)
                {
                    var set = parameterValueSets.FirstOrDefault(pvs => pvs.Iid.Equals(paramValueSet));

                    if (set != null)
                    {
                        this.ObfuscateValueSet(set);
                    }
                }

                foreach (var subscriptionIid in subscriptions)
                {
                    var subscription = parameterSubscriptions.FirstOrDefault(s => s.Iid.Equals(subscriptionIid));

                    if (subscription == null)
                    {
                        continue;
                    }

                    var paramSubscriptionValueSetIids = subscription.ValueSets;

                    foreach (var paramValueSet in paramSubscriptionValueSetIids)
                    {
                        var set = parameterSubscriptionValueSets.FirstOrDefault(psvs => psvs.Iid.Equals(paramValueSet));

                        if (set != null)
                        {
                            this.ObfuscateSubscriptionValueSet(set);
                        }
                    }
                }
            }

            // obfuscate usages
            foreach (var usageIid in thing.ContainedElement)
            {
                var usage = usages.FirstOrDefault(u => u.Iid.Equals(usageIid));

                // if no usage in response or the usage is of an element that is allowed, skip
                if (usage == null || allowedElementDefinitions.Contains(usage.ElementDefinition))
                {
                    continue;
                }

                this.ObfuscateElementUsage(usage, parameterOverrides, parameterOverrideValueSets, definitions, citations);
            }

            // obfuscate definitions
            foreach (var definitionIid in thing.Definition)
            {
                var definition = definitions.FirstOrDefault(d => d.Iid.Equals(definitionIid));

                if (definition == null)
                {
                    continue;
                }

                this.ObfuscateDefinition(definition, citations);
            }

            // obfuscate parameter groups
            foreach (var groupIid in thing.ParameterGroup)
            {
                var group = parameterGroups.FirstOrDefault(g => g.Iid.Equals(groupIid));

                if (group == null)
                {
                    continue;
                }

                this.ObfuscateParameterGroup(group);
            }
        }

        /// <summary>
        /// Obfuscates an <see cref="CDP4Common.EngineeringModelData.ParameterGroup" /> object
        /// </summary>
        /// <param name="thing">The <see cref="Citation" /> to obfuscate.</param>
        private void ObfuscateParameterGroup(ParameterGroup thing)
        {
            thing.Name = "Hidden Group";
        }

        /// <summary>
        /// Obfuscates an <see cref="CDP4Common.DTO.ElementUsage" /> object
        /// </summary>
        /// <param name="thing">The <see cref="CDP4Common.DTO.ElementUsage" /> to obfuscate.</param>
        /// <param name="parameterOverrides">The list of all <see cref="ParameterOverride" />s in the response</param>
        /// <param name="parameterOverrideValueSets">The list of all <see cref="ParameterOverrideValueSet" />s in the response</param>
        /// <param name="definitions">The list of all <see cref="Definition" />s in the response</param>
        /// <param name="citations">The list of all <see cref="Citation" />s in the response</param>
        private void ObfuscateElementUsage(ElementUsage thing,
            List<ParameterOverride> parameterOverrides,
            List<ParameterOverrideValueSet> parameterOverrideValueSets,
            List<Definition> definitions,
            List<Citation> citations)
        {
            thing.Name = "Hidden Element Usage";
            thing.ShortName = "hiddenElementUsage";

            // obfuscate all parameter values
            foreach (var paramIid in thing.ParameterOverride)
            {
                var paramDto = parameterOverrides.FirstOrDefault(p => p.Iid.Equals(paramIid));

                if (paramDto == null)
                {
                    continue;
                }

                var paramValueSetIids = paramDto.ValueSets;

                foreach (var paramValueSet in paramValueSetIids)
                {
                    var set = parameterOverrideValueSets.FirstOrDefault(pvs => pvs.Iid.Equals(paramValueSet));

                    if (set != null)
                    {
                        this.ObfuscateValueSet(set);
                    }
                }
            }

            // obfuscate definitions
            foreach (var definitionIid in thing.Definition)
            {
                var definition = definitions.FirstOrDefault(d => d.Iid.Equals(definitionIid));

                if (definition == null)
                {
                    continue;
                }

                this.ObfuscateDefinition(definition, citations);
            }
        }

        /// <summary>
        /// Obfuscates an <see cref="Definition" /> object
        /// </summary>
        /// <param name="thing">The <see cref="Definition" /> to obfuscate.</param>
        /// <param name="citations">The list of all <see cref="Citation" />s in the response</param>
        private void ObfuscateDefinition(Definition thing, List<Citation> citations)
        {
            thing.Content = "Hidden Definition";

            // obfuscate citations
            foreach (var citationIid in thing.Citation)
            {
                var citation = citations.FirstOrDefault(d => d.Iid.Equals(citationIid));

                if (citation == null)
                {
                    continue;
                }

                this.ObfuscateCitation(citation);
            }
        }

        /// <summary>
        /// Obfuscates an <see cref="Citation" /> object
        /// </summary>
        /// <param name="thing">The <see cref="Citation" /> to obfuscate.</param>
        private void ObfuscateCitation(Citation thing)
        {
            thing.ShortName = "Hidden Citation";
            thing.Location = "Hidden Location";
            thing.Remark = "Hidden Remark";
        }

        /// <summary>
        /// Obfuscates an <see cref="CDP4Common.DTO.ParameterValueSetBase" /> object
        /// </summary>
        /// <param name="thing">The <see cref="CDP4Common.DTO.ParameterValueSetBase" /> to obfuscate.</param>
        private void ObfuscateValueSet(ParameterValueSetBase thing)
        {
            var valueArrayCount = thing.Manual.Count;

            var newList = new List<string>();

            for (var i = 0; i < valueArrayCount; i++)
            {
                newList.Add("-");
            }

            var newArray = new ValueArray<string>(newList);

            thing.Manual = newArray;
            thing.Reference = newArray;
            thing.Computed = newArray;
            thing.Published = newArray;
            thing.Formula = newArray;
            thing.ValueSwitch = ParameterSwitchKind.MANUAL;
        }

        /// <summary>
        /// Obfuscates an <see cref="CDP4Common.DTO.ParameterSubscriptionValueSet" /> object
        /// </summary>
        /// <param name="thing">The <see cref="CDP4Common.DTO.ParameterValueSetBase" /> to obfuscate.</param>
        private void ObfuscateSubscriptionValueSet(ParameterSubscriptionValueSet thing)
        {
            var valueArrayCount = thing.Manual.Count;

            var newList = new List<string>();

            for (var i = 0; i < valueArrayCount; i++)
            {
                newList.Add("-");
            }

            var newArray = new ValueArray<string>(newList);

            thing.Manual = newArray;
            thing.ValueSwitch = ParameterSwitchKind.MANUAL;
        }
    }
}
