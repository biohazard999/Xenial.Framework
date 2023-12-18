﻿//HintName: BasicXpoCtorTest.BasicXpoCtorObject.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicXpoCtorObjectBuilder : BasicXpoCtorObjectBuilder<MyProject.BasicXpoCtorObject, BasicXpoCtorObjectBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicXpoCtorObjectBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicXpoCtorObject
        where TBuilder : BasicXpoCtorObjectBuilder<TClass, TBuilder>
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
                return (TClass)new MyProject.BasicXpoCtorObject(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.BasicXpoCtorObject] without a Session.{System.Environment.NewLine}Make sure to use the [WithSession] method when using the [{this.GetType().FullName}] type.");
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            return target;
        }
    }
}
