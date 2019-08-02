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

            ChromeOptions opt = new ChromeOptions();
            // opt.AddArgument("--headless");
            driver = new ChromeDriver(opt);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(20000);

            sendo = new SendoPurchase();
            sendo.setDriver(driver);


            thisForm = this;

            //Lưu danh sách sản phẩm để xử lý
            products = new List<Product>();

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
            sendo.__threadingGetSendoClock();
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
            while (true)
            {
                //Thêm vào giỏ hàng
                if (sendo.hh == 0 && sendo.mm <= 10 && !sendo.isAddedToCart)
                {
                    sendo.products = products;
                    sendo.AddToCart();
                    RefreshListBox();
                }

                //Purchase
                if (sendo.hh == 00 && sendo.mm == 00 && sendo.ss == 00 )
                {
                    sendo.StartPurchase();
                    RefreshListBox();
                }
                Thread.Sleep(10);
            }
        }

        private Thread clockThread;
        private Thread updateThread;
        private Thread sendoThread;
        private void button2_Click(object sender, EventArgs e)
        {
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
