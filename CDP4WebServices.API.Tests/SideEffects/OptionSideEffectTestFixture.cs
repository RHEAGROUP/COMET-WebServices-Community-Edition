﻿// <copyright file="OptionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using Moq;
    using Npgsql;
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="OptionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class OptionSideEffectTestFixture
    {
        private OptionSideEffect optionSideEffect;

        private Mock<IOptionService> optionService;

        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;

        private Mock<IEngineeringModelService> engineeringModelService;

        private Mock<IIterationService> iterationService;

        private Mock<IIterationSetupService> iterationSetupService;

        private Mock<ISecurityContext> securityContext;

        private Mock<IRequestUtils> requestUtils;

        private Iteration iteration;

        private IterationSetup iterationSetup;

        private Iteration updatedIteration;

        private Option option;

        private List<Option> options;

        private EngineeringModelSetup engineeringModelSetup;

        private EngineeringModel engineeringModel;

        private NpgsqlTransaction npgsqlTransaction;

        private readonly string iterationPartition = $"{CDP4Orm.Dao.Utils.IterationSubPartition}_partition";
        private readonly string engineeringModelPartition = $"{CDP4Orm.Dao.Utils.EngineeringModelPartition}_partition";

        [SetUp]
        public void SetUp()
        {
            this.optionService = new Mock<IOptionService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.iterationService = new Mock<IIterationService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.securityContext = new Mock<ISecurityContext>();
            this.engineeringModelService = new Mock<IEngineeringModelService>();
            this.requestUtils = new Mock<IRequestUtils>();
            this.npgsqlTransaction = null;


            this.optionSideEffect = new OptionSideEffect
            {
                OptionService = this.optionService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                IterationSetupService = this.iterationSetupService.Object
            };

            this.option = new Option(Guid.NewGuid(), 0);
            this.options = new List<Option>();
            this.optionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(options);

            //this.iteration = new Iteration(Guid.NewGuid(), 0);
            //this.iteration.IterationSetup = Guid.NewGuid();


            this.iterationSetup = new IterationSetup
            {
                Iid = Guid.NewGuid()
            };

            this.iteration = new Iteration
            {
                Iid = Guid.NewGuid(),
                IterationSetup = this.iterationSetup.Iid
            };

            this.updatedIteration = this.iteration.DeepClone<Iteration>();
            this.iterationSetup.IterationIid = this.iteration.Iid;

            this.engineeringModelSetup = new EngineeringModelSetup
            {
                Iid = Guid.NewGuid(),
                IterationSetup = new List<Guid> { this.iteration.IterationSetup }
            };

            this.engineeringModel = new EngineeringModel
            {
                Iid = Guid.NewGuid(),
                EngineeringModelSetup = this.engineeringModelSetup.Iid
            };
            this.engineeringModelSetup.EngineeringModelIid = this.engineeringModel.Iid;
        }

        [Test]
        public void Verify_that_when_an_iteration_contains_no_options_an_option_can_be_added()
        {
            Assert.That(this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null),
                Is.True);
        }

        [Test]
        public void Verify_that_after_delete_option_everything_an_EngineeringModel_is_a_catalogue_no_more_than_one_option_can_be_added()
        {
            this.options.Add(new Option(Guid.NewGuid(), 0));

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 0) { IterationIid = this.iteration.Iid };

            this.iterationSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<IterationSetup>() { iterationSetup });

            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            engineeringModelSetup.Kind = CDP4Common.SiteDirectoryData.EngineeringModelKind.MODEL_CATALOGUE;

            this.engineeringModelSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<EngineeringModelSetup>() { engineeringModelSetup });

            Assert.Throws<InvalidOperationException>(() =>
                this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null));
        }

        [Test]
        public void Verify_that_when_an_EngineeringModel_is_not_a_Catalogue_more_than_one_option_can_be_added()
        {
            this.options.Add(new Option(Guid.NewGuid(), 0));

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 0) { IterationIid = this.iteration.Iid };

            this.iterationSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<IterationSetup>() { iterationSetup });

            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            engineeringModelSetup.Kind = CDP4Common.SiteDirectoryData.EngineeringModelKind.STUDY_MODEL;

            this.engineeringModelSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<EngineeringModelSetup>() { engineeringModelSetup });

            Assert.That(this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null), Is.True);
        }

        [Test]
        public void
            Verify_that_reset_0f_default_option_is_not_set_and_not_saved_when_DefaultOption_is_deleted_and_DefautOption_was_already_reset_earlier()
        {
            this.SetupMethodCallsForDeleteOptionTest(SetupMethodCallsForOptionTestScenario.All);
            this.updatedIteration.DefaultOption = null;

            this.optionSideEffect.BeforeDelete(
                this.option,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.iteration.DefaultOption);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void Verify_that_reset_of_DefaultOption_is_not_set_and_not_saved_when_is_deleted()
        {

            this.SetupMethodCallsForDeleteOptionTest(SetupMethodCallsForOptionTestScenario.All);
            this.updatedIteration.DefaultOption = null;

            this.optionSideEffect.BeforeDelete(
                this.option,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNull(this.iteration.DefaultOption);

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test] public void Verify_that_reset_of_DefaultOption_is_set_and_saved_when_DefaultOption_is_deleted()
        {
            this.SetupMethodCallsForDeleteOptionTest(SetupMethodCallsForOptionTestScenario.All);

            this.optionSideEffect.BeforeDelete(
                this.option,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNull(this.updatedIteration.DefaultOption);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Once);
        }

        /// <summary>
        ///  Descriptive enum to present the <see cref="SetupMethodCallsForTopElementTest" /> method a self-descriptive value on
        ///  how fake method calls should be setup.
        ///  Uses Flags attribute for convenience
        ///  <See cref="https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute" />
        /// </summary>
        [Flags]
        private enum SetupMethodCallsForOptionTestScenario
        {
            IterationSetup = 1,
            EngineeringModelSetup = 2,
            Iteration = 4,
            EngineeringModel = 8,
            IterationSetupNotFound = EngineeringModelSetup | Iteration | EngineeringModel,
            EngineeringModelSetupNotFound = IterationSetup | Iteration | EngineeringModel,
            IterationNotFound = IterationSetup | EngineeringModelSetup | EngineeringModel,
            EngineeringModelNotFound = IterationSetup | EngineeringModelSetup | Iteration,
            All = IterationSetup | EngineeringModelSetup | Iteration | EngineeringModel
        }

        /// <summary>
        ///  Sets up fake method calls on mocked classes specifically for the unit tests that check
        ///  <see cref="Iteration.DefaultOption" /> handling when an <see cref="Option" /> is deleted.
        /// </summary>
        /// <param name="setupMethodCallsForOptionTestScenario">
        ///  The <see cref="SetupMethodCallsForOptionTestScenario" />
        ///  When an enum value flag is not set then the corresponding faked method call will NOT return any data.
        /// </param>
        private void SetupMethodCallsForDeleteOptionTest(
            SetupMethodCallsForOptionTestScenario setupMethodCallsForOptionTestScenario)
        {
            this.requestUtils
                .Setup(x => x.GetEngineeringModelPartitionString(this.engineeringModelSetup.EngineeringModelIid))
                .Returns(this.engineeringModelPartition);

            var iterationSetups = new List<Iteration>();
            if (setupMethodCallsForOptionTestScenario.HasFlag(SetupMethodCallsForOptionTestScenario
                .IterationSetup))
            {
                iterationSetups.Add(this.iteration);
            }

            this.iterationSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                    new[] { this.iteration.IterationSetup }, this.securityContext.Object))
                .Returns(iterationSetups);

            var engineeringModelSetups = new List<EngineeringModelSetup>();
            if (setupMethodCallsForOptionTestScenario.HasFlag(SetupMethodCallsForOptionTestScenario
                .EngineeringModelSetup))
            {
                engineeringModelSetups.Add(this.engineeringModelSetup);
            }

            this.engineeringModelSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                    null, this.securityContext.Object))
                .Returns(engineeringModelSetups);

            var newIterations = new List<Iteration>();
            if (setupMethodCallsForOptionTestScenario.HasFlag(SetupMethodCallsForOptionTestScenario.Iteration))
            {
                newIterations.Add(this.updatedIteration);
            }

            this.iterationService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.iteration.Iid }, this.securityContext.Object))
                .Returns(newIterations);

            var engineeringModels = new List<EngineeringModel>();

            if (setupMethodCallsForOptionTestScenario.HasFlag(SetupMethodCallsForOptionTestScenario
                .EngineeringModel))
            {
                engineeringModels.Add(this.engineeringModel);
            }

            this.engineeringModelService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.engineeringModelSetup.EngineeringModelIid }, this.securityContext.Object))
                .Returns(engineeringModels);
        }
    }
}