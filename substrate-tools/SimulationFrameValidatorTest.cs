using System;
using System.Collections.Generic;
using substrate_core.Internal.Engines;
using substrate_core.Providers.Registry;
using substrate_core.Registries;
using substrate_shared.DescriptorTypes.Enums;
using substrate_shared.DescriptorTypes.Frames;
using substrate_shared.DescriptorTypes.Types;
using substrate_shared.interfaces.Reports;
using substrate_shared.Providers.Contract;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Resolvers.Contract;
using substrate_shared.Resolvers.Contract.Types;

namespace substrate_tools
{
    #region Test Implementations

    public class TestFrameProvider : ISimulationFrameProvider
    {
        public SimulationFrame GetFrame() => new()
        {
            Intent = new IntentDescriptor
            {
                Id = "intent-001",
                IntentType = IntentAction.Approve,
                IntentTone = new NarrativeTone()
                
            },
            Context = new ContextDescriptor
            {
                Id = "context-001",
                NarrativeBackdrop = "Office",
                Setting = "After Hours"
            },
            Channel = new ChannelDescriptor
            {
                Id = "channel-001",
                Medium = "Text"
            },
            Subjects = new List<SubjectDescriptor>
            {
                new SubjectDescriptor
                {
                    SubjectId = "Attn",
                    Id = "subject-001",
                    Role = "User",
                    Archetype = "Didactic Hero"
                }
            },
            Modifiers = new List<ModifierDescriptor>
            {
                new ModifierDescriptor
                {
                    Id = "modifier-001",
                    ModifierTarget = DescriptorType.Channel,
                    Bias = Bias.Mixed,
                    Intensity = 4,
                    EffectDescription = "Ears, they bleed!"
                }
            },
            Triggers = new List<TriggerDescriptor>
            {
                new TriggerDescriptor
                {
                    Id = "trigger-001",
                    MatchMode = SubjectMatchMode.Any,
                    Subjects = new List<SubjectDescriptor>()
                    {
                        new SubjectDescriptor
                        {
                            SubjectId = "Attn",
                            Id = "subject-001",
                            Role = "User",
                            Archetype = "Didactic Hero"
                        }
                    },
                    Results = new List<ResultDescriptor>()
                    {
                        new ResultDescriptor
                        {
                            Id = "result-001",
                            ResultFormula = new ModifierDescriptor()
                            {
                                Bias = Bias.Mixed,
                                EffectDescription = "Effective",
                                Id = "modifier-001",
                                Intensity = 4,
                                ModifierTarget = DescriptorType.Subject
                            }
                        }
                    }
                }
            },
            Conditions = new List<ConditionDescriptor>
            {
                new ConditionDescriptor
                {
                    Id = "condition-001",
                    LeftOperand = "trust:subject",
                    Operator = ConditionOperator.LessThan,
                    RightOperand = "-3",
                    Logic = ConditionLogic.And,
                    Scope = ConditionScope.Self
                }
            },
            Results = new List<ResultDescriptor>
            {
                new ResultDescriptor
                {
                    Id = "result-001",
                    ResultFormula = new ModifierDescriptor()
                    {
                        Bias = Bias.Mixed,
                        EffectDescription = "Effective",
                        Id = "modifier-001",
                        Intensity = 4,
                        ModifierTarget = DescriptorType.Subject
                        
                    }
                }
            }
        };
    }

    public class TestReport : IReportSummary
    {
        private readonly List<string> _messages = new();

        public void LogValidationError(object error) =>
            Log($"[Validation Error] {error}");

        public void LogValidationError(IValidationError error) =>
            Log($"[Validation Error] {error.Message}");

        public void LogInfo(string message) => Log($"[Info] {message}");

        public void LogWarning(string message) => Log($"[Warning] {message}");

        public void LogError(string message) => Log($"[Error] {message}");

        public void LogException(Exception ex) => Log($"[Exception] {ex}");

        public IReadOnlyList<string> GetMessages() => _messages;

        private void Log(string message)
        {
            _messages.Add(message);
            Console.WriteLine(message);
        }
    }

    #endregion

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("=== CatalystEngine Bootstrapping ===");

            // Bootstrap resolver registry
            var resolverRegistry = new ResolverRegistry("CatalystResolvers");
            ResolverBootstrapper.RegisterAllResolvers(resolverRegistry, typeof(IntentResolver).Assembly);

            var descriptorRegistry = new DescriptorRegistry("CatalystDescriptors");
            DescriptorBootstrapper.RegisterAllDescriptors(descriptorRegistry, typeof(IntentDescriptor).Assembly);
            

            
            Console.WriteLine($"[Main] Registered resolvers in '{resolverRegistry.Name}':");
            foreach (var resolver in resolverRegistry.GetAll())
            {
                Console.WriteLine($" - {resolver.Name} ({resolver.Category}): {resolver.Description}");
            }

            // Setup service registry
            var services = new ServiceProviderRegistry();
            services.Register<ResolverRegistry>(resolverRegistry);
            services.Register<DescriptorRegistry>(descriptorRegistry);
            services.Register<ISimulationFrameProvider>(new TestFrameProvider());

            // Create engine and report
            var engine = new CatalystEngine(services);
            var report = new TestReport();

            // Execute engine
            var success = engine.Execute(report);

            Console.WriteLine($"=== Execution completed: {(success ? "Success" : "Failure")} ===");
        }
    }
}