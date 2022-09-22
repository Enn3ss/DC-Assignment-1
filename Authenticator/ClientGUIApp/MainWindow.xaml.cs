using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Authenticator;
using RestSharp;
using System.ServiceModel;
using Newtonsoft.Json;
using Registry.Models;
using ServiceProvider.Models;

namespace ClientGUIApp {
    public partial class MainWindow : Window {
        public IAuthenticatorServer foob;
        public RestClient client;
        private int token = -1;

        public MainWindow() {
            InitializeComponent();

            ChannelFactory<IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
            client = new RestClient("http://localhost:63273");
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e) {
            string name = NameTextBox.Text;
            string password = PasswordTextBox.Text;

            token = foob.Login(name, password);
            if (token == -1) {
                MessageBox.Show("Incorrect credentials.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                MessageBox.Show("Successfully logged in.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e) {
            SearchData searchData = new SearchData();
            searchData.Search = SearchTextBox.Text;
            searchData.Token = token;

            if (SearchTextBox.Text.Equals("")) {
                MessageBox.Show("Search description cannot be empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                RestRequest request = new RestRequest("api/service/search", Method.Post);
                request.AddJsonBody(searchData);
                RestResponse responce = client.Post(request);
                Registry.Models.StatusData statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(responce.Content);

                if (statusData.Status.Equals("Successful")) {
                    List<ServiceDescription> serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(statusData.Data);
                    ServiceList.ItemsSource = serviceDescriptions;
                }
                else {
                    MessageBox.Show("You need to login first.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e) {
            string name = NameTextBox.Text;
            string password = PasswordTextBox.Text;

            if (name.Equals("") || password.Equals("")) {
                MessageBox.Show("Name and/or password cannot be empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                string responce = foob.Register(name, password);

                if (responce.Equals("unsuccessfully registered")) {
                    MessageBox.Show("Name and password already exists.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }

        private void AllServicesBtn_Click(object sender, RoutedEventArgs e) {
            AllServicesData allServicesData = new AllServicesData();
            allServicesData.Token = token;

            RestRequest request = new RestRequest("api/service/allservices", Method.Post);
            request.AddJsonBody(allServicesData);
            RestResponse responce = client.Post(request);
            Registry.Models.StatusData statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(responce.Content);

            if (statusData.Status.Equals("Successful")) {
                List<ServiceDescription> serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(statusData.Data);
                ServiceList.ItemsSource = serviceDescriptions;
            }
            else {
                MessageBox.Show("You need to login first.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }

        private void ListViewItem_PreviewLeftButtonDown(object sender, RoutedEventArgs e) {
            if (ServiceList.SelectedItem != null) {
                ServiceDescription currItem = (ServiceDescription)ServiceList.SelectedItem;

                // add two items
                if (currItem.APIEndpoint.Equals("http://localhost:56066/AddTwoNumbers")) {
                    OperandOneTextBox.IsEnabled = true;
                    OperandTwoTextBox.IsEnabled = true;
                    OperandThreeTextBox.IsEnabled = false;
                    ResultTextBox.IsEnabled = true;
                    CalculateBtn.IsEnabled = true;
                }
                // add three items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/AddThreeNumbers")) {
                    OperandOneTextBox.IsEnabled = true;
                    OperandTwoTextBox.IsEnabled = true;
                    OperandThreeTextBox.IsEnabled = true;
                    ResultTextBox.IsEnabled = true;
                    CalculateBtn.IsEnabled = true;
                }
                // multiply two items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulTwoNumbers")) {
                    OperandOneTextBox.IsEnabled = true;
                    OperandTwoTextBox.IsEnabled = true;
                    OperandThreeTextBox.IsEnabled = false;
                    ResultTextBox.IsEnabled = true;
                    CalculateBtn.IsEnabled = true;
                }
                // multiple three items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulThreeNumbers")) {
                    OperandOneTextBox.IsEnabled = true;
                    OperandTwoTextBox.IsEnabled = true;
                    OperandThreeTextBox.IsEnabled = true;
                    ResultTextBox.IsEnabled = true;
                    CalculateBtn.IsEnabled = true;
                }
            }
        }

        private void CalculateBtn_Click(object sender, RoutedEventArgs e) {
            ServiceDescription currItem = (ServiceDescription)ServiceList.SelectedItem;

            CalculatorData calculatorData = new CalculatorData();
            int[] operands = { Int32.Parse(OperandOneTextBox.Text), Int32.Parse(OperandTwoTextBox.Text) };
            calculatorData.Token = token;
            calculatorData.Operands = operands;

            RestRequest request;
            // add two items
            if (currItem.APIEndpoint.Equals("http://localhost:56066/AddTwoNumbers")) {
                request = new RestRequest("api/calculator/addtwonumbers", Method.Post);
            }
            // add three items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/AddThreeNumbers")) {
                request = new RestRequest("api/calculator/addthreenumbers", Method.Post);
            }
            // multiply two items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulTwoNumbers")) {
                request = new RestRequest("api/calculator/multwonumbers", Method.Post);
            }
            // multiple three items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulThreeNumbers")) {
                request = new RestRequest("api/calculator/multhreenumbers", Method.Post);
            }
            else {
                request = null;
            }

            request.AddJsonBody(calculatorData);
            RestResponse responce = client.Post(request);
            ServiceProvider.Models.StatusData statusData = JsonConvert.DeserializeObject<ServiceProvider.Models.StatusData>(responce.Content);

            SearchTextBox.Text = statusData.Status;

            ResultTextBox.Text = statusData.Data;
        }
    }
}
