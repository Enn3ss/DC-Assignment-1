using System;
using System.Collections.Generic;
using System.Windows;
using Authenticator;
using RestSharp;
using System.ServiceModel;
using Newtonsoft.Json;
using Registry.Models;
using ServiceProvider.Models;

namespace ClientGUIApp
{
    public partial class MainWindow : Window {
        public IAuthenticatorServer foob;
        public RestClient registryClient;
        public RestClient serviceProviderClient;
        private int token = -1;

        public MainWindow() {
            InitializeComponent();

            ChannelFactory<IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
            registryClient = new RestClient("http://localhost:63273");
            serviceProviderClient = new RestClient("http://localhost:56066/");
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
                RestResponse response = registryClient.Post(request);
                Registry.Models.StatusData statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);

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
                string response = foob.Register(name, password);

                if (response.Equals("unsuccessfully registered")) {
                    MessageBox.Show("Name and password already exists.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }

        private void AllServicesBtn_Click(object sender, RoutedEventArgs e) {
            AllServicesData allServicesData = new AllServicesData();
            allServicesData.Token = token;

            RestRequest request = new RestRequest("api/service/allservices", Method.Post);
            request.AddJsonBody(allServicesData);
            RestResponse response = registryClient.Post(request);
            Registry.Models.StatusData statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);

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

                OperandOneTextBox.Text = "";
                OperandTwoTextBox.Text = "";
                OperandThreeTextBox.Text = "";
                ResultTextBox.Text = "";

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
            int[] operands = new int[3];
            RestRequest request;

            // add two items
            if (currItem.APIEndpoint.Equals("http://localhost:56066/AddTwoNumbers")) {
                request = new RestRequest("api/calculator/addtwonumbers", Method.Post);
                operands[0] = Int32.Parse(OperandOneTextBox.Text);
                operands[1] = Int32.Parse(OperandTwoTextBox.Text);
            }
            // add three items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/AddThreeNumbers")) {
                request = new RestRequest("api/calculator/addthreenumbers", Method.Post);
                operands[0] = Int32.Parse(OperandOneTextBox.Text);
                operands[1] = Int32.Parse(OperandTwoTextBox.Text);
                operands[2] = Int32.Parse(OperandThreeTextBox.Text);
            }
            // multiply two items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulTwoNumbers")) {
                request = new RestRequest("api/calculator/multwonumbers", Method.Post);
                operands[0] = Int32.Parse(OperandOneTextBox.Text);
                operands[1] = Int32.Parse(OperandTwoTextBox.Text);
            }
            // multiple three items
            else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulThreeNumbers")) {
                request = new RestRequest("api/calculator/multhreenumbers", Method.Post);
                operands[0] = Int32.Parse(OperandOneTextBox.Text);
                operands[1] = Int32.Parse(OperandTwoTextBox.Text);
                operands[2] = Int32.Parse(OperandThreeTextBox.Text);
            }
            else {
                request = null;
            }

            CalculatorData calculatorData = new CalculatorData();
            calculatorData.Token = token;
            calculatorData.Operands = operands;

            request.AddJsonBody(calculatorData);
            RestResponse response = serviceProviderClient.Post(request);
            ServiceProvider.Models.StatusData statusData = JsonConvert.DeserializeObject<ServiceProvider.Models.StatusData>(response.Content);

            SearchTextBox.Text = statusData.Status;
            ResultTextBox.Text = statusData.Data;
        }
    }
}
