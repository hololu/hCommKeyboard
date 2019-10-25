using System;
using System.IO.Ports;
using System.Windows.Forms;
using Microsoft.Win32;

namespace hCommKeyboard
{
    public partial class Form1 : Form
    {
        SerialPort sp = new SerialPort(); // Seri port nesnesi oluşturuyoruz
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkeyR = "Software\\hOLOlu";
        const string subkey = "Software\\hOLOlu\\hCommKeyboard";
        const string keyName = userRoot + "\\" + subkey;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nfi.Icon = this.Icon;

            cmbBaud.Items.Clear();
            cmbBaud.Items.Add("110");
            cmbBaud.Items.Add("300");
            cmbBaud.Items.Add("600");
            cmbBaud.Items.Add("1200");
            cmbBaud.Items.Add("2400");
            cmbBaud.Items.Add("4800");
            cmbBaud.Items.Add("9600");
            cmbBaud.Items.Add("14400");
            cmbBaud.Items.Add("19200");
            cmbBaud.Items.Add("28800");
            cmbBaud.Items.Add("38400");
            cmbBaud.Items.Add("56000");
            cmbBaud.Items.Add("57600");
            cmbBaud.Items.Add("115200");
            cmbBaud.Items.Add("128000");
            cmbBaud.Items.Add("153600");
            cmbBaud.Items.Add("230400");
            cmbBaud.Items.Add("256000");
            cmbBaud.Items.Add("460800");
            cmbBaud.Items.Add("921600");
            //cmbBaud.Text = "9600";

            cmdDBit.Items.Clear();
            cmdDBit.Items.Add("4");
            cmdDBit.Items.Add("5");
            cmdDBit.Items.Add("6");
            cmdDBit.Items.Add("7");
            cmdDBit.Items.Add("8");
            //cmdDBit.Text = "8";
            
            cmbParite.Items.Clear();
            cmbParite.Items.Add("None");
            cmbParite.Items.Add("Odd");
            cmbParite.Items.Add("Even");
            cmbParite.Items.Add("Mark");
            cmbParite.Items.Add("Space");
            //cmbParite.Text = "None";

            cmbStop.Items.Clear();
            cmbStop.Items.Add("1");
            cmbStop.Items.Add("1.5");
            cmbStop.Items.Add("2");
            //cmbStop.Text = "1";

            AyarOku();

            sp.BaudRate = Convert.ToInt32(cmbBaud.Text);  // Seri haberleşme hızını seçiyoruz (int32)
            sp.DataBits = Convert.ToInt32(cmdDBit.Text); // göndereceğimiz bilginin kaç bitten oluşacağını bildiriyoruz (int32).

            switch (cmbParite.SelectedIndex)
            {
                case 0:
                    sp.Parity = Parity.None; // Eşlik bitidir. Gönderilen verinin doğruluğunu kontrol etmek için kullanılır. 
                    break;

                case 1:
                    sp.Parity = Parity.Odd;
                    break;

                case 2:
                    sp.Parity = Parity.Even;
                    break;

                case 3:
                    sp.Parity = Parity.Mark;
                    break;

                case 4:
                    sp.Parity = Parity.Space;
                    break;
            }

            switch (cmbStop.SelectedIndex)
            {
                case 0:
                    sp.StopBits = StopBits.One; // Stop bitinin kaç bit olacağını belirtir.
                    break;

                case 1:
                    sp.StopBits = StopBits.OnePointFive;
                    break;

                case 2:
                    sp.StopBits = StopBits.Two;
                    break;
            }

            //SerialPort sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            Yenile();

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sp.Close();
            AyarKaydet();
        }

        private void btnBaglan_Click(object sender, EventArgs e)
        {
            if (btnBaglan.Text == "&Kop")
            {
                sp.Close();
                groupBox1.Enabled = true;
                btnBaglan.Text = "&Bağlan";
                return;
            }

            if (comboBox1.Text != "")
            {
                AyarKaydet();

                if (sp.IsOpen)
                    sp.Close();

                sp.PortName = comboBox1.Text;
                try
                {
                    sp.Open(); // Seri portumuzu açıyoruz
                }
                catch (Exception ex)
                {
                }
            } else
            {
                MessageBox.Show("Lütfen Com Portu Seçiniz..!");
            }

            if (sp.IsOpen) {
                //btnBaglan.Enabled = false;
                btnBaglan.Text = "&Kop";
                groupBox1.Enabled = false;
            } else {
                //btnBaglan.Enabled = true;
                btnBaglan.Text = "&Bağlan";
                groupBox1.Enabled = true;
            }
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] veri = new byte[sp.ReadBufferSize];
            sp.Read(veri, 0, sp.ReadBufferSize);

            //int veri = sp.ReadChar(); // Seri porttan integer türünden 1 karakter veri okur
            SendKeys.SendWait(cevir(veri));

            if (chkEnter.Checked) SendKeys.SendWait("{ENTER}");
        }

        private string cevir(byte[] gelen)
        {
            return System.Text.Encoding.UTF8.GetString(gelen).TrimEnd('\0');

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                nfi.Visible = true;
                nfi.BalloonTipText = "Sistem Çalışıyor..!";
                nfi.ShowBalloonTip(1000);
            }
        }

        private void nfi_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            nfi.Visible = false;
        }

        private void btnYenile_Click(object sender, EventArgs e)
        {
            Yenile();
        }

        private void Yenile()
        {
                    comboBox1.Items.Clear();

            string[] portlar = SerialPort.GetPortNames(); // Bağlı seri portları diziye aktardık
            foreach (string portAdi in portlar)
            {
                comboBox1.Items.Add(portAdi);
            }
        }

        private void chkEnter_CheckedChanged(object sender, EventArgs e)
        {
            AyarKaydet();
        }

        private void AyarOku()
        {
            //Registry.CurrentUser.CreateSubKey(@subkeyR);
            Registry.CurrentUser.CreateSubKey(@subkey);

            chkEnter.Checked = Convert.ToBoolean(Registry.GetValue(keyName, "ENTER", false));
            cmdDBit.Text = (string)Registry.GetValue(keyName, "DBit", "8"); 
            cmbBaud.Text = (string)Registry.GetValue(keyName, "Baud", "9600"); 
            cmbParite.Text = (string)Registry.GetValue(keyName, "Parite", "None"); 
            cmbStop.Text = (string)Registry.GetValue(keyName, "Stop", "1"); 
        }

        private void AyarKaydet()
        {
            //Registry.CurrentUser.CreateSubKey(@subkeyR);
            Registry.CurrentUser.CreateSubKey(@subkey);
            if (comboBox1.Text != "") { 
            //Registry.SetValue(keyName, "ENTER", chkEnter.Checked, RegistryValueKind.DWord);
            Registry.SetValue(keyName, "ENTER", chkEnter.Checked);
            Registry.SetValue(keyName, "DBit", cmdDBit.Text);
            Registry.SetValue(keyName, "Baud", cmbBaud.Text);
            Registry.SetValue(keyName, "Parite", cmbParite.Text);
            Registry.SetValue(keyName, "Stop", cmbStop.Text);
            }
        }
    }
}
