﻿//HintName: BasicXpoWithObjectSpaceTest.BasicXpoWithObjectSpace.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicXpoWithObjectSpaceBuilder : BasicXpoWithObjectSpaceBuilder<MyProject.BasicXpoWithObjectSpace, BasicXpoWithObjectSpaceBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicXpoWithObjectSpaceBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicXpoWithObjectSpace
        where TBuilder : BasicXpoWithObjectSpaceBuilder<TClass, TBuilder>
    {
        protected DevExpress.Xpo.Session Session { get; set; }
        protected bool WasSessionSet { get; private set; }
        
        public TBuilder WithSession(DevExpress.Xpo.Session session)
        {
            this.Session = session;
            this.WasSessionSet = true;
            return This;
        }
        
        protected DevExpress.ExpressApp.IObjectSpace ObjectSpace { get; set; }
        protected bool WasObjectSpaceSet { get; private set; }
        
        public TBuilder WithObjectSpace(DevExpress.ExpressApp.IObjectSpace objectSpace)
        {
            this.ObjectSpace = objectSpace;
            this.WasObjectSpaceSet = true;
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
            if(this.WasObjectSpaceSet)
            {
                return this.ObjectSpace.CreateObject<TClass>();
            }
            
            if(this.WasSessionSet)
            {
                return (TClass)new MyProject.BasicXpoWithObjectSpace(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.BasicXpoWithObjectSpace] without a Session or ObjectSpace.{System.Environment.NewLine}Make sure to use the [WithSession] or [WithObjectSpace] methods when using the [{this.GetType().FullName}] type.");
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            return target;
        }
    }
}
