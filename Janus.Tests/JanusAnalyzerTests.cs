// SPDX-License-Identifier: MPL-2.0

namespace Janus.Tests;

using Microsoft.CodeAnalysis.Testing;
using RhoMicro.CodeAnalysis.Janus;

public class JanusAnalyzerTests
{
    [Fact]
    public Task EqualsIsNotOverriddenIfUserProvided() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        partial struct Union
        {
            public bool Equals(Union other) => false;
        }
        """);

    [Fact]
    public Task StructUnionHasEqualityOperatorEmitted() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        partial struct Union
        {
            public static bool operator !=(Union a, Union b) => throw null;
            public static bool operator ==(Union a, Union b) => throw null;
        }
        """);

    [Theory]
    [InlineData("None")]
    [InlineData("Simple")]
    [InlineData("Detailed")]
    public Task ToStringSettingIgnored(String setting) => JanusTest.TestAnalyzer(
        $$"""
          using RhoMicro.CodeAnalysis;

          [UnionType<int>, UnionTypeSettings( {|RMJ0001:ToStringSetting = ToStringSetting.{{setting}}|} )]
          partial struct Union
          {
              public override string ToString() => throw null;
          }
          """);

    [Theory]
    [InlineData("class ")]
    [InlineData("struct ")]
    [InlineData("")]
    public Task RecordUnionsAreDisallowed(String modifier) => JanusTest.TestAnalyzer(
        $$"""
          using RhoMicro.CodeAnalysis;

          #pragma warning disable RMJ0018

          [UnionType<int>]
          partial {|RMJ0002:record|} {{modifier}}Union;
          """);

    [Fact]
    public Task ExplicitlyGenericUnionsCannotBeJsonSerializable() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        [UnionTypeSettings( {|RMJ0003:JsonConverterSetting = JsonConverterSetting.EmitJsonConverter|} )]
        partial struct Union<[UnionType] T>;
        """);

    [Fact]
    public Task TransientlyGenericUnionsCannotBeJsonSerializable() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        partial class Outer<T>
        {
            [UnionType<int>]
            [UnionTypeSettings( {|RMJ0003:JsonConverterSetting = JsonConverterSetting.EmitJsonConverter|} )]
            partial struct Union;
        }
        """);

    [Fact]
    public Task ExplicitlyGenericConstValueUnionsCannotBeJsonSerializable() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        static class Constants
        {
            public const JsonConverterSetting Value = JsonConverterSetting.EmitJsonConverter;
        }

        [UnionTypeSettings( {|RMJ0003:JsonConverterSetting = Constants.Value|} )]
        partial struct Union<[UnionType] T>;
        """);

    [Fact]
    public Task TransientlyGenericConstValueUnionsCannotBeJsonSerializable() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        static class Constants
        {
            public const JsonConverterSetting Value = JsonConverterSetting.EmitJsonConverter;
        }

        partial class Outer<T>
        {
            [UnionType<int>]
            [UnionTypeSettings( {|RMJ0003:JsonConverterSetting = Constants.Value|} )]
            partial struct Union;
        }
        """);

    [Fact]
    public Task NoMoreThan31VariantGroupsMayBeDefined() => JanusTest.TestAnalyzer(
        $$"""
          using RhoMicro.CodeAnalysis;

          [UnionType<int>({|RMJ0004:Groups = [{{String.Join(", ", Enumerable.Range(1, 32).Select(i => $"\"Group{i}\""))}}]|})]
          partial struct Union;
          """);

    [Fact]
    public Task StaticUnionsAreDisallowed() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<int>]
        {|RMJ0005:static|} partial class Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_SingleExplicitDuplicate() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>({|RMJ0006:Name = "Foo"|})]
        [UnionType<double>({|RMJ0006:Name = "Foo"|})]
        partial struct Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_MultipleExplicitDuplicate() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>({|RMJ0006:Name = "Foo"|})]
        [UnionType<double>({|RMJ0006:Name = "Foo"|})]
        [UnionType<float>({|RMJ0006:Name = "Foo"|})]
        [UnionType<string>({|RMJ0006:Name = "Foo"|})]
        partial struct Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_SingleImplicitDuplicate() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;

        [UnionType<{|RMJ0006:List<int>|}>]
        [UnionType<{|RMJ0006:List<float>|}>]
        [UnionType<int>]
        partial struct Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_SingleImplicitInlineDuplicate() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;

        [UnionType<{|RMJ0006:List<int>|}, int, {|RMJ0006:List<float>|}>]
        partial struct Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_MultipleImplicitDuplicate() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;

        [UnionType<int>]
        [UnionType<{|RMJ0006:List<float>|}>]
        [UnionType<{|RMJ0006:List<string>|}>]
        [UnionType<{|RMJ0006:List<char>|}>]
        partial struct Union;
        """);

    [Fact]
    public Task VariantNamesMustBeUnique_TypeParameter() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;

        [UnionType<{|RMJ0006:System.String|}>]
        partial struct Union<[UnionType]{|RMJ0006:String|}>;
        """);

    [Fact]
    public Task EnsureValidStructUnionState() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<string>]
        partial struct {|RMJ0007:Union|};
        """);

    [Fact]
    public Task InterfaceVariantIsExcludedFromConversionOperators() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;

        #pragma warning disable RMJ0018

        [UnionType<{|RMJ0008:IEnumerable<int>|}>]
        partial class Union;
        """
    );

    [Fact]
    public Task VariantTypesMustBeUnique() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018
        #pragma warning disable RMJ0006

        [UnionType<{|RMJ0009:int|}>]
        [UnionType<{|RMJ0009:int|}>]
        partial class Union;
        """
    );

    [Fact]
    public Task VariantTypesMustBeUniqueInline() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018
        #pragma warning disable RMJ0006

        [UnionType<{|RMJ0009:int|}, {|RMJ0009:int|}>]
        partial class Union;
        """
    );

    [Fact]
    public Task ObjectCannotBeUsedAsAVariant_Class() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<{|RMJ0010:object|}>]
        partial class Union;
        """
    );

    [Fact]
    public Task ObjectCannotBeUsedAsAVariant_Struct() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0007

        [UnionType<{|RMJ0010:object|}>]
        partial struct Union;
        """
    );

    [Fact]
    public Task ValueTypeCannotBeUsedAsAVariantOfStructUnions() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0007

        [UnionType<{|RMJ0019:System.ValueType|}>]
        partial struct Union;
        """
    );

    [Fact]
    public Task UnionCannotBeUsedAsVariantOfItself() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<{|RMJ0012:Union|}>]
        partial class Union;
        """
    );

    [Theory]
    [InlineData("System.Collections.Generic.HashSet<string>")]
    [InlineData("System.Collections.Generic.List<string>")]
    public Task UnionCannotExplicitlyDefineBaseType(String baseType) => JanusTest.TestAnalyzer(
        $$"""
          using RhoMicro.CodeAnalysis;

          #pragma warning disable RMJ0018

          [UnionType<int>]
          partial class {|RMJ0013:Union|} : {{baseType}};
          """
    );

    [Theory]
    [InlineData(
        "System.Collections.Generic.IEnumerable<string>",
        """
        public System.Collections.Generic.IEnumerator<string> GetEnumerator() => throw null;
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw null;
        """
    )]
    [InlineData(
        "System.IComparable<Union>",
        """
        public int CompareTo(Union other) => throw null;
        """
    )]
    public Task UnionCanExplicitlyDefineInterfaces(String baseType, String implementation) => JanusTest.TestAnalyzer(
        $$"""
          using RhoMicro.CodeAnalysis;

          #pragma warning disable RMJ0018

          [UnionType<int>]
          partial class Union : {{baseType}}
          {
          #pragma warning disable
              {{implementation}}
          #pragma warning restore
          }
          """
    );

    [Fact]
    public Task PreferNullableStructOverIsNullable() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<int>({|RMJ0014:IsNullable = true|})]
        [UnionType<string>(IsNullable = true)]
        partial class Union;
        """
    );

    [Fact]
    public Task NullableVariantNotAllowedAlongWithNonNullableVariant() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<{|RMJ0015:int|}>]
        [UnionType<{|RMJ0015:System.Nullable<int>|}>(Name = "NullableInt")]
        partial class Union;
        """
    );

    [Fact]
    public Task UnionTypeSettingsAttributeIgnoredDueToMissingUnionTypeAttribute() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [{|RMJ0016:UnionTypeSettings|}]
        partial class Union;
        """
    );

    [Fact]
    public Task DuplicateVariantGroupNamesAreIgnored() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        #pragma warning disable RMJ0018

        [UnionType<int>(Groups = [ "Group", {|RMJ0017:"Group"|} ])]
        partial class Union;
        """
    );

    [Fact]
    public Task ClassUnionsShouldBeSealed() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        partial class {|RMJ0018:Union|};
        """);

    [Fact]
    public Task UnionCannotBeRefStruct() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        ref partial struct {|RMJ0020:Union|};
        """);

    [Fact]
    public Task NonNullableReferenceTypeVariantCannotBeDefaultVariant() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        [UnionType<string>({|RMJ0023:IsDefault = true|})]
        readonly partial struct Union;
        """);

    [Fact]
    public Task ClassUnionsShouldNotUseDefaultVariants() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>({|RMJ0021:IsDefault = true|})]
        sealed partial class Union;
        """);

    [Fact]
    public Task CannotDeclareMultipleDefaultVariants() => JanusTest.TestAnalyzer(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>({|RMJ0022:IsDefault = true|})]
        [UnionType<double>({|RMJ0022:IsDefault = true|})]
        readonly partial struct Union;
        """);

    [Theory]
    [InlineData("""
                using RhoMicro.CodeAnalysis;
                using System.Collections.Generic;

                [UnionType<int>(Name = "Int")]
                [UnionType<List<string>>]
                sealed partial class Union<[UnionType(Name = "ValueT")] T> where T : struct
                {
                    public void Foo()
                    {
                        Switch(
                            onInt: _ => { },
                            onList: _ => { },
                            onValueT: _ => { });
                    }
                }
                """
    )]
    [InlineData(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        [UnionTypeSettings(ToStringSetting = ToStringSetting.Simple)]
        sealed partial class Union;
        """
    )]
    [InlineData(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int>]
        [UnionTypeSettings(ToStringSetting = ToStringSetting.None)]
        sealed partial class Union;
        """
    )]
    [InlineData(
        """
        using RhoMicro.CodeAnalysis;

        [UnionType<int[]>]
        sealed partial class Union;
        """
    )]
    [InlineData(
        """
        // StructUnionWithoutUnknownVariant
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;
        using System;

        [UnionType<string, List<int>, DateTime>]
        partial struct StructUnionWithoutUnknownVariant;
        """
    )]
    [InlineData(
        """
        // StructUnionWithUnmanagedDefaultVariant
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;
        using System.Threading;

        [UnionType<int>(IsDefault = true)]
        [UnionType<List<int>>(IsNullable = true)]
        // unmanaged but alphabetically first, managed value type, non-nullable reference type
        [UnionType<double, CancellationToken, string>]
        partial struct StructUnionWithUnmanagedDefaultVariant;
        """
    )]
    [InlineData(
        """
        // StructUnionWithManagedStructDefaultVariant
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;
        using System.Threading;

        [UnionType<CancellationToken>(IsDefault = true)]
        [UnionType<List<int>>(IsNullable = true)]
        // unmanaged, non-nullable reference type
        [UnionType<double, string>]
        partial struct StructUnionWithManagedStructDefaultVariant;
        """
    )]
    [InlineData(
        """
        // StructUnionWithNullableReferenceTypeDefaultVariant
        using RhoMicro.CodeAnalysis;
        using System.Collections.Generic;
        using System.Threading;

        [UnionType<List<int>>(IsNullable = true, IsDefault = true)]
        // unmanaged, managed struct, non-nullable reference type
        [UnionType<double, CancellationToken, string>]
        partial struct StructUnionWithNullableReferenceTypeDefaultVariant;
        """
    )]
    public Task ProducesNoDiagnostics(String source) => JanusTest.TestAnalyzer(source);
}
