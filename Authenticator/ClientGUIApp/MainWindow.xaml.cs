using System;
using System.Collections.Generic;
using System.Windows;
using Authenticator;
using RestSharp;
using System.ServiceModel;
using Newtonsoft.Json;
using Registry.Models;
using ServiceProvider.Models;
using System.Threading.Tasks;

namespace ClientGUIApp
{
    public partial class MainWindow : Window {
        public IAuthenticatorServer foob;
        public RestClient registryClient;
        public RestClient serviceProviderClient;
        public int token = -1;

        public MainWindow() {
            InitializeComponent();

            ChannelFactory<IAuthenticatorServer> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<IAuthenticatorServer>(tcp, URL);
            foob = foobFactory.CreateChannel();
            registryClient = new RestClient("http://localhost:63273");
            serviceProviderClient = new RestClient("http://localhost:56066/");

            disableCalculator();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e) {
            Task<int> task = new Task<int>(Login);
            task.Start();

            whenLoading(); //disable views and enable progress bar

            try {
                token = await task;

                afterLoading(); //enable views and disable progress bar

                if (token == -1) {
                    MessageBox.Show("Incorrect credentials.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else {
                    MessageBox.Show("Successfully logged in.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                }
            }
            catch (FaultException ex) {
                MessageBox.Show("There was an error on the server's end.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                afterLoading(); //enable views and disable progress bar
            }
            disableCalculator();
        }

        private int Login() {
            string name = null;
            string password = null;
            int returnToken = -1;

            NameTextBox.Dispatcher.Invoke(new Action(() => name = NameTextBox.Text));
            PasswordTextBox.Dispatcher.Invoke(new Action(() => password = PasswordTextBox.Text));

            try {
                returnToken = foob.Login(name, password);
            }
            catch (FaultException ex) {
                throw ex;
            }

            return returnToken;
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e) {

            if (NameTextBox.Text.Equals("") || PasswordTextBox.Text.Equals("")) {
                MessageBox.Show("Name and/or password cannot be empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                Task<string> task = new Task<string>(Register);
                task.Start();

                whenLoading(); //disable views and enable progress bar

                try {
                    string response = await task;

                    afterLoading(); //enable views and disable progress bar

                    if (response.Equals("unsuccessfully registered")) {
                        MessageBox.Show("Name and password already exists.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                }
                catch (FaultException ex) {
                    MessageBox.Show("There was an error on the server's end.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    afterLoading(); //enable views and disable progress bar
                }
            }
            disableCalculator();
        }

        private string Register() {
            string name = null;
            string password = null;
            string returnMsg = "";

            NameTextBox.Dispatcher.Invoke(new Action(() => name = NameTextBox.Text));
            PasswordTextBox.Dispatcher.Invoke(new Action(() => password = PasswordTextBox.Text));

            try {
                returnMsg = foob.Register(name, password);
            }
            catch (FaultException ex) {
                throw ex;
            }

            return returnMsg;
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e) {
            if (SearchTextBox.Text.Equals("")) {
                MessageBox.Show("Search description cannot be empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else {
                disableCalculator();

                // build the task
                Task<Registry.Models.StatusData> task = new Task<Registry.Models.StatusData>(SearchServices);
                task.Start();

                whenLoading(); //disable views and enable progress bar

                try {
                    // wait for the task to be done
                    Registry.Models.StatusData statusData = await task;

                    afterLoading(); //enable views and disable progress bar

                    // process the results
                    if (statusData.Status.Equals("Successful")) {
                        List<ServiceDescription> serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(statusData.Data);
                        ServiceList.ItemsSource = serviceDescriptions;
                    }
                    else {
                        MessageBox.Show("You need to login first.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                }
                catch (FaultException ex) {
                    MessageBox.Show("There was an error on the server's end.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    afterLoading(); //enable views and disable progress bar
                }
            }
            disableCalculator();
        }

        private Registry.Models.StatusData SearchServices() {
            // generate the SearchData
            SearchData searchData = new SearchData();
            SearchTextBox.Dispatcher.Invoke(new Action(() => searchData.Search = SearchTextBox.Text));
            searchData.Token = token;

            Registry.Models.StatusData statusData;

            try {
                // make the RestRequest
                RestRequest request = new RestRequest("api/service/search", Method.Post);
                request.AddJsonBody(searchData);
                RestResponse response = registryClient.Post(request);

                // get the results
                statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);
            }
            catch (FaultException ex) {
                throw ex;
            }

            return statusData;
        }

        private async void AllServicesBtn_Click(object sender, RoutedEventArgs e) {
            // build the task
            Task<Registry.Models.StatusData> task = new Task<Registry.Models.StatusData>(SearchAllServices);

            // and start it
            task.Start();

            whenLoading(); //disable views and enable progress bar

            try {
                Registry.Models.StatusData statusData = await task;

                afterLoading(); //enable views and disable progress bar

                if (statusData.Status.Equals("Successful")) {
                    List<ServiceDescription> serviceDescriptions = JsonConvert.DeserializeObject<List<ServiceDescription>>(statusData.Data);
                    ServiceList.ItemsSource = serviceDescriptions;
                }
                else {
                    MessageBox.Show("You need to login first.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
            catch (FaultException ex) {
                MessageBox.Show("There was an error on the server's end.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                afterLoading(); //enable views and disable progress bar
            }
            disableCalculator();
        }

        private Registry.Models.StatusData SearchAllServices() {
            // generate the AllServicesData
            AllServicesData allServicesData = new AllServicesData();
            allServicesData.Token = token;

            Registry.Models.StatusData statusData;

            try {
                // make request
                RestRequest request = new RestRequest("api/service/allservices", Method.Post);
                request.AddJsonBody(allServicesData);
                RestResponse response = registryClient.Post(request);

                // deserialize
                statusData = JsonConvert.DeserializeObject<Registry.Models.StatusData>(response.Content);
            }
            catch (FaultException ex) {
                throw ex;
            }

            return statusData;
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
                else {
                    OperandOneTextBox.IsEnabled = false;
                    OperandTwoTextBox.IsEnabled = false;
                    OperandThreeTextBox.IsEnabled = false;
                    ResultTextBox.IsEnabled = false;
                    CalculateBtn.IsEnabled = false;
                }
            }
        }

        private async void CalculateBtn_Click(object sender, RoutedEventArgs e) {
            // build the task
            Task<ServiceProvider.Models.StatusData> task = new Task<ServiceProvider.Models.StatusData>(Calculate);
            task.Start();

            whenLoading();

            try {
                ServiceProvider.Models.StatusData statusData = await task;

                afterLoading();

                if (statusData.Status.Equals("Denied")) {
                    MessageBox.Show("There was an error in calculating the result.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else {
                    ResultTextBox.Text = statusData.Data;
                }
            }
            catch (FaultException ex) {
                MessageBox.Show("There was an error on the server's end.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                afterLoading(); //enable views and disable progress bar
                disableCalculator();
            }
            catch (FormatException ex) {
                MessageBox.Show("The operands must be a number.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                afterLoading(); //enable views and disable progress bar
                disableCalculator();
            }
        }

        private ServiceProvider.Models.StatusData Calculate() {
            ServiceDescription currItem = null;
            ServiceList.Dispatcher.Invoke(new Action(() => currItem = (ServiceDescription) ServiceList.SelectedItem));

            int[] operands = new int[3];
            RestRequest request;
            ServiceProvider.Models.StatusData statusData;

            try {
                // add two items
                if (currItem.APIEndpoint.Equals("http://localhost:56066/AddTwoNumbers")) {
                    request = new RestRequest("api/calculator/addtwonumbers", Method.Post);
                    OperandOneTextBox.Dispatcher.Invoke(new Action(() => operands[0] = Int32.Parse(OperandOneTextBox.Text)));
                    OperandTwoTextBox.Dispatcher.Invoke(new Action(() => operands[1] = Int32.Parse(OperandTwoTextBox.Text)));
                }
                // add three items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/AddThreeNumbers")) {
                    request = new RestRequest("api/calculator/addthreenumbers", Method.Post);
                    OperandOneTextBox.Dispatcher.Invoke(new Action(() => operands[0] = Int32.Parse(OperandOneTextBox.Text)));
                    OperandTwoTextBox.Dispatcher.Invoke(new Action(() => operands[1] = Int32.Parse(OperandTwoTextBox.Text)));
                    OperandThreeTextBox.Dispatcher.Invoke(new Action(() => operands[2] = Int32.Parse(OperandThreeTextBox.Text)));
                }
                // multiply two items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulTwoNumbers")) {
                    request = new RestRequest("api/calculator/multwonumbers", Method.Post);
                    OperandOneTextBox.Dispatcher.Invoke(new Action(() => operands[0] = Int32.Parse(OperandOneTextBox.Text)));
                    OperandTwoTextBox.Dispatcher.Invoke(new Action(() => operands[1] = Int32.Parse(OperandTwoTextBox.Text)));
                }
                // multiple three items
                else if (currItem.APIEndpoint.Equals("http://localhost:56066/MulThreeNumbers")) {
                    request = new RestRequest("api/calculator/multhreenumbers", Method.Post);
                    OperandOneTextBox.Dispatcher.Invoke(new Action(() => operands[0] = Int32.Parse(OperandOneTextBox.Text)));
                    OperandTwoTextBox.Dispatcher.Invoke(new Action(() => operands[1] = Int32.Parse(OperandTwoTextBox.Text)));
                    OperandThreeTextBox.Dispatcher.Invoke(new Action(() => operands[2] = Int32.Parse(OperandThreeTextBox.Text)));
                }
                else {
                    request = null;
                }

                CalculatorData calculatorData = new CalculatorData();
                calculatorData.Token = token;
                calculatorData.Operands = operands;

                request.AddJsonBody(calculatorData);
                RestResponse response = serviceProviderClient.Post(request);
                statusData = JsonConvert.DeserializeObject<ServiceProvider.Models.StatusData>(response.Content);
            }
            catch (Exception ex) when (ex is FormatException || ex is FaultException) {
                throw ex;            
            }
            return statusData;
        }

        private void whenLoading() {
            // Progress Bar
            ProgressBar.Visibility = Visibility.Visible;

            // Login/Register
            NameTextBox.IsEnabled = false;
            PasswordTextBox.IsEnabled = false;
            LoginBtn.IsEnabled = false;
            RegisterBtn.IsEnabled = false;

            // Search
            SearchTextBox.IsEnabled = false;
            SearchBtn.IsEnabled = false;
            AllServicesBtn.IsEnabled = false;

            // Calculator
            OperandOneTextBox.IsEnabled = false;
            OperandTwoTextBox.IsEnabled = false;
            OperandThreeTextBox.IsEnabled = false;
            CalculateBtn.IsEnabled = false;
            ResultTextBox.IsEnabled = false;
        }

        private void afterLoading() {
            // Progress Bar
            ProgressBar.Visibility = Visibility.Hidden;

            // Login/Register
            NameTextBox.IsEnabled = true;
            PasswordTextBox.IsEnabled = true;
            LoginBtn.IsEnabled = true;
            RegisterBtn.IsEnabled = true;

            // Search
            SearchTextBox.IsEnabled = true;
            SearchBtn.IsEnabled = true;
            AllServicesBtn.IsEnabled = true;

            // Calculator
            OperandOneTextBox.IsEnabled = true;
            OperandTwoTextBox.IsEnabled = true;
            OperandThreeTextBox.IsEnabled = true;
            CalculateBtn.IsEnabled = true;
            ResultTextBox.IsEnabled = true;
        }

        private void disableCalculator() {
            OperandOneTextBox.Text = "";
            OperandTwoTextBox.Text = "";
            OperandThreeTextBox.Text = "";
            ResultTextBox.Text = "";

            OperandOneTextBox.IsEnabled = false;
            OperandTwoTextBox.IsEnabled = false;
            OperandThreeTextBox.IsEnabled = false;
            ResultTextBox.IsEnabled = false;
            CalculateBtn.IsEnabled = false;
        }
    }
}
