using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Descriptors.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract.Types;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Managers;
using substrate_shared.Strategies;
using substrate_shared.Transmitters;
using Xunit;

namespace substrate_tests
{
    [Collection("Registry collection")]
    public class IntentDescriptorTests
    {
        [Theory]
        [InlineData(IntentAction.Encourage, Bias.Positive, NarrativeGroup.SupportiveActions, 1, "Encourage", ToneType.Resilient)]
        [InlineData(IntentAction.Support, Bias.Positive, NarrativeGroup.SupportiveActions, 2, "Support", ToneType.Resilient)]
        [InlineData(IntentAction.Comfort, Bias.Positive, NarrativeGroup.SupportiveActions, 3, "Comfort", ToneType.Calm)]
        [InlineData(IntentAction.Approve, Bias.Positive, NarrativeGroup.AffirmativeActions, 4, "Approve", ToneType.Radiant)]
        [InlineData(IntentAction.Inspire, Bias.Positive, NarrativeGroup.AffirmativeActions, 5, "Inspire", ToneType.Joy)]
        [InlineData(IntentAction.Celebrate, Bias.Positive, NarrativeGroup.AffirmativeActions, 6, "Celebrate", ToneType.Radiant)]
        [InlineData(IntentAction.Observe, Bias.Neutral, NarrativeGroup.Equilibrium, 0, "Observe", ToneType.Equilibrium)]
        [InlineData(IntentAction.Inform, Bias.Neutral, NarrativeGroup.Equilibrium, 0, "Inform", ToneType.Reflective)]
        [InlineData(IntentAction.Question, Bias.Neutral, NarrativeGroup.Equilibrium, 0, "Question", ToneType.Equilibrium)]
        [InlineData(IntentAction.Reflect, Bias.Neutral, NarrativeGroup.Equilibrium, 0, "Reflect", ToneType.Composite)]
        [InlineData(IntentAction.Criticize, Bias.Negative, NarrativeGroup.HostileActions, -1, "Criticize", ToneType.Hostile)]
        [InlineData(IntentAction.Challenge, Bias.Negative, NarrativeGroup.HostileActions, -2, "Challenge", ToneType.Hostile)]
        [InlineData(IntentAction.Reject, Bias.Negative, NarrativeGroup.HostileActions, -3, "Reject", ToneType.Hostile)]
        [InlineData(IntentAction.Distract, Bias.Negative, NarrativeGroup.FracturedActions, -4, "Distract", ToneType.Forsaken)]
        [InlineData(IntentAction.Cynical, Bias.Negative, NarrativeGroup.HostileActions, -5, "Cynical", ToneType.Despairing)]
        public void FrameProvider_ShouldPopulateDescriptorAndToneCorrectly(
            IntentAction action,
            Bias expectedBias,
            NarrativeGroup expectedGroup,
            int expectedScale,
            string expectedNarrativeFragment,
            ToneType expectedToneType)
        {
            var provider = new SimulationFrameProvider(new ToneManager());
            var frame = provider.GenerateFrames().First(f => f.Intent!.IntentType == action);

            var descriptor = frame.Intent!;
            var strategy = frame.Strategy!;

            Assert.Equal(action, descriptor.IntentType);
            Assert.Equal(expectedBias, descriptor.Bias);
            Assert.Equal(expectedGroup, descriptor.Group);
            Assert.Equal(expectedScale, descriptor.ScaleValue);
            Assert.Contains(expectedNarrativeFragment, descriptor.Narrative);

            Assert.NotNull(strategy.Tone);
            Assert.Equal(expectedToneType, strategy.Tone!.Type);
        }

        [Fact]
        public void Strategy_ShouldCarryToneAndNarrative_FromDescriptor()
        {
            var descriptor = new IntentDescriptor
            {
                Id = Guid.NewGuid().ToString(),
                IntentType = IntentAction.Encourage,
                Bias = Bias.Positive,
                Group = NarrativeGroup.SupportiveActions,
                ScaleValue = 1,
                Narrative = "Encourage others",
                IntentTone = new NarrativeTone(ToneType.Resilient, "Resilient", NarrativeGroup.SupportiveActions, Bias.Positive)
            };

            var strategy = new IntentActionStrategy(descriptor);

            Assert.Equal(descriptor.Id, strategy.ActionId);
            Assert.Equal(descriptor.Type, strategy.Type);
            Assert.Equal(descriptor.IntentTone, strategy.Tone);
            Assert.Equal(descriptor.Narrative, strategy.Narrative);
        }

        public class ConsoleTransmitterTests
        {
            [Fact]
            public void Emit_ShouldWriteToneAndNarrative_ToConsole()
            {
                var descriptor = new IntentDescriptor
                {
                    Id = Guid.NewGuid().ToString(),
                    IntentType = IntentAction.Encourage,
                    Narrative = "Encourage: a torch raised in the dark...",
                    IntentTone = new NarrativeTone(ToneType.Resilient, "Resilient", NarrativeGroup.SupportiveActions, Bias.Positive)
                };
                var strategy = new IntentActionStrategy(descriptor);
                var transmitter = new ConsoleTransmitter();
                var report = new FakeReportSummary();

                using var sw = new StringWriter();
                Console.SetOut(sw);

                transmitter.Emit(strategy, report);

                var output = sw.ToString();
                Assert.Contains("Encourage", output);
                Assert.Contains("Resilient", output);
            }

            private class FakeReportSummary : IReportSummary
            {
                public void LogInfo(string message) => throw new NotImplementedException();
                public void LogWarning(string message) => throw new NotImplementedException();
                public void LogError(string message) => throw new NotImplementedException();
                public void LogException(Exception ex) => throw new NotImplementedException();
                public void LogValidationError(IValidationError error) => throw new NotImplementedException();
                public IReadOnlyList<string> GetMessages() => throw new NotImplementedException();
                public void Increment(DescriptorType type) { /* no-op */ }
            }
        }
    }
}