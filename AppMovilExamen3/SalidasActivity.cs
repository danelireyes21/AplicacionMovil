using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Json;

namespace AppMovilExamen3
{
    [Activity(Label = "SalidasActivity")]
    public class SalidasActivity : Activity
    {
        List<Estacionamientos> estacionamiento;
        List<ElementosTabla> items;
        double res;
        EditText txtNombre, txtTipo, txtEntrada;
        TextView lblP;
        double res1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.SalidaVehiculo);
            var txtEspacio = FindViewById<EditText>(Resource.Id.txtespacioS);
            var btnBuscar = FindViewById<ImageView>(Resource.Id.btnBuscar);
            var btnCobrar = FindViewById<Button>(Resource.Id.btnCobrar);
            var Imagen = FindViewById<ImageView>(Resource.Id.imagenS);
            var txtSalida = FindViewById<EditText>(Resource.Id.txtSalidaS);
            var txtTiempo = FindViewById<EditText>(Resource.Id.txttiempoS);
            var txtPago = FindViewById<EditText>(Resource.Id.txtpagoS);
            txtTipo = FindViewById<EditText>(Resource.Id.txttipoS);
            txtNombre = FindViewById<EditText>(Resource.Id.txtnombreS);
            txtEntrada = FindViewById<EditText>(Resource.Id.txtEntradaS);
            lblP = FindViewById<TextView>(Resource.Id.lblPrecio);
            var btnRegresarS = FindViewById<ImageButton>(Resource.Id.btnRegresarS);
            var btnSalidaF = FindViewById<ImageView>(Resource.Id.btnSalidaF);

            btnSalidaF.Click += delegate
            {
                txtSalida.Text = DateTime.Now.ToString("hh:mm ");
            };

            btnRegresarS.Click += delegate
            {
                StartActivity(typeof(OpcionesActivity));
                Finish();
            };

            btnBuscar.Click += async delegate
            {
                try
                {
                    var CuentadeAlmacenamiento = CloudStorageAccount.Parse
                  ("");
                    var ClienteBlob = CuentadeAlmacenamiento.CreateCloudBlobClient();
                    var carpeta = ClienteBlob.GetContainerReference("imagenes");
                    var resourceBlob = carpeta.GetBlockBlobReference(txtEspacio.Text + ".jpg");
                    string carp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string arch = (txtEspacio.Text + ".jpg");
                    string ruta = System.IO.Path.Combine(carp, arch);
                    var stream = File.OpenWrite(ruta);
                    await resourceBlob.DownloadToStreamAsync(stream);
                    Android.Net.Uri
                    rutaImagen = Android.Net.Uri.Parse(ruta);
                    Imagen.SetImageURI(rutaImagen);

                    var API = "" + txtEspacio.Text;
                    JsonValue json = await DatosA(API);
                    Transform(json);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            btnCobrar.Click += delegate
            {
                //Variables para las horas
                string horasminutos = Horas(txtEntrada.Text, txtSalida.Text);
                int hora = int.Parse(horasminutos.Split(":")[0]);
                double pago = 0;

                var estacionamiento = new Estacionamientos("Espacio", txtTipo.Text);
                estacionamiento.Tiempo = horasminutos;

                var API = "";
                var request = (HttpWebRequest)WebRequest.Create(API);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseText = reader.ReadToEnd();
                res = double.Parse(responseText);

                //Para dar el precio/pago
                for (int i = 0; i < hora; i++)
                {
                    pago += res1;
                }
                estacionamiento.Pago = pago;

                txtTiempo.Text = horasminutos;
                txtPago.Text = pago.ToString();
            };
        }

        public async Task<JsonValue> DatosA(string API)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(API));
            request.ContentType = "application/json";
            request.Method = "GET";
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    var jsondoc = await Task.Run(() => JsonValue.Load(stream));
                    return jsondoc;
                }
            }
        }

        public void Transform(JsonValue json)
        {
            try
            {
                var Resultado = json[0];
                txtTipo.Text = Resultado["tipo"];
                txtNombre.Text = Resultado["nombre"];
                txtEntrada.Text = Resultado["entrada"];
                res1 = double.Parse(Resultado["precio"].ToString());
                lblP.Text = "$"+res1.ToString();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        //Metodo para  dar la resta de las horas y minutos automaticamente.
        public string Horas(string entrada, string salida)
        {
            double entradahora = double.Parse(entrada.Split(":")[0]);
            double entradaminuto = double.Parse(entrada.Split(":")[1]);

            double salidahora = double.Parse(salida.Split(":")[0]);
            double salidaminuto = double.Parse(salida.Split(":")[1]);

            double horafinal = entradahora - salidahora;
            double minutofinal = entradaminuto - salidaminuto;
            string final = $"{Math.Abs(horafinal)}:{Math.Abs(minutofinal)}";
            return final;
        }
      
        public class ElementosTabla
        {
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public int Lugar { get; set; }
            public string Imagen { get; set; }
            public string HoraEntrada { get; set; }
            public string HoraSalida { get; set; }
            public string Costo { get; set; }
            public string Tiempo { get; set; }
            public double Pago { get; set; }
        }

        public class Estacionamientos : TableEntity
        {
            public Estacionamientos(string Espacio, string Lugar)
            {
                PartitionKey = Espacio;
                RowKey = Lugar;
            }

            public Estacionamientos() { }
            public string Tipo { get; set; }
            public string Nombre { get; set; }
            public string HoraEntrada { get; set; }
            public string HoraSalida { get; set; }
            public string Costo { get; set; }
            public string Tiempo { get; set; }
            public string Imagen { get; set; }
            public double Pago { get; set; }
        }
    }
}