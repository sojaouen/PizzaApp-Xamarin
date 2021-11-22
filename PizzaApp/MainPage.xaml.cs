using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaApp.Model;
using Xamarin.Forms;

namespace PizzaApp
{
    public partial class MainPage : ContentPage
    {

        private enum Etri { TRI_AUCUN, TRI_NOM, TRI_PRIX };
        private Etri triCourant = Etri.TRI_AUCUN;

        private List<Pizza> pizzas = new List<Pizza>();
        private String pizzaJSON;// = "[{ \"Nom\": \"MOMENT OF TRUFFE\",  \"Prix\":14.50,  \"Ingredients\":[\"crème de truffe noire\",\"ricotta\", \"mozza fior di latte\", \"truffe fraîche de saison\", \"carpaccio de champignons\",\"ciboulette\"],  \"ImageURL\":\"https://www.lafelicita.fr/wp-content/uploads/2018/04/pizza-truffe-resized.jpg\"}," +
                                 //"{  \"Nom\": \"MAMMARGHERITA\",  \"Prix\":10.50,  \"Ingredients\":[\"mozza fior di latte\",\"sauce de tomates San Marzano\", \"parmigiano\", \"basilic frais\"], \"ImageURL\":\"https://www.lafelicita.fr/wp-content/uploads/2018/04/pizza-mozza-resized.jpg\"}," +
                                 //"{  \"Nom\": \"4EVER\",  \"Prix\":12.50,  \"Ingredients\":[\"crème de gorgonzola\",\"mozza fior di latte\", \"caciocavallo\", \"parmigiano\", \"basilic frais\"],  \"ImageURL\":\"https://images.unsplash.com/photo-1594007654729-407eedc4be65?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=856&q=80\"}," +
                                 //"{  \"Nom\": \"TA REINE CE SOIR\",  \"Prix\":11.50,  \"Ingredients\":[\"mozza fior di latte\",\"crème de champignons\", \"jambon aux herbes\", \"champignons portobello grillés\", \"olives taggiasche\", \"pousses de moutarde et basilic frais\"],  \"ImageURL\":\"https://www.lafelicita.fr/wp-content/uploads/2018/05/stationf-75492.jpg\"}," +
                                 //"{  \"Nom\": \"VEGGIE HADID\",  \"Prix\":13.50,  \"Ingredients\":[\"mozza fior di latte\",\"parmigiano\", \"pleurotes\", \"pecorino\", \"gorgonzola\", \"roquette\", \"noix de pécan et miel on top\", \"basilic frais\"],  \"ImageURL\":\"https://images.unsplash.com/photo-1574126154517-d1e0d89ef734?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=1674&q=80\"},]";

        private string jsonFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pizzas.json");
        private string jsonFilenameCopy = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pizzas.jsonCopy");
        private List<String> pizzasFav = new List<string>();

        public MainPage()
        {
            InitializeComponent();
            this.pizzasFav.Add("4 fromages");
            this.pizzasFav.Add("indienne");

            //pizzas.Add(new Pizza
            //{
            //    Nom = "MOMENT OF TRUFFE",
            //    Prix = 5.99f,
            //    Ingredients = new String[] {"crème de truffe noire", "ricotta", "mozza fior di latte", "truffe fraîche de saison", "carpaccio de champignons", "ciboulette"},
            //    ImageURL= "https://www.lafelicita.fr/wp-content/uploads/2018/04/pizza-truffe-resized.jpg"
            //});

            //pizzas.Add(new Pizza
            //{
            //    Nom = "MAMMARGHERITA",
            //    Prix = 5.99f,
            //    Ingredients = new String[] { "mozza fior di latte", "sauce de tomates San Marzano", "parmigiano", "basilic frais" },
            //    ImageURL = "https://www.lafelicita.fr/wp-content/uploads/2018/04/pizza-mozza-resized.jpg"
            //});

            //pizzas.Add(new Pizza
            //{
            //    Nom = "4EVER",
            //    Prix = 5.99f,
            //    Ingredients = new String[] { "crème de gorgonzola", "mozza fior di latte", "caciocavallo", "parmigiano", "basilic frais" },
            //    ImageURL = "https://images.unsplash.com/photo-1594007654729-407eedc4be65?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=856&q=80"
            //});

            //pizzas.Add(new Pizza
            //{
            //    Nom = "TA REINE CE SOIR",
            //    Prix = 5.99f,
            //    Ingredients = new String[] { "mozza fior di latte", "crème de champignons", "jambon aux herbes",
            //    "champignons portobello grillés", "olives taggiasche", "pousses de moutarde et basilic frais" },
            //    ImageURL = "https://www.lafelicita.fr/wp-content/uploads/2018/05/stationf-75492.jpg"
            //});

            //pizzas.Add(new Pizza
            //{
            //    Nom = "VEGGIE HADID",
            //    Prix = 5.99f,
            //    Ingredients = new String[] { "mozza fior di latte", "parmigiano", "pleurotes", "pecorino", "gorgonzola",
            //    "roquette", "noix de pécan et miel on top", "basilic frais"  },
            //    ImageURL = "https://images.unsplash.com/photo-1574126154517-d1e0d89ef734?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=1674&q=80"
            //});
            if (Application.Current.Properties.ContainsKey("tri"))
            {
                this.triCourant = (Etri)Application.Current.Properties["tri"];
                this.sortButton.Source = GetImageFromSort();
            }
            this.pizzaListView.IsVisible = false;
            this.waitLayout.IsVisible = true;

            if (File.Exists(this.jsonFilenameCopy))
            {
                pizzaJSON = File.ReadAllText(jsonFilenameCopy);
                if (!String.IsNullOrEmpty(pizzaJSON))
                {
                    pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzaJSON);
                    pizzaListView.ItemsSource = GetPizzaCells(GetPizzasFromSort(), pizzasFav);
                    pizzaListView.IsVisible = true;
                    waitLayout.IsVisible = false;
                }
            }

