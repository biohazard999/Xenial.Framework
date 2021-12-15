﻿//HintName: MyNonPartialClass.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    partial class MyPartialClass
    {
        private struct PropertyIdentifier
        {
            private string propertyName;
            public string PropertyName { get { return this.propertyName; } }
            
            private PropertyIdentifier(string propertyName)
            {
                this.propertyName = propertyName;
            }
            
            public static implicit operator string(PropertyIdentifier identifier)
            {
                return identifier.PropertyName;
            }
            
            public static PropertyIdentifier Create(string propertyName)
            {
                return new PropertyIdentifier(propertyName);
            }
        }
    }
}
