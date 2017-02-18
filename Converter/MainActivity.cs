using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            //ActionBar.SetIcon(Resource.Drawable.Icon);

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
                string url = "https://openexchangerates.org/api/latest.json?app_id=79ea982fc55142519d6070fc8b066c9f";
                // recebe resposta, parse informacao e mostra na tela resultado:
                JsonValue json = await consultando(url);
                // parse e mostra (json);
                mostrando(json);
            };

        }
        
        private async Task<JsonValue> consultando(string url)
        {

            Toast.MakeText(this, "Consultando WebService...", ToastLength.Short).Show();
            real.Text = "";
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
                    //string content = stream.
                    // Use this stream to build a JSON document object:
                    Console.Out.WriteLine("Stream aqui: {0}", stream.ToString());
                    JsonValue jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }

        private void mostrando(JsonValue json)
        {
            JsonValue valorEscolhido = json["rates"][escolha];
            JsonValue valorReal = json["rates"]["BRL"];
            JsonValue data = json["timestamp"];

            double moedaEscolhida = valorEscolhido;
            double realAtual = valorReal;

            //deixando apenas 2 casas decimais
            moedaEscolhida = Math.Truncate(moedaEscolhida * 100) / 100;
            realAtual = Math.Truncate(realAtual * 100) / 100;

            double dolar = 1; //api a base e dolar
            moedaEscolhida = dolar - (moedaEscolhida - dolar);
            double valorConvertidoParaReal = Math.Truncate((moedaEscolhida * realAtual) * 100) /100;
            real.Text = "R$ "+ (double.Parse(qtd.Text) * valorConvertidoParaReal).ToString();
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
                case "EUR":
                    imageCurrency.SetImageResource(Resource.Drawable.euro);
                    break;
                case "GBP":
                    imageCurrency.SetImageResource(Resource.Drawable.libra);
                    break;
            }
                
        }
    }
}

