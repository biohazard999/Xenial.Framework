//HintName: DoesExpandComplexMemberTree.TargetClassBuilder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items;
using Xenial.Framework.Layouts.Items.Base;
using Xenial.Framework.Layouts.Items.LeafNodes;

namespace MyProject
{
    [CompilerGenerated]
    partial class TargetClassBuilder
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
        
        private partial struct Constants
        {
            public const string Parent = "Parent";
        }
        
        private partial struct Property
        {
            public static PropertyIdentifier Parent { get { return PropertyIdentifier.Create("Parent"); } }
            
        }
        
        private partial struct Editor
        {
            public static LayoutPropertyEditorItem Parent { get { return LayoutPropertyEditorItem.Create("Parent"); } }
            
        }
    }
}
namespace MyProject
{
    partial class TargetClassBuilder
    {
        private partial struct Constants
        {
            public partial struct _Parent
            {
                public const string GrandParent = "Parent.GrandParent";
                public const string Parents = "Parent.Parents";
            }
        }
        
        private partial struct Property
        {
            public partial struct _Parent
            {
                public static PropertyIdentifier GrandParent { get { return PropertyIdentifier.Create("Parent.GrandParent"); } }
                public static PropertyIdentifier Parents { get { return PropertyIdentifier.Create("Parent.Parents"); } }
            }
        }
        
        private partial struct Editor
        {
            public partial struct _Parent
            {
                public static LayoutPropertyEditorItem GrandParent { get { return LayoutPropertyEditorItem.Create("Parent.GrandParent"); } }
                public static LayoutPropertyEditorItem Parents { get { return LayoutPropertyEditorItem.Create("Parent.Parents"); } }
            }
        }
    }
}
namespace MyProject
{
    partial class TargetClassBuilder
    {
        private partial struct Constants
        {
            public partial struct _Parent
            {
                public partial struct _GrandParent
                {
                    public const string GrandParentStringMember = "Parent.GrandParent.GrandParentStringMember";
                }
            }
        }
        
        private partial struct Property
        {
            public partial struct _Parent
            {
                public partial struct _GrandParent
                {
                    public static PropertyIdentifier GrandParentStringMember { get { return PropertyIdentifier.Create("Parent.GrandParent.GrandParentStringMember"); } }
                }
            }
        }
        
        private partial struct Editor
        {
            public partial struct _Parent
            {
                public partial struct _GrandParent
                {
                    public static LayoutPropertyEditorItem GrandParentStringMember { get { return LayoutPropertyEditorItem.Create("Parent.GrandParent.GrandParentStringMember"); } }
                }
            }
        }
    }
}
