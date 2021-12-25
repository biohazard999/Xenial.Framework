﻿
using Xenial.Framework.Layouts;
using Xenial.Framework.Layouts.Items.Base;

namespace MailClient.Module.BusinessObjects
{
    public partial class MailLayoutBuilder : LayoutBuilder<Mail>
    {
        public Layout BuildLayout() => new()
        {
            Editor.Account,
            Editor.Subject,
            Editor.From,
            Editor.To,
            Editor.CC,
            Editor.AttachmentCount
        };
    }
}
