namespace Analysis_Lab1.Enums;

[AttributeUsage(AttributeTargets.Enum)] //Field?
internal sealed class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}