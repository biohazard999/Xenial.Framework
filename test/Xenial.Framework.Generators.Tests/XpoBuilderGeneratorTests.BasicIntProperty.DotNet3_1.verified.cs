﻿//HintName: BasicIntProperty.BasicIntProperty.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class BasicIntPropertyBuilder : BasicIntPropertyBuilder<MyProject.BasicIntProperty, BasicIntPropertyBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class BasicIntPropertyBuilder<TClass, TBuilder>
        where TClass : MyProject.BasicIntProperty
        where TBuilder : BasicIntPropertyBuilder<TClass, TBuilder>
    {
        protected TBuilder This
        {
            get
            {
                return (TBuilder)this;
            }
        }
        
        protected virtual TClass CreateTarget()
        {
            return (TClass)new MyProject.BasicIntProperty();
        }
        
        protected int IntProperty { get; set; }
        protected bool WasIntPropertyCalled { get; private set; }
        
        public TBuilder WithIntProperty(int intProperty)
        {
            this.IntProperty = intProperty;
            this.WasIntPropertyCalled = true;
            return This;
        }
        
        protected int IntProperty2 { get; set; }
        protected bool WasIntProperty2Called { get; private set; }
        
        public TBuilder WithIntProperty2(int intProperty2)
        {
            this.IntProperty2 = intProperty2;
            this.WasIntProperty2Called = true;
            return This;
        }
        
        protected long IntProperty3 { get; set; }
        protected bool WasIntProperty3Called { get; private set; }
        
        public TBuilder WithIntProperty3(long intProperty3)
        {
            this.IntProperty3 = intProperty3;
            this.WasIntProperty3Called = true;
            return This;
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            if(this.WasIntPropertyCalled)
            {
                target.IntProperty = this.IntProperty;
            }
            
            if(this.WasIntProperty2Called)
            {
                target.IntProperty2 = this.IntProperty2;
            }
            
            if(this.WasIntProperty3Called)
            {
                target.IntProperty3 = this.IntProperty3;
            }
            
            return target;
        }
    }
}
