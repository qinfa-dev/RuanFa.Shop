namespace RuanFa.FashionShop.Application.Abstractions.Loggings.Attributes;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class LogActivityAttribute(string? actionName = null) : Attribute
{
    public string? ActionName { get; } = actionName;
}
