using System.Drawing;

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Frames;

namespace Xenial.FeatureCenter.Module.Win
{
    internal static class ColorHelper
    {
        public static Color GetControlColor(UserLookAndFeel provider)
            => LookAndFeelHelper.GetSystemColor(provider, SystemColors.Control);

        public static Color TextColor
            => CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor(CommonColors.ControlText);

        public static Color GetTransparentRowForeColor(UserLookAndFeel lookAndFeel)
        {
            var color = Color.Empty;
            var activeSkin = lookAndFeel.ActiveSkinName;
            if (activeSkin == "VS2010" || activeSkin == "Dark Side" ||
                 activeSkin == "Sharp" || activeSkin == "Pumpkin")
            {
                color = GetControlColor(lookAndFeel);
            }
            else if (FrameHelper.IsDarkSkin(lookAndFeel))
            {
                color = TextColor;
            }
            return color;
        }
    }
}
