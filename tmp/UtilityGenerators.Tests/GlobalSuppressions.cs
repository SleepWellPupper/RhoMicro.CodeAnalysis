// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "test names are more expressive this way.")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Test methods receive theory data (not null) and are not intended for external calls.")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Test classes need to be public.")]
