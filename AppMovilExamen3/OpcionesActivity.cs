using Android.App;
using Android.OS;
using Android.Widget;

namespace AppMovilExamen3
{
    [Activity(Label = "OpcionesActivity")]
    public class OpcionesActivity : Activity
    {
        public object SupportActionBar { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Opciones);
            var btnEntrada = FindViewById<Button>(Resource.Id.btnEntradas);
            var btnSalida = FindViewById<Button>(Resource.Id.btnSalidas);
            var btnVerV = FindViewById<Button>(Resource.Id.btnVerVehiculos);

            btnEntrada.Click += delegate
            {
                StartActivity(typeof(MainActivity));
                Finish();
            };

            btnSalida.Click += delegate
            {
                StartActivity(typeof(SalidasActivity));
                Finish();
            };

            btnVerV.Click += delegate
            {
                StartActivity(typeof(VerVehiculosActivity));
                Finish();
            };

        }
    }
}