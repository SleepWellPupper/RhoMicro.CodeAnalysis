// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis;

partial class UnionTypeSettingsAttribute
{
    partial record struct Model
    {
        public static Model InheritanceRoot { get; } = new()
        {
            EqualityOperatorsSetting = EqualityOperatorsSetting.EmitOperatorsIfValueType,
            ToStringSetting = ToStringSetting.Detailed,
            JsonConverterSetting = JsonConverterSetting.OmitJsonConverter
        };
        
        public void Inherit(Model source)
        {
            if (EqualityOperatorsSetting is EqualityOperatorsSetting.Inherit)
            {
                EqualityOperatorsSetting = source.EqualityOperatorsSetting;
            }

            if (ToStringSetting is ToStringSetting.Inherit)
            {
                ToStringSetting = source.ToStringSetting;
            }

            if (JsonConverterSetting is JsonConverterSetting.Inherit)
            {
                JsonConverterSetting = source.JsonConverterSetting;
            }
        }
    }
}
