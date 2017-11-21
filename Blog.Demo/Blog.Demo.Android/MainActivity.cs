using Android.App;
using Android.Content.PM;
using Android.OS;
using Java.Lang;
using Unity;
using Prism.Unity;

namespace Blog.Demo.Droid
{
    [Activity(Label = "Blog.Demo", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                base.OnCreate(bundle);

                global::Xamarin.Forms.Forms.Init(this, bundle);
                LoadApplication(new App(new AndroidInitializer()));
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            // Register any platform specific implementations
        }
    }
}

