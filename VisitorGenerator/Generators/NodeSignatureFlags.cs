namespace RhoMicro.CodeAnalysis;

[Flags]
internal enum NodeSignatureFlags
{
    None = 0,
    IsRecord = 1 << 0,
    IsPublic = 1 << 1,
}
