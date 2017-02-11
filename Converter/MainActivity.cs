using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Json;

namespace Converter
{
    [Activity(Label = "Conversor de Moeda", MainLauncher = true, Icon = "@drawable/Icon", Theme = "@style/ConverterTheme")]
    public class MainActivity : Activity
    {

        private ImageView imageCurrency;
        private Spinner spinner;
        private EditText qtd;
        private TextView real; 
        private Button converter;
        private string escolha = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            
            //icon da bar
            ActionBar.SetIcon(Resource.Drawable.Icon);

            spinner = FindViewById<Spinner>(Resource.Id.spinnerCurrency);
            converter = FindViewById<Button>(Resource.Id.buttonConvert);
            imageCurrency = FindViewById<ImageView>(Resource.Id.imageCurrency);
            qtd = FindViewById<EditText>(Resource.Id.textQuantity);
            real = FindViewById<TextView>(Resource.Id.textReal);

            //cria o spinner
            spinner.ItemSelected += new System.EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.currency_list, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;

            //cria o metodo click do botao
            converter.Click += async (sender, e) => {

                    // Consulta api com o valor escolhido para conversao
                    string url = "http://api.promasters.net.br/cotacao/v1/valores?moedas=" +
                                 escolha +
                                 "&alt=json";

                    // recebe resposta, parse informacao e mostra na tela resultado:
                    JsonValue json = await consultando(url);
                    // parse e mostra (json);
                    mostrando(json);
                };
        }

        private async Task<JsonValue> consultando(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }

        private void mostrando(JsonValue json)
        {
            JsonValue valores = json["valores"];

            var moeda = valores[escolha];

            double valor = moeda["valor"];

            real.Text = (double.Parse(qtd.Text) * valor).ToString();
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            escolha = spinner.GetItemAtPosition(e.Position).ToString();
            if (!escolha.Equals("Escolha uma moeda"))
            {
                string[] resposta = escolha.Split('-');
                escolha = resposta[0].Trim();

                SetImageCurrency(escolha);
                imageCurrency.Visibility = Android.Views.ViewStates.Visible;
                qtd.Visibility = Android.Views.ViewStates.Visible;
                
            }else
            {
                escolha = "";
                imageCurrency.Visibility = Android.Views.ViewStates.Invisible;
                qtd.Visibility = Android.Views.ViewStates.Invisible;
                qtd.Text = "";
            }
            
        }

        private void SetImageCurrency(string escolha)
        {
           switch (escolha)
            {
                case "USD":
                    imageCurrency.SetImageResource(Resource.Drawable.dolar);
                    break;
                case "BTC":
                    imageCurrency.SetImageResource(Resource.Drawable.bitcoin);
                    break;
                case "EUR":
                    imageCurrency.SetImageResource(Resource.Drawable.euro);
                    break;
                case "GBT":
                    imageCurrency.SetImageResource(Resource.Drawable.libra);
                    break;
            }
                
        }
    }
}

