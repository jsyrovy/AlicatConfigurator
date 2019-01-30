using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AlicatConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Alicat alicat;
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            RefreshPorts();
            FillBaudRates();
            FillIds();
        }

        private void RefreshPorts()
        {
            comboBoxPorts.ItemsSource = Alicat.GetPorts();
            comboBoxPorts.SelectedIndex = 0;
        }

        private void FillIds()
        {
            comboBoxIds.ItemsSource = Alicat.GetIds();
            comboBoxIds.SelectedIndex = 0;
        }

        private void FillBaudRates()
        {
            comboBoxBaudRates.ItemsSource = Alicat.GetBaudRates();
            comboBoxBaudRates.SelectedValue = 19200;
        }

        private void EnableButtons()
        {
            comboBoxDevices.IsEnabled = !alicat.IsConnected;
            comboBoxPorts.IsEnabled = !alicat.IsConnected;
            comboBoxBaudRates.IsEnabled = !alicat.IsConnected;
            comboBoxIds.IsEnabled = !alicat.IsConnected;
            textBoxDelay.IsEnabled = !alicat.IsConnected;
            buttonRefresh.IsEnabled = !alicat.IsConnected;
            buttonConnect.IsEnabled = !alicat.IsConnected;
            buttonDisconnect.IsEnabled = alicat.IsConnected;
        }

        private void SetControls()
        {
            string[] valueNames = alicat.GetValueNames();
            foreach (string name in valueNames)
            {
                StackPanel sp = CreateStackPanel($"stackPanelCurrent{name}");
                Label lblName = CreateLabel($"labelCurrent{name}Name", name);
                Label lblValue = CreateLabel($"labelCurrent{name}Value", "0", true);
                sp.Children.Add(lblName);
                sp.Children.Add(lblValue);
                stackPanelCurrent.Children.Add(sp);
            }
            StackPanel spSetpt = CreateStackPanel("stackPanelSetSetpt");
            Label lblSetptName = CreateLabel("labelSetSetptName", "Setpt");
            TextBox tbSetptValue = CreateTextBox("textBoxSetSetptValue", alicat.CurrentSetpt.ToString());
            spSetpt.Children.Add(lblSetptName);
            spSetpt.Children.Add(tbSetptValue);
            stackPanelSet.Children.Add(spSetpt);
            if (alicat is AlicatMc)
            {
                StackPanel spGas = CreateStackPanel("stackPanelSetGas");
                Label lblSetGasName = CreateLabel("labelSetGasName", "Gas");
                ComboBox cbSetGasValue = CreateComboBox("comboBoxSetGasValue", ((AlicatMc)alicat).GetGases());
                spGas.Children.Add(lblSetGasName);
                spGas.Children.Add(cbSetGasValue);
                stackPanelSet.Children.Add(spGas);
                cbSetGasValue.SelectedValue = ((AlicatMc)alicat).CurrentGas;
            }
            Button btnSet = CreateButton("buttonSet", "Set");
            btnSet.Click += ButtonSet_Click;
            RegisterName(btnSet.Name, btnSet);
            stackPanelSet.Children.Add(btnSet);
        }

        private void RemoveControls()
        {
            string[] valueNames = alicat.GetValueNames();
            foreach (string name in valueNames)
            {
                StackPanel sp = (StackPanel)this.FindName($"stackPanelCurrent{name}");
                stackPanelCurrent.Children.Remove(sp);
                UnregisterName(sp.Name);
                UnregisterName($"labelCurrent{name}Name");
                UnregisterName($"labelCurrent{name}Value");
            }
            StackPanel spSetpt = (StackPanel)this.FindName($"stackPanelSetSetpt");
            Button btnSet = (Button)this.FindName($"buttonSet");
            stackPanelSet.Children.Remove(spSetpt);
            stackPanelSet.Children.Remove(btnSet);
            UnregisterName(spSetpt.Name);
            UnregisterName(btnSet.Name);
            UnregisterName("labelSetSetptName");
            UnregisterName("textBoxSetSetptValue");
            if (alicat is AlicatMc)
            {
                StackPanel spGas = (StackPanel)this.FindName($"stackPanelSetGas");
                stackPanelSet.Children.Remove(spGas);
                UnregisterName(spGas.Name);
                UnregisterName("labelSetGasName");
                UnregisterName("comboBoxSetGasValue");
            }
        }

        private void ShowValues()
        {
            Label lblPress = (Label)this.FindName("labelCurrentPressValue");
            Label lblSetpt = (Label)this.FindName("labelCurrentSetptValue");
            Label lblStatus = (Label)this.FindName("labelCurrentStatusValue");
            Label lblLastUpdate = (Label)this.FindName("labelCurrentLastUpdateValue");
            lblPress.Content = alicat.CurrentPress.ToString();
            lblSetpt.Content = alicat.CurrentSetpt.ToString();
            lblStatus.Content = alicat.Status;
            lblLastUpdate.Content = $"{alicat.LastUpdate:yyyy-MM-dd HH:mm:ss}";
            if (alicat is AlicatMc)
            {
                Label lblTemp = (Label)this.FindName("labelCurrentTempValue");
                Label lblVolFlow = (Label)this.FindName("labelCurrentVolFlowValue");
                Label lblMassFlow = (Label)this.FindName("labelCurrentMassFlowValue");
                Label lblGas = (Label)this.FindName("labelCurrentGasValue");
                lblTemp.Content = ((AlicatMc)alicat).CurrentTemp.ToString();
                lblVolFlow.Content = ((AlicatMc)alicat).CurrentVolFlow.ToString();
                lblMassFlow.Content = ((AlicatMc)alicat).CurrentMassFlow.ToString();
                lblGas.Content = ((AlicatMc)alicat).CurrentGas;
            }
        }

        private StackPanel CreateStackPanel(string name)
        {
            StackPanel sp = new StackPanel
            {
                Name = name,
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            RegisterName(sp.Name, sp);
            return sp;
        }

        private Label CreateLabel(string name, string content, bool autoWidth = false)
        {
            Label lbl = new Label
            {
                Name = name,
                Content = content,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (!autoWidth)
                lbl.Width = 75;
            RegisterName(lbl.Name, lbl);
            return lbl;
        }

        private TextBox CreateTextBox(string name, string text)
        {
            TextBox tb = new TextBox
            {
                Name = name,
                Text = text,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 75
            };
            RegisterName(tb.Name, tb);
            return tb;
        }

        private ComboBox CreateComboBox(string name, string[] itemsSource)
        {
            ComboBox cb = new ComboBox
            {
                Name = name,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 75,
                ItemsSource = itemsSource,
                SelectedIndex = 0
            };
            RegisterName(cb.Name, cb);
            return cb;
        }

        private Button CreateButton(string name, string content)
        {
            Button btn = new Button
            {
                Name = name,
                Content = content,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(10, 0, 10, 0)
            };
            RegisterName(btn.Name, btn);
            return btn;
        }

        private void StartTimer(int delay)
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, delay);
            dispatcherTimer.Start();
        }

        private void ReadValues()
        {
            try
            {
                alicat.ReadValues();
            }
            catch (IndexOutOfRangeException)
            {
                alicat.Status = "Unknown Data";
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPorts();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            string device = comboBoxDevices.Text;
            string portName = comboBoxPorts.Text;
            int portBaudRate = int.Parse(comboBoxBaudRates.Text);
            char id = char.Parse(comboBoxIds.Text);
            if (int.TryParse(textBoxDelay.Text, out int delay))
            {
                if (device == "Pressure Controller")
                    alicat = new AlicatPc();
                else
                    alicat = new AlicatMc();
                alicat.Config(device, portName, portBaudRate, id);
                alicat.Connect();
                if (alicat is AlicatMc && !File.Exists(((AlicatMc)alicat).gasesXmlPath))
                {
                    alicat.Disconnect();
                    MessageBox.Show($"Config file '{((AlicatMc)alicat).gasesXmlPath}' not found.", "Missing Config File", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!alicat.TryReadSerial())
                {
                    alicat.Disconnect();
                    MessageBox.Show("This device doesn't send any data.", "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (alicat.GetCurrentMsgLength() != alicat.GetReqMsgLength())
                {
                    alicat.Disconnect();
                    MessageBox.Show($"This device isn't the {device}.", "Invalid Device", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ReadValues();
                EnableButtons();
                SetControls();
                ShowValues();
                StartTimer(delay);
            }
            else
                MessageBox.Show("Delay has to be integer.", "Invalid Delay", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ButtonDisonnect_Click(object sender, RoutedEventArgs e)
        {
            alicat.DisconnectWithTimer(dispatcherTimer);
            EnableButtons();
            RemoveControls();
        }

        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(((TextBox)this.FindName("textBoxSetSetptValue")).Text, out float setpt))
            {
                alicat.SetSetpt(setpt);
                if (alicat is AlicatMc)
                    ((AlicatMc)alicat).SetGas(((ComboBox)this.FindName("comboBoxSetGasValue")).Text);
                ShowValues();
            }
            else
                MessageBox.Show($"Setpt has to be number.\n\nYour decimal separator is '{Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}'.", "Invalid Setpt", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ReadValues();
            ShowValues();
        }
    }
}
