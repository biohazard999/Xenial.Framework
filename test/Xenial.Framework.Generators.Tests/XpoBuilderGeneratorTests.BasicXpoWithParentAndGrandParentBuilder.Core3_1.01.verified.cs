//HintName: BasicXpoWithParentAndGrandParentBuilder.BasicXpoParent.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicXpoParentBuilder : BasicXpoParentBuilder<MyProject.BasicXpoParent, BasicXpoParentBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicXpoParentBuilder<TClass, TBuilder>
        : BasicXpoGrandParentBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicXpoParent
        where TBuilder : BasicXpoParentBuilder<TClass, TBuilder>
    {
        protected override TClass CreateTarget()
        {
            if(this.WasSessionSet)
            {
                return (TClass)new MyProject.BasicXpoParent(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.BasicXpoParent] without a Session.{System.Environment.NewLine}Make sure to use the [WithSession] method when using the [{this.GetType().FullName}] type.");
        }
        
        protected string ParentStringProperty { get; set; }
        protected bool WasParentStringPropertyCalled { get; private set; }
        
        public TBuilder WithParentStringProperty(string parentStringProperty)
        {
            this.ParentStringProperty = parentStringProperty;
            this.WasParentStringPropertyCalled = true;
            return This;
        }
        
        public override TClass Build()
        {
            TClass target = base.CreateTarget();
            
            if(this.WasParentStringPropertyCalled)
            {
                target.ParentStringProperty = this.ParentStringProperty;
            }
            
            return target;
        }
    }
}
