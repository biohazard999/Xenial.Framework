//HintName: DoesExpandComplexMemberTree2.TargetClassBuilder.g.cs
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
            public const string Parent1 = "Parent1";
            public const string Parent2 = "Parent2";
        }
        
        private partial struct Editor
        {
            public static LayoutPropertyEditorItem Parent1 { get { return LayoutPropertyEditorItem.Create("Parent1"); } }
            public static LayoutPropertyEditorItem Parent2 { get { return LayoutPropertyEditorItem.Create("Parent2"); } }
        }
    }
}
namespace MyProject
{
    partial class TargetClassBuilder
    {
        private partial struct Constants
        {
            public partial struct _Parent1
            {
                public const string ParentString = "Parent1.ParentString";
            }
        }
        
        private partial struct Editor
        {
            public partial struct _Parent1
            {
                public static StringLayoutPropertyEditorItem ParentString { get { return StringLayoutPropertyEditorItem.Create("Parent1.ParentString"); } }
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
            public partial struct _Parent2
            {
                public const string ParentString = "Parent2.ParentString";
            }
        }
        
        private partial struct Editor
        {
            public partial struct _Parent2
            {
                public static StringLayoutPropertyEditorItem ParentString { get { return StringLayoutPropertyEditorItem.Create("Parent2.ParentString"); } }
            }
        }
    }
}
