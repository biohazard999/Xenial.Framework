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
        
        private partial struct Constants
        {
            public const string Parent = "Parent";
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
        
        private partial struct Editor
        {
            public partial struct _Parent
            {
                public partial struct _GrandParent
                {
                    public static StringLayoutPropertyEditorItem GrandParentStringMember { get { return StringLayoutPropertyEditorItem.Create("Parent.GrandParent.GrandParentStringMember"); } }
                }
            }
        }
    }
}
