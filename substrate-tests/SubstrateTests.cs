using NUnit.Framework;
using substrate_shared.Defaults;
using substrate_shared.Defaults.Policies;
using substrate_shared.enums;
using substrate_shared.structs;
using substrate_shared.types.core;

namespace substrate_tests
{
    [TestFixture]
    public sealed class SubstrateTests
    {
        private CycleRunner _runner;

        [SetUp]
        public void Setup()
        {
            var bias = BiasVector.Zero;
            _runner = new CycleRunner(
                bias,
                new DefaultResolutionLens(),
                new DefaultArtifactFactory(),
                new DefaultTraitResolver(),
                new DefaultBiasDriftPolicy(new DriftParameters())
            );
        }

        [Test]
        public void SilentCycle_WhenMagnitudeZero_ReturnsSilentArtifact()
        {
            var input = new MoodVector(0, 0, 0, 0, 0, 0, 0);
            var artifact = _runner.RunCycle(input);

            Assert.AreEqual(ArtifactType.SilentCycle, artifact.Type);
        }

        [Test]
        public void Collapse_WhenMagnitudeHigh_ReturnsCollapseArtifact()
        {
            var input = new MoodVector(10, 0, 0, 0, 0, 0, 0);
            var artifact = _runner.RunCycle(input);

            Assert.AreEqual(ArtifactType.CatastrophicCollapse, artifact.Type);
        }
        
        [Test]
        public void BiasDrift_DeterministicApply_ProducesNonZero()
        {
            // 1) Known inputs
            var before  = BiasVector.Zero;
            var delta   = new BiasDelta(5f, 2f, 1f, 0f, 0f, 0f, 0f);
            var layers  = new ResolutionLayers(dominant: 0.5f, hidden: 0.0f, secondary: 0.0f); // adjust to your struct shape
            var artifact = new ArtifactCore(ArtifactType.Pearl);
            float magnitude = 8f;

            // 2) Strong parameters to guarantee alpha > 0
            var p = new DriftParameters(
                commonBase: 0.2f, commonSlope: 0.10f,
                rareBase: 0.15f,  rareSlope: 0.10f,
                chaosBase: 0.12f, chaosSlope: 0.10f,
                collapseBase: 0.2f, collapseSlope: 0.15f,
                dominantEmphasis: 0.2f
            );

            var policy = new DefaultBiasDriftPolicy(p);

            // 3) Apply policy directly
            var after = policy.Apply(before, delta, magnitude, artifact, layers);

            // 4) Assert with component-level tolerance to avoid float traps
            Assert.That(after.Resonance, Is.Not.EqualTo(0f).Within(0f));  // must change
            Assert.That(after.Warmth,    Is.Not.EqualTo(0f).Within(0f));  // must change
            Assert.That(after.Irony,     Is.Not.EqualTo(0f).Within(0f));  // must change
        }

        [Test]
        public void ChaosCrystal_WhenHiddenHigh_ReturnsChaosArtifact()
        {
            // Hidden axis dominant, others zero
            var input = new MoodVector(0, 0, 0, 0, 0, 0, 3.0f);
            var artifact = _runner.RunCycle(input);

            Assert.AreEqual(ArtifactType.ChaosCrystal, artifact.Type);
        }



        


        #region Summary
        // Signature: SubstrateTests.RunCycle() validations -> NUnit tests
        // substrate-tests/ PRISMx Version: 1.1 | JSType.Date: 2025.111.28
        // Tags: #tests #valsubstrate-testson #substrate
        #endregion
    }
}