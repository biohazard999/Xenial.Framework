using System;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Editors
{
    [Persistent]
    public partial class WebViewHtmlStringEditorDemo : FeatureCenterEditorsBaseObject
    {
        private string? html;
        private string? htmlContent;

        public WebViewHtmlStringEditorDemo(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            ResetDemo();
        }

        [Size(SizeAttribute.Unlimited)]
        [ImmediatePostData]
        public string? HtmlContent
        {
            get => htmlContent;
            set
            {
                if (SetPropertyValue(ref htmlContent, value) && IsSaveForBusinessLogic)
                {
                    Html = value;
                }
            }
        }

        [WebViewHtmlStringEditor]
        [Size(SizeAttribute.Unlimited)]
        public string? Html { get => html; set => SetPropertyValue(ref html, value); }

        [Action(ImageName = "BO_Audit_ChangeHistory", Caption = "Reset Demo")]
        public void ResetDemo() => HtmlContent = $@"<!doctype html>
<html lang=""en"">
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <link href='https://unpkg.com/bulma@0.9.1/css/bulma.min.css' rel='stylesheet' />
  </head>
  <body>
    <section class='section'>
      <div class='container'>
        <h1 class='title'>{ nameof(WebViewHtmlStringEditorDemo) }</h1>
        <div class='content'>
            <p>Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.</p>
            <ul>
                <li>Duis autem vel eum iriure</li>
                <li>dolor in hendrerit in vulputate velit esse</li>
                <li>molestie consequat, vel illum dolore eu feugiat nulla</li>
            </ul>
            <ol>
                <li>facilisis at vero eros et accumsan</li>
                <li>et iusto odio dignissim qui blandit praesent</li>
                <li>luptatum zzril delenit augue duis dolore</li>
            </ol>
            <h1>vulputate velit esse molestie consequat</h1>
            <h2>Lorem ipsum dolor sit amet</h2>
            <h3>te feugait nulla facilisi</h3>
            <h4>Lorem ipsum dolor sit amet</h4>
            <h5>consectetuer adipiscing elit</h5>
            <h6>sed diam nonummy nibh euismod tincidunt</h6>
            <dl>
                <dt>ut laoreet dolore magna aliquam erat volutpat.</dt>
                <dd>Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl</dd>
            </dl>
            <blockquote>ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.</blockquote>
            <p>Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.</p>
        </div>
      </div>
    </section>
  </body>
</html>";

    }
}
