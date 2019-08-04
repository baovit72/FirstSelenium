using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Remote;
namespace WindowsFormsApp10
{


    public partial class Form1 : Form
    {
        private ChromeDriver driver;
        private SendoPurchase sendo;

        private Form thisForm;
        private List<Product> products;
        public Form1()
        {
            InitializeComponent();

            driver = CreateChromeDriver();

            sendo = new SendoPurchase();
            sendo.setDriver(driver);


            thisForm = this;

            //Lưu danh sách sản phẩm để xử lý
            products = new List<Product>();

            comboBox1.Items.Add("0855765343:hnq18082002");
            comboBox1.Items.Add("0855715527:hnq18082002");
            comboBox1.Items.Add("0941638715:hnq18082002");
            comboBox1.Items.Add("0352986463:hnq18082002");


        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            //Reset wifi
            //try
            //{
            if (MessageBox.Show("Reset ?!", "WIFI", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                NetworkUtility.Reset_Wifi(driver);
                MessageBox.Show("Successful!!!!!!!!!!!!!!!!!!!!!");
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Oops! Something's wrong...................");
            //}

        }

        private void clockThreading()
        {
            try
            {
                sendo.__threadingGetSendoClock();
            }
            catch (Exception e)
            {

            }
            MessageBox.Show("Done timer!!!");
        }

        private void updateTimerThreading()
        {

            MethodInvoker inv = delegate
            {
                this.label1.Text = "Timer: " + sendo.hh + ":" + sendo.mm + ":" + sendo.ss;

            };

            while (sendo.hh != 0 || sendo.mm != 0 || sendo.ss != 0)
            {
                this.Invoke(inv);
                Thread.Sleep(50);

            }



        }

        private void sendoThreading()
        {
            //Tiến hành login
            sendo.DoLogin();
            //Tiến hành sleep để tiết kiệm năng lượng 

            //=> Placeholder.................

            bool bugDetectorPermission = false;
            while (true)
            {
                //Thêm vào giỏ hàng
                if (sendo.hh == 0 && sendo.mm <= 10 && sendo.mm > -1 && !sendo.isAddedToCart)
                {
                    sendo.products = products;
                    sendo.AddToCart();
                    RefreshListBox();
                }
                //Bug gì đầy :V chuyển từ 1 sang 0 nhưng chuyển phút trước dẫn đến tình trạng nhận nhầm thành 0:0:0
                if (sendo.hh == 00 && sendo.mm == 00 && sendo.ss <=50  && sendo.ss>=30 && !bugDetectorPermission)
                {
                    bugDetectorPermission = true;
                }

                //Purchase
                    if (sendo.hh == 00 && sendo.mm == 00 && sendo.ss == 00 && bugDetectorPermission)
                {
                    sendo.StartPurchase();
                    RefreshListBox();
                }
                Thread.Sleep(5);
            }
        }
        private ChromeDriver CreateChromeDriver()
        {
            ChromeOptions opt = new ChromeOptions();
            opt.AddArgument(@"--incognito");
            ChromeDriver driver;
            driver = new ChromeDriver(opt);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            driver.Manage().Cookies.DeleteAllCookies();
            return driver;
        }
        private Thread clockThread;
        private Thread updateThread;
        private Thread sendoThread;
        private void button2_Click(object sender, EventArgs e)
        {
            sendo.ResetState();
            if (comboBox1.SelectedIndex == -1) return;
            if (driver != null)
            {
                driver.Close();
                driver.Quit();
            }
            driver = CreateChromeDriver();

            sendo.setDriver(driver);

            //Lấy tài khoản đăng nhập
            sendo.username = comboBox1.SelectedItem.ToString().Split(':')[0];
            sendo.password = comboBox1.SelectedItem.ToString().Split(':')[1];
            if (clockThread != null && updateThread != null)
            {
                clockThread.Abort();
                updateThread.Abort();
                sendoThread.Abort();
            }
            ThreadStart clockThreadStart = new ThreadStart(clockThreading);
            ThreadStart updateTimerThreadStart = new ThreadStart(updateTimerThreading);
            ThreadStart sendoThreadStart = new ThreadStart(sendoThreading);

            clockThread = new Thread(clockThreadStart);
            updateThread = new Thread(updateTimerThreadStart);
            sendoThread = new Thread(sendoThreading);

            //Chạy 2 thread

            clockThread.Start();
            updateThread.Start();
            sendoThread.Start();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clockThread != null && updateThread != null)
            {
                clockThread.Abort();
                updateThread.Abort();
                sendoThread.Abort();
            }
            driver.Close();
            driver.Quit();
        }
        private void RefreshListBox()
        {
            listBox1.DataSource = products.Select(product => product.Name).ToList();
            listBox2.DataSource = products.Select(product => product.Status).ToList();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Uri.IsWellFormedUriString(tbURL.Text, UriKind.Absolute))
                return;
            products.Add(new Product(tbURL.Text, "Initialized"));
            tbURL.Text = "";
            RefreshListBox();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                products.RemoveAt(listBox1.SelectedIndex);
                RefreshListBox();
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
    class Product
    {
        public string Name { get; set; }
        public String Status { get; set; }

        public Product(String name, string status)
        {
            Name = name;
            Status = status;
        }
    }
}