            DownloadData();
            

            // webclient.Dispose();
            //pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzaJSON);
            //this.pizzaListView.ItemsSource = pizzas;


            this.pizzaListView.RefreshCommand = new Command((obj) =>
            {
                Console.WriteLine("RefreshCommand");
                DownloadData();
            });

        }

        public void DownloadData()
        {
            using (var webclient = new WebClient())
            {
                const string URL = "https://drive.google.com/uc?export=download&id=1YVa8DNIuESXJI8sM4sxcd-w-rk6xpFIZ";

                try
                {
                    //pizzaJSON = webclient.DownloadString(URL);
                    //webclient.DownloadStringCompleted += Webclient_DownloadStringCompleted;
                    //webclient.DownloadStringAsync(new Uri(URL));
                    webclient.DownloadFileCompleted += Webclient_DownloadFileCompleted;
                    webclient.DownloadFileAsync(new Uri(URL), this.jsonFilename);

                }

                catch (Exception ex)
                {
                    DisplayAlert("Accès web", "Erreur d'accès : " + ex.Message, "OK");
                    return;
                }
            }
        }

        private void Webclient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
             Exception ex = e.Error;
                if (ex == null)
                { // pas d'erreur
                    this.pizzaJSON = File.ReadAllText(this.jsonFilename);
                    if(File.Exists(this.jsonFilenameCopy))
                        File.Delete(this.jsonFilenameCopy);

                    File.Copy(this.jsonFilename, this.jsonFilenameCopy);
                    this.pizzas = JsonConvert.DeserializeObject<List<Pizza>>(this.pizzaJSON);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        pizzaListView.ItemsSource = GetPizzaCells(GetPizzasFromSort(), pizzasFav);
                        pizzaListView.IsVisible = true;
                        waitLayout.IsVisible = false;
                        pizzaListView.IsRefreshing = false;
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Erreur", "Erreur survenue " + ex.Message, "OK");
                    });
                }
        }

        private void Webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("Données chargée " + e.Result);
                pizzaJSON = e.Result;
            }

            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Accès web", "Erreur d'accès au retour : " + ex.Message, "ok");
                });
                return;
            }
           this.pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzaJSON);
            Device.BeginInvokeOnMainThread(() =>
            {
                //pizzaListView.ItemsSource = pizzas;
                this.pizzaListView.ItemsSource = GetPizzaCells(GetPizzasFromSort(), pizzasFav); ;
                this.pizzaListView.IsVisible = true;
                this.waitLayout.IsVisible = false;
                this.pizzaListView.IsRefreshing = false;
            });
        }

        private void sortButton_Clicked(Object sender, EventArgs e)
        {
            if (this.triCourant == Etri.TRI_AUCUN)
                this.triCourant = Etri.TRI_NOM;
            else if (this.triCourant == Etri.TRI_NOM)
                this.triCourant = Etri.TRI_PRIX;
            else if (this.triCourant == Etri.TRI_PRIX)
                this.triCourant = Etri.TRI_AUCUN;

            this.sortButton.Source = GetImageFromSort();
            this.pizzaListView.ItemsSource = GetPizzaCells(GetPizzasFromSort(), pizzasFav);

            Application.Current.Properties["tri"] = (int)triCourant;
            Application.Current.SavePropertiesAsync();

        }

        public string GetImageFromSort()
        {
            switch(this.triCourant)
            {
                case Etri.TRI_AUCUN:
                    return "sort_none.png";
                case Etri.TRI_NOM:
                    return "sort_nom.png";
                case Etri.TRI_PRIX:
                    return "sort_prix.png";
                default: return "";

            }
        }


        public List<Pizza> GetPizzasFromSort()
        {
            List<Pizza> listToReturn = null;
            switch (this.triCourant)
            {
                case Etri.TRI_AUCUN:
                    listToReturn = this.pizzas;
                    break;
                case Etri.TRI_NOM:
                    listToReturn = new List<Pizza>(pizzas);
                    listToReturn.Sort((pizza1, pizza2) => { return pizza1.Nom.CompareTo(pizza2.Nom); });
                    break;
                case Etri.TRI_PRIX:
                    listToReturn = new List<Pizza>(pizzas);
                    listToReturn.Sort((pizza1, pizza2) => { return pizza1.Prix.CompareTo(pizza2.Prix); });
                    break;
                default:
                    listToReturn = null;
                    break;

            }
            return listToReturn;
        }

        private List<PizzaCell> GetPizzaCells(List<Pizza> pizzas, List<String> favorites)
        {
            List<PizzaCell> returnList = new List<PizzaCell>();
            if (pizzas == null)
                return returnList;
            foreach (Pizza p in pizzas)
            {
                bool isFavorite = favorites.Contains(p.Nom);
                PizzaCell pc = new PizzaCell(p, isFavorite);
                returnList.Add(pc);
            }
            return returnList;
        }
    }
}