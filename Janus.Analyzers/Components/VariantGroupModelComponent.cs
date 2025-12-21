// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct VariantGroupModelComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append(
                $$"""
                  {{Summary("Initializes a new instance.")}}
                  {{Param("kinds", "The kinds of variant groups to be included in the group set.")}}
                  public VariantGroupModel(VariantGroupKinds kinds)
                  {
                      Kinds = kinds;
                  }

                  {{Summary("Gets the kinds of groups contained in the group.")}}
                  public VariantGroupKinds Kinds { get; }

                  {{List<String>(m.VariantGroups, static (g, _, _, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append(
                          $$"""
                            {{Summary(g, static (g, b, ct) =>
                            {
                                ct.ThrowIfCancellationRequested();

                                b.Append($"Gets the group model representing the <c>{g}</c> group.");
                            })}}
                            public static VariantGroupModel {{g}} { get; } = new(VariantGroupKinds.{{g}});               
                            """
                      );
                  }, separator: "\n")}}

                  {{List<String>(m.VariantGroups, static (g, _, _, b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      if (g is "None")
                      {
                          return;
                      }

                      b.AppendLine(
                          $"""
                           {Summary(g, static (g, b, ct) =>
                           {
                               ct.ThrowIfCancellationRequested();

                               b.Append($"Gets a value indicating whether this group contains the <c>{g}</c> group.");
                           })}
                           public bool Contains{g} => Contains(VariantGroupModel.{g});
                           """
                      );
                  })}}
                  {{Summary("Gets a value indicating whether this group contains the provided group.")}}
                  {{Param("group", "The group to check is contained in this group.")}}
                  {{Returns(static (b, ct) =>
                  {
                      ct.ThrowIfCancellationRequested();

                      b.Append($"{Langword("true")} if this group contains {ParamRef("group")}; otherwise, {Langword("false")}.");
                  })}}
                  public bool Contains(VariantGroupModel group) => (Kinds & group.Kinds) == group.Kinds;

                  {{Summary("Gets the set of all available groups.")}}
                  public static global::System.Collections.Immutable.ImmutableArray<VariantGroupModel> AllValues =>
                  [
                      {{List<String>(m.VariantGroups, static (g, _, _, b, ct) =>
                      {
                          ct.ThrowIfCancellationRequested();

                          b.Append(g);
                      }, separator: ",\n")}}
                  ];

                  {{Summary("Gets the amount of individual groups contained in this group.")}}
                  public int IndividualGroupCount => PopCount((uint)Kinds);

                  private void GetIndividualGroups(global::System.Span<VariantGroupModel> buffer)
                  {
                      var count = IndividualGroupCount;

                      if (count is 0)
                      {
                          return;
                      }

                      if (buffer.Length < count)
                      {
                          throw new global::System.ArgumentOutOfRangeException(
                              nameof(buffer),
                              $"{nameof(buffer)} did not have the required length of IndividualGroupCount. The required length was {count}, but the span provided had a length of {buffer.Length}.");
                      }

                      if (count is 1)
                      {
                          buffer[0] = this;
                          return;
                      }

                      var groupIndex = 0;
                      for (var i = LeadingZeroCount() + 1; i < 33 && groupIndex < count; i++)
                      {
                          var flagPosition = 32 - i;
                          var flag = 1 << flagPosition;
                          if (((int)Kinds & flag) == flag)
                          {
                              buffer[groupIndex++] = new VariantGroupModel((VariantGroupKinds)flag);
                          }
                      }
                  }

                  {{Summary("Gets the individual groups contained in this group.")}}
                  {{Returns("The individual groups contained in this group.")}}
                  public global::System.Collections.Immutable.ImmutableArray<VariantGroupModel> GetIndividualGroups()
                  {
                      var groups = new VariantGroupModel[IndividualGroupCount];
                      GetIndividualGroups(groups);
                      var result = global::System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(groups);

                      return result;
                  }

                  {{Summary("Gets the name of the group.")}}
                  public string Name
                  {
                      get
                      {
                          var count = IndividualGroupCount;

                          if (count is 0 or 1)
                          {
                              return DegenerateName;
                          }

                          global::System.Span<VariantGroupModel>
                          groups = stackalloc VariantGroupModel[count]; // Count never exceeds 32
                          GetIndividualGroups(groups);
                          var builder = new global::System.Text.StringBuilder();
                          for (var i = 0; i < count; i++)
                          {
                              var group = groups[i];
                              var name = group.DegenerateName;

                              if (i is not 0)
                              {
                                  builder.Append(" | ");
                              }

                              builder.Append(name);
                          }

                          var result = builder.ToString();

                          return result;
                      }
                  }

                  private string DegenerateName
                  {
                      get
                      {
                          switch (Kinds)
                          {
                              {{List(m.VariantGroups, static (v, _, _, b, ct) =>
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      b.Append(
                                          $$"""
                                            case VariantGroupKinds.{{v}}:
                                            {
                                                return nameof(VariantGroupKinds.{{v}});
                                            }
                                            """
                                      );
                                  }, separator: "\n")
                              }}
                              default:
                              {
                                  throw CreateInvalidKindsException();
                              }
                          }
                      }
                  }

                  private global::System.InvalidOperationException CreateInvalidKindsException()
                    => new($"The {nameof(VariantGroupModel)} instance was not initialized correctly and is holding an invalid value: {Kinds}");

                  {{Inheritdoc()}}
                  public override string ToString() => Kinds.ToString();

                  private static global::System.ReadOnlySpan<byte> Log2DeBruijn =>
                  [
                      00, 09, 01, 10, 13, 21, 02, 29,
                      11, 14, 16, 18, 22, 25, 03, 30,
                      08, 12, 20, 28, 15, 17, 24, 07,
                       19, 27, 23, 06, 26, 05, 04, 31
                  ];

                  private int LeadingZeroCount()
                  {
                      var value = (uint)Kinds;

                      if (value == 0)
                      {
                          return 32;
                      }

                      value |= value >> 01;
                      value |= value >> 02;
                      value |= value >> 04;
                      value |= value >> 08;
                      value |= value >> 16;

                      var result = 31 ^ global::System.Runtime.CompilerServices.Unsafe.AddByteOffset(
                          ref global::System.Runtime.InteropServices.MemoryMarshal.GetReference(Log2DeBruijn),
                          ({{typeof(IntPtr)}})(int)((value * 0x07C4ACDDu) >> 27));

                      return result;
                  }

                  private static int PopCount(uint value)
                  {
                      const uint c1 = 0x_55555555u;
                      const uint c2 = 0x_33333333u;
                      const uint c3 = 0x_0F0F0F0Fu;
                      const uint c4 = 0x_01010101u;

                      value -= (value >> 1) & c1;
                      value = (value & c2) + ((value >> 2) & c2);
                      value = (((value + (value >> 4)) & c3) * c4) >> 24;

                      return (int)value;
                  }
                  """
            );
        });

        var type = Create((Model, members), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, members) = t;

            b.Append(
                $"""
                 {Summary(model, static (m, b, ct) =>
                 {
                     ct.ThrowIfCancellationRequested();

                     b.Append($"Provides strongly typed access to the group of a variant of {Cref(m.DocsCommentId)}.");
                 })}
                 {Type(
                     "public readonly record struct",
                     "VariantGroupModel",
                     members)}
                 """
            );
        });

        var region = Region("VariantGroupModel", type);

        builder.Append(region);
    }
}
