﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParticipantSideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class ParticipantSideEffectTestFixture
    {
        private const string SelectedDomainKey = "SelectedDomain";

        private Mock<ISecurityContext> securityContext;

        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;

        private Mock<IParticipantService> participantService;

        private Mock<IPersonService> personService;

        private NpgsqlTransaction npgsqlTransaction;

        private ClasslessDTO rawUpdateInfo;

        private ParticipantSideEffect participantSideEffect;

        private Participant participant;

        private EngineeringModelSetup originalEngineeringModelSetup;

        private EngineeringModelSetup engineeringModelSetup;
        private Person person;
        private Participant originalParticipant;

        [SetUp]
        public void SetUp()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.participantService = new Mock<IParticipantService>();
            this.personService = new Mock<IPersonService>();

            this.npgsqlTransaction = null;
            this.participantSideEffect = new ParticipantSideEffect();

            this.participantSideEffect.PersonService = this.personService.Object;
            this.participantSideEffect.EngineeringModelSetupService = this.engineeringModelSetupService.Object;
            this.participantSideEffect.ParticipantService = this.participantService.Object;

            this.person = new Person(Guid.NewGuid(), 0)
            {
                ShortName = "TestPerson"
            };

            this.participant = new Participant(Guid.NewGuid(), 0)
            {
                Person = this.person.Iid
            };

            this.participant.Domain.Add(Guid.NewGuid());

            this.originalParticipant = new Participant(Guid.NewGuid(), 0)
            {
                Person = this.person.Iid
            };

            this.originalParticipant.Domain.Add(Guid.NewGuid());

            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);

            this.originalEngineeringModelSetup = new EngineeringModelSetup(this.engineeringModelSetup.Iid, 0);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidOrNullSelectedDomain()
        {
            //null selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, null }
            };

            Assert.Throws<InvalidOperationException>(
                () =>
                    this.participantSideEffect.BeforeUpdate(
                        this.participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo));

            //invalid selected domain verification
            this.rawUpdateInfo = new ClasslessDTO()
            {
                { SelectedDomainKey, default }
            };

            Assert.Throws<InvalidOperationException>(
                () =>
                    this.participantSideEffect.BeforeUpdate(
                        this.participant,
                        null,
                        this.npgsqlTransaction,
                        "partition",
                        this.securityContext.Object,
                        this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenContainerIsNull()
        {
            Assert.Throws<Cdp4ModelValidationException>(
                () =>
                this.participantSideEffect.AfterCreate(this.participant, null, null, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenContainerIsNotAnEngineeringModelSetup()
        {
            Assert.Throws<Cdp4ModelValidationException>(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.participant, null, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsExceptionWhenEngineeringModelSetupIsNotFound()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(Array.Empty<Thing>());

            Assert.Throws<Cdp4ModelValidationException>(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenParticipantIsNotFound()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(new List<Thing> {this.engineeringModelSetup});

            this.participantService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(Array.Empty<Thing>());

            Assert.DoesNotThrow(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenNoNewParticipantIsAdded()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(new List<Thing> { this.engineeringModelSetup });

            this.participantService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(new List<Thing> { this.participant });

            Assert.DoesNotThrow(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateDoesNotThrowWhenNewParticipantIsAdded()
        {
            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(new List<Thing> { this.engineeringModelSetup });

            this.participantService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", this.originalEngineeringModelSetup.Participant, this.securityContext.Object)).Returns(new List<Thing> { this.participant });

            this.personService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new [] {this.participant.Person}, this.securityContext.Object)).Returns(Array.Empty<Thing>());

            Assert.DoesNotThrow(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );
        }

        [Test]
        public void VerifyThatAfterCreateThrowsWhenSameNewParticipantIsAddedButPersonIsNotFound()
        {
            this.engineeringModelSetup.Participant.Add(this.participant.Iid);
            this.engineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.originalEngineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(new List<Thing> { this.engineeringModelSetup });

            this.participantService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", this.engineeringModelSetup.Participant, this.securityContext.Object)).Returns(new List<Thing> { this.participant, this.originalParticipant });

            this.personService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.participant.Person }, this.securityContext.Object)).Returns(Array.Empty<Thing>());

            var ex = Assert.Throws<Cdp4ModelValidationException>(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );

            Assert.That(ex.Message.Contains($"{nameof(Person)} itself was not found."), Is.True);
        }

        [Test]
        public void VerifyThatAfterCreateThrowsWhenSameNewParticipantIsAdded()
        {
            this.engineeringModelSetup.Participant.Add(this.participant.Iid);
            this.engineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.originalEngineeringModelSetup.Participant.Add(this.originalParticipant.Iid);

            this.engineeringModelSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.originalEngineeringModelSetup.Iid }, this.securityContext.Object)).Returns(new List<Thing> { this.engineeringModelSetup });

            this.participantService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", this.engineeringModelSetup.Participant, this.securityContext.Object)).Returns(new List<Thing> { this.participant, this.originalParticipant });

            this.personService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", new[] { this.participant.Person }, this.securityContext.Object)).Returns(new List<Thing> {this.person});

            var ex = Assert.Throws<Cdp4ModelValidationException>(
                () =>
                    this.participantSideEffect.AfterCreate(this.participant, this.originalEngineeringModelSetup, this.participant, this.npgsqlTransaction, "partition", this.securityContext.Object)
            );

            Assert.That(ex.Message.Contains($"is already a {nameof(Participant)}"), Is.True);
        }
    }
}
