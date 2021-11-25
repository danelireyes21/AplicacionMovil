using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using System.Net;

namespace AppMovilExamen3
{
    [Activity(Label = "VerVehiculosActivity")]
    public class VerVehiculosActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.VerVehiculoVista);
            var btnRegresar = FindViewById<ImageView>(Resource.Id.btnRegresarC);
            var btnActualizar = FindViewById<Button>(Resource.Id.btnCosto);
            var txtCostoA = FindViewById<EditText>(Resource.Id.txtCostoActual);
            var txtCostoN = FindViewById<EditText>(Resource.Id.txtCostoNuevo);

            var API = "";
            var request = (HttpWebRequest)WebRequest.Create(API);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseText = reader.ReadToEnd();
            txtCostoA.Text = "$ "+responseText.ToString() + " / HORA";
            txtCostoN.Text = "";

            btnRegresar.Click += delegate
            {
                StartActivity(typeof(OpcionesActivity));
                Finish();
            };

            btnActualizar.Click += delegate
            {
                try
                {
                    var API = "" + txtCostoN.Text+ "";
                    var request = (HttpWebRequest)WebRequest.Create(API);
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseText = reader.ReadToEnd();
                    txtCostoA.Text = "$ "+txtCostoN.Text+" / HORA";
                    txtCostoN.Text = "";
                    Toast.MakeText(this, responseText, ToastLength.Long).Show();
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
        }
    }
}