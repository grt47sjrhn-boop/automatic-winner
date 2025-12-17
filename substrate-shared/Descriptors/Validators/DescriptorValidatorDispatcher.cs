using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Frames;
using substrate_shared.Descriptors.Types;

namespace substrate_shared.Descriptors.Validators
{
    public static class DescriptorValidatorDispatcher
    {
        public static bool Validate(BaseDescriptor descriptor, out string? error)
        {
            switch (descriptor)
            {
                case IntentDescriptor intent:
                    return new IntentDescriptorValidator().IsValid(intent, out error);
                case ModifierDescriptor modifier:
                    return new ModifierDescriptorValidator().IsValid(modifier, out error);
                case ResultDescriptor result:
                    return new ResultDescriptorValidator().IsValid(result, out error);
                case ContextDescriptor context:
                    return new ContextDescriptorValidator().IsValid(context, out error);
                case ChannelDescriptor channel:
                    return new ChannelDescriptorValidator().IsValid(channel, out error);
                case SubjectDescriptor subject:
                    return new SubjectDescriptorValidator().IsValid(subject, out error);
                case TriggerDescriptor trigger:
                    return new TriggerDescriptorValidator().IsValid(trigger, out error);
                case ConditionDescriptor condition:
                    return new ConditionDescriptorValidator().IsValid(condition, out error);
                default:
                    error = $"No validator found for descriptor type: {descriptor?.Type}";
                    return false;
            }
        }

        public static bool Validate(SimulationFrame frame, out List<string> errors)
        {
            errors = [];

            // Validate single descriptors
            ValidateIfPresent(frame.Intent, errors);
            ValidateIfPresent(frame.Context, errors);
            ValidateIfPresent(frame.Channel, errors);

            // Validate descriptor collections
            ValidateEach(frame.Subjects, errors);
            ValidateEach(frame.Modifiers, errors);
            ValidateEach(frame.Triggers, errors);
            ValidateEach(frame.Conditions, errors);
            ValidateEach(frame.Results, errors);

            return errors.Count == 0;
        }

        private static void ValidateIfPresent(BaseDescriptor? descriptor, List<string> errors)
        {
            if (descriptor != null && !Validate(descriptor, out var error) && !string.IsNullOrWhiteSpace(error))
            {
                errors.Add(error);
            }
        }

        private static void ValidateEach<T>(IEnumerable<T>? descriptors, List<string> errors) where T : BaseDescriptor
        {
            if (descriptors == null) return;

            foreach (var descriptor in descriptors)
            {
                if (descriptor == null) continue;

                if (!Validate(descriptor, out var error) && !string.IsNullOrWhiteSpace(error))
                {
                    errors.Add(error);
                }
            }
        }
    }
}