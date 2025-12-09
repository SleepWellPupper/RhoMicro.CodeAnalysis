// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Equa;

using Library.Models.Collections;
using Lyra;
using Microsoft.CodeAnalysis;

internal readonly record struct RecordComponent(RecordModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var type = ComponentFactory.Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            b.Append("partial")
                .SetCondition(m.IsRecord)
                .Append(" record")
                .UnsetCondition()
                .Append(m.TypeKind is TypeKind.Struct ? " struct " : " class ")
                .Append(new RecordNameComponent(m))
                .SetCondition(m.IsRecord)
                .Append(" : global::System.IEquatable<")
                .Append(m.Name)
                .SetCondition(m.TypeParameters is not [])
                .Append($"<{ComponentFactory.List(m.TypeParameters, separator: ", ")}>")
                .UnsetCondition()
                .Append('>')
                .UnsetCondition()
                .AppendLine()
                .Append($$"""
                          {
                              public override int GetHashCode()
                              {
                                  var hc = new global::System.HashCode();
                                  
                                  {{ComponentFactory.List(m.ScalarMembers, static (m, i, l, b, ct) =>
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      b.Append($"hc.Add(this.{m});");
                                  }, "\n")}}
                                  
                                  {{ComponentFactory.List(m.VectorMembers, static (m, i, l, b, ct) =>
                                  {
                                      ct.ThrowIfCancellationRequested();

                                      b.Append($$"""
                                                 foreach(var element in {{m}})
                                                 {
                                                     hc.Add(element);
                                                 }
                                                 """);
                                  }, "\n")}}
                                  
                                  var result = hc.ToHashCode();
                                  
                                  return result;
                              }
                              
                              {{ComponentFactory.Create<RecordModel>(m, static (m, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  if (m.IsRecord)
                                  {
                                      return;
                                  }

                                  b.Append("public override bool Equals(object? other) => other is ")
                                      .Append(new RecordNameComponent(m))
                                      .Append(" o && Equals(o);");
                              })}}
                              
                              {{ComponentFactory.Create<RecordModel>(m, static (m, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  if (m.IsRecord)
                                  {
                                      return;
                                  }

                                  b.Append($$"""
                                             public {{(m.IsRecord ? String.Empty : "override ")}}bool Equals({{new RecordNameComponent(m)}}{{(m.TypeKind is TypeKind.Struct ? String.Empty : "?")}} other)
                                             {
                                                 {{ComponentFactory.Create<RecordModel>(m, static (m, b, ct) =>
                                                 {
                                                     ct.ThrowIfCancellationRequested();

                                                     if (m.TypeKind is TypeKind.Struct)
                                                     {
                                                         return;
                                                     }

                                                     b.Append("""
                                                              if(other is null)
                                                              {
                                                                  return false;
                                                              }
                                                              """
                                                     );
                                                 })}}

                                                  {{ComponentFactory.List(m.ScalarMembers, static (m, i, l, b, ct) =>
                                                  {
                                                      ct.ThrowIfCancellationRequested();

                                                      b.Append($$"""
                                                                 if(!global::System.Collections.Generic.EqualityComparer<{{m.Type}}>.Default.Equals(this.{{m.Name}}, other.{{m.Name}}))
                                                                 {
                                                                     return false;
                                                                 }
                                                                 """);
                                                  }, "\n")}}

                                                  {{ComponentFactory.List(m.VectorMembers, static (m, i, l, b, ct) =>
                                                  {
                                                      ct.ThrowIfCancellationRequested();

                                                      b.Append($$"""
                                                                 if(!this.{{m.Name}}.SequenceEqual(other.{{m.Name}}))
                                                                 {
                                                                     return false;
                                                                 }
                                                                 """);
                                                  }, "\n")}}
                                                  
                                                  return true;
                                             }
                                             """
                                  );
                              })}}
                          }
                          """
                );
        });

        var containingTypes = ComponentFactory.Create((Model, type), static (t, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var (model, type) = t;

            b.AppendLine("using System.Linq;");

            foreach (var containingType in model.ContainingTypes)
            {
                b.Append($"{containingType.Modifier} {containingType.Name}");
                if (containingType.TypeParameters is not [])
                {
                    b.Append($"<{ComponentFactory.List(containingType.TypeParameters, separator: ", ")}>");
                }

                b.AppendLine().AppendLine('{').Indent();
            }

            b.AppendLine(type);

            for (var i = 0; i < model.ContainingTypes.Count; i++)
            {
                b.Detent().AppendLine('}');
            }
        });

        var @namespace = ComponentFactory.Namespace(
            Model.Namespace,
            containingTypes);

        builder.Append(@namespace);
    }
}
