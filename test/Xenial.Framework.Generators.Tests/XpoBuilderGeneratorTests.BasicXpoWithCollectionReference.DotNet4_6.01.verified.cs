//HintName: BasicXpoWithCollectionReference.XpoObject.Builder.g.cs
// <auto-generated />

using System;
using System.Runtime.CompilerServices;

namespace MyProject
{
    [CompilerGenerated]
    internal partial class XpoObjectBuilder : XpoObjectBuilder<MyProject.XpoObject, XpoObjectBuilder> { }
    
    [CompilerGenerated]
    internal abstract partial class XpoObjectBuilder<TClass, TBuilder>
        where TClass : MyProject.XpoObject
        where TBuilder : XpoObjectBuilder<TClass, TBuilder>
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
                return (TClass)new MyProject.XpoObject(this.Session);
            }
            
            throw new System.InvalidOperationException($"Could not create instance of type [MyProject.XpoObject] without a Session.{System.Environment.NewLine}Make sure to use the [WithSession] method when using the [{this.GetType().FullName}] type.");
        }
        
        protected string StringProperty { get; set; }
        protected bool WasStringPropertyCalled { get; private set; }
        
        public TBuilder WithStringProperty(string stringProperty)
        {
            this.StringProperty = stringProperty;
            this.WasStringPropertyCalled = true;
            return This;
        }
        
        private System.Collections.Generic.IList<MyProject.ReferenceXpoBuilder> _ReferencesBuildersCollection = new System.Collections.Generic.List<MyProject.ReferenceXpoBuilder>();
        protected System.Collections.Generic.IList<MyProject.ReferenceXpoBuilder> ReferencesBuildersCollection { get { return _ReferencesBuildersCollection; } }
        
        public TBuilder WithReferences(Action<MyProject.ReferenceXpoBuilder> referencesBuilder)
        {
            if(referencesBuilder != null)
            {
                MyProject.ReferenceXpoBuilder builder = new MyProject.ReferenceXpoBuilder();
                this.WithReferences(builder);
                referencesBuilder.Invoke(builder);
            }
            return This;
        }
        
        public TBuilder WithReferences(MyProject.ReferenceXpoBuilder references)
        {
            if(this.WasSessionSet)
            {
                references.WithSession(this.Session);
            }
            this.ReferencesBuildersCollection.Add(references);
            return This;
        }
        
        private System.Collections.Generic.IList<MyProject.ReferenceXpo> _ReferencesCollection = new System.Collections.Generic.List<MyProject.ReferenceXpo>();
        protected System.Collections.Generic.IList<MyProject.ReferenceXpo> ReferencesCollection { get { return _ReferencesCollection; } }
        
        public TBuilder WithReferences(MyProject.ReferenceXpo references)
        {
            this.ReferencesCollection.Add(references);
            return This;
        }
        
        public virtual TClass Build()
        {
            TClass target = this.CreateTarget();
            
            if(this.WasStringPropertyCalled)
            {
                target.StringProperty = this.StringProperty;
            }
            
            foreach(MyProject.ReferenceXpoBuilder item in this.ReferencesBuildersCollection)
            {
                this.WithReferences(item);
            }
            
            foreach(MyProject.ReferenceXpo item in this.ReferencesCollection)
            {
                this.References.Add(item);
            }
            
            return target;
        }
    }
}
