using System;
using System.Collections.Generic;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Utils.Commands;

using MailClient.Module.BusinessObjects;
using MailClient.Module.Domain;

namespace MailClient.Module
{
    public class ReceiveMailsViewController : ViewController
    {
        public SimpleAction ReceiveMailsSimpleAction { get; }
        public ReceiveMailsViewController()
        {
            TargetObjectType = typeof(Mail);
            ReceiveMailsSimpleAction = new(this, nameof(ReceiveMailsSimpleAction), DevExpress.Persistent.Base.PredefinedCategory.Edit)
            {
                Caption = "Receive",
            };
            ReceiveMailsSimpleAction.Execute += ReceiveMailsSimpleAction_Execute;
        }

        private async void ReceiveMailsSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var receiver = new ImapMailReceiver((t) => Application.CreateObjectSpace(t));
            using var os = Application.CreateObjectSpace(typeof(MailAccount));
            foreach (var mailAccount in os.GetObjects<MailAccount>())
            {
                await foreach (var mail in receiver.ReceiveAsync(mailAccount.Id))
                {

                }
            }
        }
    }
}
