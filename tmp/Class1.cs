using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;

using Xenial;

namespace MyProject
{
    [DomainComponent]
    public class Class1 : NonPersistentLiteObject
    {
        private string myProperty;

        public string MyProperty { get => myProperty; set => SetPropertyValue(ref myProperty, value); }
    }

    [XenialAction(Caption = "Do some stuff")]
    public partial class Class1Action : IDetailViewAction<Class1>
    {
        public partial void Execute(
            Class1 myTarget,
            IObjectSpace objectSpace,
            XafApplication application
        )
        {
            myTarget.MyProperty = new Random().Next().ToString();
        }
    }

    // [Xenial.XenialImageNames(
    //     Sizes = false,
    //     SmartComments = true,
    //     ResourceAccessors = true
    // )]
    // public static partial class ImageNamesWithSizes
    // {
    //     static ImageNamesWithSizes()
    //     {
    //         var foo = ImageNamesWithSizes.aac;
    //         var x = ImageNamesWithSizes.AsImage.aac();

    //     }
    // }

    // public class MyTarget
    // {
    //     public const string MyImage = "ABC";
    // }

    // [XenialAction(
    //     Caption = "Foo",
    //     ImageName = MyTarget.MyImage,
    //     Category = "View"
    // )]
    // public partial class MyAction : IDetailViewAction<MyTarget>
    // {
    //     public partial void Execute(MyTarget myTarget) => throw new NotImplementedException();
    // }

    //public class FooController : ViewController
    //{
    //    public FooController()
    //    {
    //        this.TargetViewType = ViewType.DetailView;

    //        SimpleAction x = new()
    //        {
    //            SelectionDependencyType = SelectionDependencyType.RequireSingleObject
    //        };
    //        x.Execute += X_Execute;
    //    }

    //    private void X_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //    }

    //    protected override void OnActivated() => base.OnActivated();

    //    protected override void OnDeactivated() => base.OnDeactivated();
    //}
}
