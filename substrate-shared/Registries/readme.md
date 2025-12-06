# Registry architecture overview

This README explains how the per‑type Registry Manager works and what the Super Registry Manager adds on top. It’s written for contributors and critics to understand both the modular design and the cross‑axis orchestration layer.

---

## Core concepts

- **Enum as semantic anchors:** Each registry enum (ToneType, MoodType, IntentAction, etc.) defines discrete values decorated with attributes that carry narrative metadata (description, bias, narrative group).
- **Interface-driven access:** Enum values are wrapped by a struct (`RegistryValue<TEnum>`) that implements `IReadableRegistry`. This exposes metadata uniformly via `GetDescription()`, `GetBias()`, and `GetGroup()` without leaking enum details.
- **Separation of axes:** Each registry represents a distinct narrative axis. Keeping them separate preserves clarity, safety, and contributor extensibility.

---

## Basic registry manager (per‑type)

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

### Example

```csharp
var joy = RegistryManager<ToneType>.Get(ToneType.Joy);
Console.WriteLine(joy.GetDescription());
// "Joy rings like bells in a haunted city, fragile yet defiant against the void."

var coreJoy = RegistryManager<ToneType>.DescribeCluster(NarrativeGroup.CoreJoy);
Console.WriteLine(coreJoy);
```

---

## Super registry manager (cross‑axis orchestration)

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
    - Used for triads and clusters to give instant analytic clarity.
- **Cluster scoring:** `DescribeClusterWithScore(NarrativeGroup group)` returns group entries with an overall bias classification.
- **Random sampling:** Helpers to select random entries or clusters for demos, quickstarts, and onboarding.
- **Narrative overlays:** Concatenate descriptions to produce a single mythic string for presentations or logs.
- **Critic report generator:** `GenerateCriticReport()` assembles a full multi‑section report with bias overview, group summaries (with scores), and a sample triad.

### Why it matters

- **Panoramic clarity:** Critics and contributors can see the whole mythic palette at once without losing per‑axis identity.
- **Reusable orchestration:** Centralized helpers prevent brittle, copy‑pasted LINQ across types.
- **Narrative analytics:** Scoring makes combinations and clusters interpretable at a glance — no need to eyeball dozens of entries.

### Example

```csharp
// Register registries once
SuperRegistryManager.Register<ToneType>();
SuperRegistryManager.Register<MoodType>();
SuperRegistryManager.Register<IntentAction>();

// Unified query
var positives = SuperRegistryManager.GetByBias(Bias.Positive);

// Grouped breakdown
var groupedJoy = SuperRegistryManager.GetGroupedByRegistry(NarrativeGroup.CoreJoy);
foreach (var kvp in groupedJoy)
{
    Console.WriteLine($"{kvp.Key.Name}:");
    foreach (var entry in kvp.Value)
        Console.WriteLine($"- {entry.GetDescription()} ({entry.GetBias()})");
}

// Triad overlay with scoring
Console.WriteLine(SuperRegistryManager.DescribeTriad(ToneType.Joy, MoodType.Calm, IntentAction.Hostile));

// Cluster with score
Console.WriteLine(SuperRegistryManager.DescribeClusterWithScore(NarrativeGroup.Equilibrium));

// Critic report
Console.WriteLine(SuperRegistryManager.GenerateCriticReport());
```

---

## Design principles upheld

- **Separation of concerns:** Per‑type managers handle registry internals; the super manager handles cross‑axis orchestration.
- **Type safety and clarity:** Each enum remains its own domain. The super layer never blurs axes; it overlays queries and compositions.
- **Contributor empowerment:** Adding entries requires only defining enum members with attributes; everything else is automatic.
- **Critic readiness:** Scored summaries and triads make the system interpretable and presentable without custom glue code.

---

## Common pitfalls and fixes

- **Accessing the underlying enum value:** `RegistryManager<TEnum>.GetAll()` returns `IReadableRegistry`. If you need the raw enum, you must cast to `RegistryValue<TEnum>` because it’s a struct.
  ```csharp
  var toneEntry = RegistryManager<ToneType>.GetAll().First();
  var tone = ((RegistryValue<ToneType>)toneEntry).Value; // direct cast, not 'as'
  ```
- **Forgetting to register registries:** The super manager only knows about types you register. Call `Register<TEnum>()` for each axis you want in unified queries.
- **Assuming global caches:** Each `RegistryManager<TEnum>` maintains a per‑type static cache. This is intentional for isolation; the super manager aggregates them when asked.

---

## Contributor quickstart

1. **Add a new enum value with attributes**
   ```csharp
   public enum ToneType
   {
       [RegistryNarrative("New tone description...", Bias.Positive, NarrativeGroup.CoreJoy)]
       Radiant
   }
   ```
2. **No extra wiring needed**
    - The per‑type manager auto‑wraps it.
    - The super manager includes it once the type is registered.

3. **Explore your addition**
   ```csharp
   SuperRegistryManager.Register<ToneType>(); // usually done at startup
   var joyCluster = SuperRegistryManager.GetByGroup(NarrativeGroup.CoreJoy);
   ```

---

## Final note

Keep per‑type registries clean and narratable. Let the super manager do the heavy lifting when you need cross‑axis clarity, orchestration, and analytics. This architecture keeps PRISMx resilient, extensible, and mythically coherent — every tick narratable, every overlay interpretable.
