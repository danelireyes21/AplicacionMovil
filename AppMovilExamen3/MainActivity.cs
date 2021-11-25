using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Plugin.Media;
using System.IO;
using Plugin.CurrentActivity;
using Android.Graphics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Net;

namespace AppMovilExamen3
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        string Archivo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            SupportActionBar.Hide();//quita barra superior
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            var Imagen = FindViewById<ImageView>(Resource.Id.imagen);
            var btnAlmacenar = FindViewById<Button>(Resource.Id.btnAlmacenar);
            var txtEspacio = FindViewById<EditText>(Resource.Id.txtespacio);
            var txtTipo = FindViewById<EditText>(Resource.Id.txttipo);
            var txtNombre = FindViewById<EditText>(Resource.Id.txtnombre);
            var txtEntrada = FindViewById<EditText>(Resource.Id.txtEntrada);
            var btnRegresarE = FindViewById<ImageView>(Resource.Id.btnRegresarE);
            var txtCosto = FindViewById<EditText>(Resource.Id.txtCostoP);
            var btnFecEntrada = FindViewById<ImageButton>(Resource.Id.btnFecEntrada);

            var API = "";
            var request = (HttpWebRequest)WebRequest.Create(API);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseText = reader.ReadToEnd();
            txtCosto.Text = "Costo General $ " + responseText.ToString() + " / HORA";

            btnFecEntrada.Click += delegate
            {
                txtEntrada.Text = DateTime.Now.ToString("hh:mm ");
            };

            btnRegresarE.Click += delegate
            {
                StartActivity(typeof(OpcionesActivity));
                Finish();
            };

            Imagen.Click += async delegate
            {
                await CrossMedia.Current.Initialize();
                var archivo = await CrossMedia.Current.TakePhotoAsync
                (new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Imagenes",
                    Name = txtEspacio.Text,
                    SaveToAlbum = true,
                    CompressionQuality = 30,
                    CustomPhotoSize = 30,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front
                });
                if (archivo == null)
                    return;
                Bitmap bp = BitmapFactory.DecodeStream(archivo.GetStream());
                Archivo = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), txtEspacio.Text + ".jpg");
                var stream = new FileStream(Archivo, FileMode.Create);
                bp.Compress(Bitmap.CompressFormat.Jpeg, 30, stream);
                stream.Close();
                Imagen.SetImageBitmap(bp);
            };

            btnAlmacenar.Click += async delegate 
            {
                var CuentaAlmacenamiento = CloudStorageAccount.Parse("");
                var ClienteBlob = CuentaAlmacenamiento.CreateCloudBlobClient();
                var Carpeta = ClienteBlob.GetContainerReference("imagenes");
                var resourceBlob = Carpeta.GetBlockBlobReference(txtEspacio.Text + ".jpg");
                resourceBlob.Properties.ContentType = "image/jpeg";
                await resourceBlob.UploadFromFileAsync(Archivo.ToString());
                Toast.MakeText(this, "Imagen Almacenada Correctamente en Contenedor de Azure", ToastLength.Long).Show();

                var TablaNoSQL = CuentaAlmacenamiento.CreateCloudTableClient();
                var Coleccion = TablaNoSQL.GetTableReference("estacionamientoss");

                await Coleccion.CreateIfNotExistsAsync();
                
                var estacionamiento = new Estacionamientos("Espacio", txtEspacio.Text);
                estacionamiento.Tipo = txtTipo.Text;
                estacionamiento.Nombre = txtNombre.Text;
                estacionamiento.HoraEntrada = txtEntrada.Text;
                estacionamiento.Costo = double.Parse(responseText);

                estacionamiento.Imagen = "" + txtEspacio.Text + ".jpg";
                var Store = TableOperation.Insert(estacionamiento);
                await Coleccion.ExecuteAsync(Store);

                var API = "" + txtEspacio.Text + "&Nombre="+txtNombre.Text+"&Tipo="+txtTipo.Text+"&Entrada="+txtEntrada.Text+"&Precio="+estacionamiento.Costo+"";
                var request = (HttpWebRequest)WebRequest.Create(API);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseText2 = reader.ReadToEnd();

                Toast.MakeText(this, "Datos Almacenados en Tabla NoSQL", ToastLength.Long).Show();
            };
        }
       
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public class Estacionamientos : TableEntity
        {
            public Estacionamientos(string Espacio, string Lugar)
            {
                PartitionKey = Espacio;
                RowKey = Lugar;
            }
            public string Tipo { get; set; }
            public string Nombre { get; set; }
            public string HoraEntrada { get; set; }
            public double Costo { get; set; }
            public string HoraSalida { get; set; }
            public string Tiempo { get; set; }
            public double Pago { get; set; }
            public string Imagen { get; set; }
        }
    }
}