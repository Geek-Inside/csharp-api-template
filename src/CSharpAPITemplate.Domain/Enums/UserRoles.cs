namespace CSharpAPITemplate.Domain.Enums;

/// <summary>
/// User roles. Can be combined as flags.
/// </summary>
[Flags]
public enum UserRoles
{
	User = 0,
	Admin = 1,
}