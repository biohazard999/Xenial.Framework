using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.SystemModule;

namespace MainDemo.Module.Win.Controllers {
    public class UpdateActionsOnAsyncLoadingController : AsyncLoadingIndicationController {
        protected override void UpdateActions(bool isEnabled) {
            base.UpdateActions(isEnabled);
            UpdateAction(Frame.GetController<ChangeVariantController>()?.ChangeVariantAction, isEnabled);
            UpdateAction(Frame.GetController<FileAttachmentListViewController>()?.AddFromFileAction, isEnabled);
        }
    }
}
