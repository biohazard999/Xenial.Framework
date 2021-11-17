namespace MyProject
{
    [Xenial.XenialImageNames(Sizes = true, SmartComments = true)]
    public partial class ImageNamesWithSizes
    {
    }


    [XafAction(Caption = "Foo")]
    public class MyAction : Action<TargetType>
    {
        public DetailView Execute(XafApplication application)
        {
            CurrentObject.Foo = "Blah";

            ObjectSpace.CommitChanges();

            var os = Application.CreateObjectSpace(typeof(NestedObject));
            var dv = Application.CreateDetailView(os, CurrentObject.NestedObj); //Could be made easier, by just returning a new Type instead of returning a custom view, but its just an idea
            return dv;
        }
}
