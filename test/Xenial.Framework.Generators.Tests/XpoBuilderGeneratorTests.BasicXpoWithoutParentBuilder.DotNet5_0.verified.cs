﻿//HintName: BasicXpoWithoutParentBuilder.BasicXpoWithoutParentBuilder.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicXpoWithoutParentBuilderBuilder : BasicXpoWithoutParentBuilderBuilder<MyProject.BasicXpoWithoutParentBuilder, BasicXpoWithoutParentBuilderBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicXpoWithoutParentBuilderBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicXpoWithoutParentBuilder
        where TBuilder : BasicXpoWithoutParentBuilderBuilder<TClass, TBuilder>
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
                return (TClass)new MyProject.BasicXpoWithoutParentBuilder(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.BasicXpoWithoutParentBuilder] without a Session.{System.Environment.NewLine}Make sure to use the [WithSession] method when using the [{this.GetType().FullName}] type.");
        }
        
        protected string ParentStringProperty { get; set; }
        protected bool WasParentStringPropertyCalled { get; private set; }
        
        public TBuilder WithParentStringProperty(string parentStringProperty)
        {
            this.ParentStringProperty = parentStringProperty;
            this.WasParentStringPropertyCalled = true;
            return This;
        }
        
        protected string OwnStringProperty { get; set; }
        protected bool WasOwnStringPropertyCalled { get; private set; }
        
        public TBuilder WithOwnStringProperty(string ownStringProperty)
        {
            this.OwnStringProperty = ownStringProperty;
            this.WasOwnStringPropertyCalled = true;
            return This;
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            if(this.WasParentStringPropertyCalled)
            {
                target.ParentStringProperty = this.ParentStringProperty;
            }
            
            if(this.WasOwnStringPropertyCalled)
            {
                target.OwnStringProperty = this.OwnStringProperty;
            }
            
            return target;
        }
    }
}
