using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MichelleTool
{
    public class CurrentModule
    {
        public static MngLanguage LanguageMng;
        public static RegistryFile RegistryMng = null;
        public static RibbonManage RibbonManage = null;
        public static ResourcesAssembly ResourcesAssemblyMng = null;
        public static Brush BgColor = null;
        public static BitmapImage GetBitmapImage(string icon)
        {
            return new BitmapImage(new Uri("pack://application:,,,/MichelleTool;component/Resources/" + icon));
        }
        public static void InitialTemplate(UIControlledApplication application, string pathRegistry = @"Software\MichelleTool\RVT")
        {
            ResourcesAssemblyMng = new ResourcesAssembly(Assembly.GetExecutingAssembly());
            LanguageMng = new MngLanguage();
            MngLanguage.ChangeLanguage(application.ControlledApplication.Language == LanguageType.Japanese ? "ja-JP" : "en-US");
            RegistryMng = new RegistryFile(pathRegistry);
            RibbonManage = new RibbonManage(application);
            var brushConverter = new BrushConverter();
            BgColor = (Brush)brushConverter.ConvertFromString("#f5f5f5");
        }

        public static string keyDimentionJson = "DimensionSettings";
    }
}
