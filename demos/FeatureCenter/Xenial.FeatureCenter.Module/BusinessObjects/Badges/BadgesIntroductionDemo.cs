using System;
using System.Linq;

using DevExpress.Xpo;

namespace Xenial.FeatureCenter.Module.BusinessObjects.Badges
{
    [Persistent]
    public partial class BadgesIntroductionDemo : FeatureCenterBadgesBaseObject
    {
        public BadgesIntroductionDemo(Session session) : base(session) { }
    }
}
