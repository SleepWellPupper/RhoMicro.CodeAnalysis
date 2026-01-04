// SPDX-License-Identifier: MPL-2.0

namespace RhoMicro.CodeAnalysis.Janus;

using Lyra;
using static Lyra.ComponentFactory;
using static Lyra.ComponentFactory.Docs;

internal readonly record struct SwitchComponent(UnionModel Model) : ICSharpSourceComponent
{
    public void AppendTo(CSharpSourceBuilder builder, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var methods = Create(Model, static (m, b, ct) =>
        {
            ct.ThrowIfCancellationRequested();

            var statelessHandler = Create(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchHandlerParams(m)}}
                      public void Switch(
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append($"{typeof(Action)}<{v.Type.NullableName}> on{v.Name}");
                              }, separator: ",\n")
                          }})
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                              {
                                  ct.ThrowIfCancellationRequested();

                                  b.Append(
                                      $"""
                                       on{v.Name}.Invoke({new VariantAccessorComponent(v)});
                                       return;
                                       """);
                              })
                          }}
                      }
                      """);
            });

            var statefulHandler = Create(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchStateParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchStateTypeparam()}}
                      public void Switch<TState>(
                          TState state,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{typeof(Action)}<{v.Type.NullableName}, TState> on{v.Name}");
                          }, separator: ",\n")}})
                      #if NET9_0_OR_GREATER
                          where TState : allows ref struct
                      #endif
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append(
                                  $"""
                                   on{v.Name}.Invoke({new VariantAccessorComponent(v)}, state);
                                   return;
                                   """);
                          })}}
                      }
                      """);
            });

            var statelessDefaultHandler = Create(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchDefaultHandlerParam()}}
                      {{SwitchHandlerParams(m)}}
                      public void Switch(
                          {{typeof(Action)}} defaultHandler,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{typeof(Action)}<{v.Type.NullableName}>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}});
                                         }
                                         else 
                                         {
                                             defaultHandler.Invoke();
                                         }

                                         return;
                                         """
                              );
                          })}}
                      }
                      """);
            });

            var statefulDefaultHandler = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchStateParam()}}
                      {{SwitchDefaultHandlerParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchStateTypeparam()}}
                      public void Switch<TState>(
                          TState state,
                          {{typeof(Action)}}<TState> defaultHandler,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{typeof(Action)}<{v.Type.NullableName}, TState>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      #if NET9_0_OR_GREATER
                          where TState : allows ref struct
                      #endif
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}}, state);
                                         }
                                         else 
                                         {
                                             defaultHandler.Invoke(state);
                                         }

                                         return;
                                         """
                              );
                          })}}
                      }
                      """);
            });

            const String func = "global::System.Func";

            var statelessProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TResult>(
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TResult> on{v.Name}");
                          }, separator: ",\n")}})
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"return on{v.Name}.Invoke({new VariantAccessorComponent(v)});");
                          })}}
                      }
                      """);
            });

            var statefulProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchStateParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchStateTypeparam()}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TState, TResult>(
                          TState state,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TState, TResult> on{v.Name}");
                          }, separator: ",\n")}})
                      #if NET9_0_OR_GREATER
                          where TState : allows ref struct
                      #endif
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"return on{v.Name}.Invoke({new VariantAccessorComponent(v)}, state);");
                          })}}
                      }
                      """);
            });

            var statelessDefaultProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchDefaultHandlerParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TResult>(
                          {{func}}<TResult> defaultHandler,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TResult>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             return on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}});
                                         }
                                         else 
                                         {
                                             return defaultHandler.Invoke();
                                         }
                                         """
                              );
                          })}}
                      }
                      """);
            });

            var statefulDefaultProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchStateParam()}}
                      {{SwitchDefaultHandlerParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchStateTypeparam()}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TState, TResult>(
                          TState state,
                          {{func}}<TState, TResult> defaultHandler,
                          {{List<UnionTypeAttribute.Model>(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TState, TResult>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      #if NET9_0_OR_GREATER
                          where TState : allows ref struct
                      #endif
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             return on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}}, state);
                                         }
                                         else 
                                         {
                                             return defaultHandler.Invoke(state);
                                         }
                                         """
                              );
                          })}}
                      }
                      """);
            });

            var statelessDefaultResultProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchDefaultResultParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TResult>(
                          TResult defaultResult,
                          {{List(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TResult>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             return on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}});
                                         }
                                         else 
                                         {
                                             return defaultResult;
                                         }
                                         """
                              );
                          })}}
                      }
                      """);
            });

            var statefulDefaultResultProjection = Create<UnionModel>(m, static (m, b, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                b.Append(
                    $$"""
                      {{SwitchSummary()}}
                      {{SwitchStateParam()}}
                      {{SwitchDefaultResultParam()}}
                      {{SwitchHandlerParams(m)}}
                      {{SwitchStateTypeparam()}}
                      {{SwitchResultTypeparam()}}
                      {{SwitchReturns()}}
                      public TResult Switch<TState, TResult>(
                          TState state,
                          TResult defaultResult,
                          {{List(m.Variants, static (v, _, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($"{func}<{v.Type.NullableName}, TState, TResult>? on{v.Name} = null");
                          }, separator: ",\n")}})
                      #if NET9_0_OR_GREATER
                          where TState : allows ref struct
                      #endif
                      {
                          {{new VariantsSwitchComponent(m, static (v, _, b, ct) =>
                          {
                              ct.ThrowIfCancellationRequested();

                              b.Append($$"""
                                         if(on{{v.Name}} is not null)
                                         {
                                             return on{{v.Name}}.Invoke({{new VariantAccessorComponent(v)}}, state);
                                         }
                                         else 
                                         {
                                             return defaultResult;
                                         }
                                         """
                              );
                          })}}
                      }
                      """);
            });

            b.Append($"""
                      {statelessHandler}
                      {statelessDefaultHandler}
                      {statefulHandler}
                      {statefulDefaultHandler}
                      {statelessProjection}
                      {statelessDefaultProjection}
                      {statelessDefaultResultProjection}
                      {statefulProjection}
                      {statefulDefaultProjection}
                      {statefulDefaultResultProjection}
                      """
            );
        });

        var region = Region("Switch", methods);

        builder.Append(region);
    }
}
