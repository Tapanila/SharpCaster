// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Suppress CA1707 (remove underscores from identifiers) for all Models and Messages
// These follow Google Cast API naming conventions which use underscores
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Google Cast API naming conventions use underscores", Scope = "NamespaceAndDescendants", Target = "~N:Sharpcaster.Models")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Google Cast API naming conventions use underscores", Scope = "NamespaceAndDescendants", Target = "~N:Sharpcaster.Messages")]
