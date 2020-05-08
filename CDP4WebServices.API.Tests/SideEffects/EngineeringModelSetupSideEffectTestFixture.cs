﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;    
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4Authentication;
    using CDP4Orm.Dao;
    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using CDP4WebServices.API.Services.Protocol;
    using Moq;
    using Npgsql;
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="EngineeringModelSetupSideEffect"/>
    /// </summary>
    [TestFixture]
    internal class EngineeringModelSetupSideEffectTestFixture
    {
        private Credentials credentials;
        private AuthenticationPerson authenticatedPerson;
        private Mock<ICdp4RequestContext> cdp4RequestContext;
        private EngineeringModelSetupSideEffect engineeringModelSetupSideEffect;
        private ModelCreatorManager modelCreatorManager;

        private Mock<IRevisionService> revisionService;
        private Mock<IIterationService> iterationService;
        private Mock<IIterationSetupService> iterationSetupService;
        private Mock<IEngineeringModelService> engineeringModelService;
        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;
        private Mock<IParticipantService> participantService;
        private Mock<IOptionService> optionService;
        private Mock<ISecurityContext> securityContext;
        private Mock<IPersonResolver> personResolver;
        private Mock<IPermissionService> permissionService;
        private Mock<IEngineeringModelDao> engineeringModelDao;

        private SiteDirectory siteDirectory;
        private NpgsqlTransaction npgsqlTransaction;
        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };
        private EngineeringModelSetup engineeringModelSetup;

        [SetUp]
        public void Setup()
        {
            var configurationFilePath = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "config.json");

            AppConfig.Load(configurationFilePath);

            this.authenticatedPerson = new AuthenticationPerson(Guid.NewGuid(), 1) { DefaultDomain = Guid.NewGuid() };
            this.credentials = new Credentials() { Person = this.authenticatedPerson };
            this.cdp4RequestContext = new Mock<ICdp4RequestContext>();
            this.cdp4RequestContext.Setup(x => x.AuthenticatedCredentials).Returns(this.credentials);
            this.requestUtils.Context = this.cdp4RequestContext.Object;

            this.iterationService = new Mock<IIterationService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.engineeringModelService = new Mock<IEngineeringModelService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.participantService = new Mock<IParticipantService>();
            this.optionService = new Mock<IOptionService>();
            this.securityContext = new Mock<ISecurityContext>();
            this.revisionService = new Mock<IRevisionService>();
            this.personResolver = new Mock<IPersonResolver>();
            this.permissionService = new Mock<IPermissionService>();
            this.engineeringModelDao = new Mock<IEngineeringModelDao>();
            this.npgsqlTransaction = null;

            this.modelCreatorManager = new ModelCreatorManager();
            this.modelCreatorManager.RevisionService = this.revisionService.Object;

            this.engineeringModelSetupSideEffect = new EngineeringModelSetupSideEffect
            {
                EngineeringModelService = this.engineeringModelService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                IterationService = this.iterationService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                ParticipantService = this.participantService.Object,
                OptionService = this.optionService.Object,
                RequestUtils = this.requestUtils,
                ModelCreatorManager = this.modelCreatorManager,
                PersonResolver = this.personResolver.Object,
                PermissionService = this.permissionService.Object,
                RevisionService = this.revisionService.Object,
                EngineeringModelDao = this.engineeringModelDao.Object
            };

            this.siteDirectory = new SiteDirectory(Guid.NewGuid(), 1);
            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 1);
            var domainOfExpertise = new DomainOfExpertise(Guid.NewGuid(), 1);
            var domainOfExpertise1 = new DomainOfExpertise(Guid.NewGuid(), 1);
            var domainFileStore = new DomainFileStore(Guid.NewGuid(), 1);
            domainFileStore.Owner = domainOfExpertise.Iid;
            var domainFileStore1 = new DomainFileStore(Guid.NewGuid(), 1);
            domainFileStore1.Owner = domainOfExpertise1.Iid;
            var option = new Option(Guid.NewGuid(), 1)
            {
                Name = "Option 1",
                ShortName = "optioin_1"
            };

            this.engineeringModelSetup.ActiveDomain.Add(domainOfExpertise.Iid);
            var engineeringModel = new EngineeringModel(this.engineeringModelSetup.EngineeringModelIid, 1);
            var iteration = new Iteration(Guid.NewGuid(), 1);
            iteration.DomainFileStore.Add(domainFileStore.Iid);

            var orderedOption = new OrderedItem
            {
                K = 1,
                V = option.Iid
            };
            iteration.Option.Add(orderedOption);

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 1);
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);

            var participant = new Participant(Guid.NewGuid(), 1);
            this.engineeringModelSetup.Participant.Add(participant.Iid);

            engineeringModel.Iteration.Add(iteration.Iid);
            iterationSetup.IterationIid = iteration.Iid;
            this.siteDirectory.Model.Add(this.engineeringModelSetup.Iid);
            this.siteDirectory.DefaultParticipantRole = Guid.NewGuid();

            this.engineeringModelService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<EngineeringModel>(), It.IsAny<Thing>(), -1)).Returns(true);
            this.engineeringModelService.Setup(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<EngineeringModel>(), null)).Returns(true);
            this.engineeringModelService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).Returns(new[] { engineeringModel });

            this.engineeringModelSetupService.Setup(x => x.UpdateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<EngineeringModelSetup>(), It.IsAny<Thing>())).Returns(true);

            this.iterationService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>(), -1)).Returns(true);
            this.iterationService.Setup(
                x =>
                x.GetShallow(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    this.securityContext.Object)).Returns(new[] { iteration });
            this.iterationSetupService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IterationSetup>(), It.IsAny<EngineeringModelSetup>(), -1)).Returns(true);
            this.iterationSetupService.Setup(
                x =>
                x.GetShallow(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    this.securityContext.Object)).Returns(new[] { iterationSetup });

            this.participantService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Participant>(), It.IsAny<EngineeringModelSetup>(), -1)).Returns(true);
            this.participantService.Setup(
                x =>
                x.GetShallow(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    this.securityContext.Object)).Returns(new[] { participant });

            this.optionService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Option>(), It.IsAny<Iteration>(), -1)).Returns(true);
            this.optionService.Setup(x => x.GetShallow(
                It.IsAny<NpgsqlTransaction>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(),
                this.securityContext.Object)).Returns(new[] { option });

            this.revisionService.Setup(
                x => x.SaveRevisions(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<int>())).Returns(new List<Thing>());

            this.engineeringModelDao.Setup(x => x.GetNextIterationNumber(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>())).Returns(1);
        }

        [Test]
        public void VerifyAfterUpdate()
        {
            var originalThing = this.engineeringModelSetup.DeepClone<Thing>();
            this.engineeringModelSetupSideEffect.BeforeUpdate(this.engineeringModelSetup, this.siteDirectory, this.npgsqlTransaction, "siteDirectory", this.securityContext.Object, null);
            this.engineeringModelSetup.ActiveDomain.RemoveAt(0);
            var domainOfExpertise2 = new DomainOfExpertise(Guid.NewGuid(), 1);
            this.engineeringModelSetup.ActiveDomain.Add(domainOfExpertise2.Iid);
            
            this.engineeringModelSetupSideEffect.AfterUpdate(this.engineeringModelSetup, this.siteDirectory, originalThing, this.npgsqlTransaction, "siteDirectory", this.securityContext.Object);
        }

        [Test]
        public void VerifyAfterDelete()
        {
            var originalThing = this.engineeringModelSetup.DeepClone<Thing>();
            this.engineeringModelSetupSideEffect.AfterDelete(this.engineeringModelSetup, this.siteDirectory, originalThing, this.npgsqlTransaction, "siteDirectory", this.securityContext.Object);

            // Check that the engineering model is deleted in the newEngineeringModelPartition
            this.engineeringModelService.Verify(x => x.DeleteConcept(this.npgsqlTransaction, It.IsAny<string>(), It.IsAny<EngineeringModel>(), null), Times.Never);
        }

        [Test]
        public void VerifyThatCreateFromSourceWorks()
        {
            
        }
    }
}