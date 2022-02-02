//HintName: BasicXpoWithParentAndGrandParentBuilder.BasicXpoGrandParent.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicXpoGrandParentBuilder : BasicXpoGrandParentBuilder<MyProject.BasicXpoGrandParent, BasicXpoGrandParentBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicXpoGrandParentBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicXpoGrandParent
        where TBuilder : BasicXpoGrandParentBuilder<TClass, TBuilder>
    {
        protected DevExpress.Xpo.Session Session { get; set; }
        protected bool WasSessionSet { get; private set; }
        
        public TBuilder WithSession(DevExpress.Xpo.Session session)
        {
            this.Session = session;
            this.WasSessionSet = true;
            return This;
        }
        
        protected TBuilder This
        {
            get
            {
                return (TBuilder)this;
            }
        }
        
        protected virtual TClass CreateTarget()
        {
            if(this.WasSessionSet)
            {
                return (TClass)new MyProject.BasicXpoGrandParent(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.BasicXpoGrandParent] without a Session.{System.Environment.NewLine}Make sure to use the [WithSession] method when using the [{this.GetType().FullName}] type.");
        }
        
        protected string GrantParentStringProperty { get; set; }
        protected bool WasGrantParentStringPropertyCalled { get; private set; }
        
        public TBuilder WithGrantParentStringProperty(string grantParentStringProperty)
        {
            this.GrantParentStringProperty = grantParentStringProperty;
            this.WasGrantParentStringPropertyCalled = true;
            return This;
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            if(this.WasGrantParentStringPropertyCalled)
            {
                target.GrantParentStringProperty = this.GrantParentStringProperty;
            }
            
            return target;
        }
    }
}
