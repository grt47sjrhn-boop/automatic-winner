Here’s an updated version of your **Registry architecture overview README**, expanded to reflect the current state of PRISMx — including the fact that profiles and overlays are now part of the shared substrate alongside registries. This version makes it clear to contributors why static classes live here, how managers orchestrate them, and how profiles fit into the bigger picture.

---

# Registry Architecture Overview

This README explains how the per‑type Registry Manager works, what the Super Registry Manager adds on top, and how profiles and overlays extend the registry substrate. It’s written for contributors and critics to understand both the modular design and the cross‑axis orchestration layer.

---

## Core Concepts

- **Enum as semantic anchors:** Each registry enum (`ToneType`, `MoodType`, `IntentAction`, etc.) defines discrete values decorated with attributes that carry narrative metadata (description, bias, narrative group).
- **Interface‑driven access:** Enum values are wrapped by a struct (`RegistryValue<TEnum>`) that implements `IReadableRegistry`. This exposes metadata uniformly via `GetDescription()`, `GetBias()`, and `GetGroup()` without leaking enum details.
- **Separation of axes:** Each registry represents a distinct narrative axis. Keeping them separate preserves clarity, safety, and contributor extensibility.
- **Profiles as presets:** Static profile containers (e.g., `OpponentProfiles`) define bias/difficulty presets. Each profile is a normal class implementing `IOpponentProfile`, but exposed through static accessors for convenience.
- **Overlays as narratable math:** Geometry and trigonometric overlays translate bias vectors into narratable metrics (hypotenuse, area, cos/sin ratios). They live in `shared` because they are simulation primitives.

---

## Basic Registry Manager (Per‑Type)

The `RegistryManager<TEnum>` is a generic, per‑type manager. It initializes a static cache of all values in the given enum, each wrapped as `RegistryValue<TEnum>` so they can be accessed via `IReadableRegistry`.

### What it does

- **Caches all values of TEnum:** On first use, it enumerates `Enum.GetValues(typeof(TEnum))`, wraps each in `RegistryValue<TEnum>`, and stores them in a type‑specific cache.
- **Provides uniform access:**
    - `Get(TEnum value)` returns an `IReadableRegistry` wrapper for that enum value.
    - `GetAll()` returns all wrapped values for that enum type.
- **Cluster helpers:**
    - `GetByGroup(NarrativeGroup group)` filters values by narrative group (e.g., CoreJoy, Equilibrium).
    - `DescribeCluster(NarrativeGroup group)` returns a formatted summary with description and bias.

### Why it matters

- **Modularity:** Each axis (Tone, Mood, Intent) stays isolated and type‑safe.
- **Extensibility:** Contributors add enum values with attributes; the manager automatically surfaces them with no extra wiring.
- **Narratability:** Metadata (description, bias, group) is immediately accessible for diagnostics and overlays.

---

## Super Registry Manager (Cross‑Axis Orchestration)

The `SuperRegistryManager` sits above all per‑type managers. It does not merge registries; it overlays unified querying and orchestration while preserving the split. It’s the panoramic layer for contributors and critics.

### What it adds

- **Registration of multiple registries:** `Register<TEnum>()` pulls `RegistryManager<TEnum>.GetAll()` and stores references per type.
- **Unified sweeps:** Query across all registered registries.
    - `GetAll()` returns all entries across types.
    - `GetByBias(Bias bias)` filters by bias across types.
    - `GetByGroup(NarrativeGroup group)` filters by narrative group across types.
- **Grouped breakdowns:** `GetGroupedByRegistry(NarrativeGroup group)` returns a dictionary keyed by type, preserving the per‑axis split in outputs.
- **Triad overlays:** `DescribeTriad(ToneType tone, MoodType mood, IntentAction intent)` composes cross‑axis combinations (Tone + Mood + Intent) with a single formatted narrative block.
- **Bias scoring:** Classifies entries as Positive, Negative, Neutral, Leaning Positive/Negative, or Mixed (Conflict).
- **Cluster scoring:** `DescribeClusterWithScore(NarrativeGroup group)` returns group entries with an overall bias classification.
- **Random sampling:** Helpers to select random entries or clusters for demos, quickstarts, and onboarding.
- **Narrative overlays:** Concatenate descriptions to produce a single mythic string for presentations or logs.
- **Critic report generator:** `GenerateCriticReport()` assembles a full multi‑section report with bias overview, group summaries (with scores), and a sample triad.

---

## Profiles and Overlays

- **Profiles (`OpponentProfiles`):** Static containers expose named presets (`StoryMode`, `Challenge`, `Nightmare`, `Balanced`). Each preset is a normal `OpponentProfile` class implementing `IOpponentProfile`. This keeps profiles injectable if needed, but convenient to access statically.
- **Overlays (`GeometryOverlay`, `TrigOverlay`):** Instance‑based services implementing `IOverlay`. They compute narratable metrics (hypotenuse, area, cos/sin ratios, log/exp scaling) and are injected into resolvers and engines. They live in `shared` because they are simulation primitives, not adapter logic.

---

## Design Principles Upheld

- **Separation of concerns:** Per‑type managers handle registry internals; the super manager handles cross‑axis orchestration; overlays handle math; profiles handle presets.
- **Type safety and clarity:** Each enum remains its own domain. The super layer never blurs axes; it overlays queries and compositions.
- **Contributor empowerment:** Adding entries requires only defining enum members with attributes; adding profiles requires only defining a preset; overlays are reusable services.
- **Critic readiness:** Scored summaries, triads, and overlays make the system interpretable and presentable without custom glue code.

---

## Common Pitfalls and Fixes

- **Accessing the underlying enum value:** `RegistryManager<TEnum>.GetAll()` returns `IReadableRegistry`. If you need the raw enum, cast to `RegistryValue<TEnum>`.
- **Forgetting to register registries:** The super manager only knows about types you register. Call `Register<TEnum>()` for each axis you want in unified queries.
- **Assuming global caches:** Each `RegistryManager<TEnum>` maintains a per‑type static cache. This is intentional for isolation; the super manager aggregates them when asked.
- **Confusing static vs instance:** Profiles are static containers but normal classes underneath. Overlays are instance services and must be injected.

---

## Contributor Quickstart

1. **Add a new enum value with attributes**
   ```csharp
   public enum ToneType
   {
       [RegistryNarrative("New tone description...", Bias.Positive, NarrativeGroup.CoreJoy)]
       Radiant
   }
   ```
2. **Add a new profile preset**
   ```csharp
   public static IOpponentProfile Heroic => new OpponentProfile
   {
       CollapseBiasFactor = 1.0,
       RecoveryBiasFactor = 2.0,
       NeutralBiasFactor  = 0.5,
       MagnitudeVariance  = 1.1,
       DifficultyMultiplier = 1.2,
       AggressionNudge = false,
       Label = "Duel",
       Category = "Heroic"
   };
   ```
3. **Explore your addition**
   ```csharp
   SuperRegistryManager.Register<ToneType>(); // usually done at startup
   var joyCluster = SuperRegistryManager.GetByGroup(NarrativeGroup.CoreJoy);
   ```

---

## Final Note

Keep per‑type registries clean and narratable. Let the super manager do the heavy lifting when you need cross‑axis clarity, orchestration, and analytics. Profiles and overlays extend the substrate with presets and narratable math. This architecture keeps PRISMx resilient, extensible, and mythically coherent — every tick narratable, every overlay interpretable, every profile a mythic stance.
