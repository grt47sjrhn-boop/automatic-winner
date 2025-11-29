using substrate_shared.enums;

namespace substrate_shared.types.core
{
    /// <summary>
    /// Encodes substrate state transitions based on magnitude and artifact type.
    /// Used to gate contributor logic, trigger atmosphere shifts, or track collapse thresholds.
    /// </summary>
    public sealed class StateMachine
    {
        public SubstrateState CurrentState { get; private set; } = SubstrateState.Stable;

        public SubstrateState EvaluateNextState(float magnitude, ArtifactType artifactType)
        {
            if (artifactType == ArtifactType.CatastrophicCollapse)
                return TransitionTo(SubstrateState.Collapse);

            if (artifactType == ArtifactType.ChaosCrystal)
                return TransitionTo(SubstrateState.Crystallization);

            if (magnitude > 2.0f)
                return TransitionTo(SubstrateState.Stress);

            return TransitionTo(SubstrateState.Stable);
        }

        private SubstrateState TransitionTo(SubstrateState next)
        {
            CurrentState = next;
            return next;
        }

        public bool IsInCollapse() => CurrentState == SubstrateState.Collapse;
        public bool IsCrystallizing() => CurrentState == SubstrateState.Crystallization;
        public bool IsStressed() => CurrentState == SubstrateState.Stress;
        public bool IsStable() => CurrentState == SubstrateState.Stable;

        #region Summary
        // Signature: StateMachine.EvaluateNextState(float magnitude, ArtifactType artifactType) -> SubstrateState
        // PRISMx Version: 1.1 | Date: 2025.111.28
        // Tags: #substrate #state #transition
        #endregion
    }
}