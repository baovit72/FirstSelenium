using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
 

using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;


namespace WindowsFormsApp10
{

    class SendoPurchase
    {
        public bool isLoggedIn { get; set; } = false;
        public bool isAddedToCart { get; set; } = false;
        public bool isPurchased { get; set; } = false;
        public List<Product> products { get; set; }
        List<string> tabHandler;

        //User
        public string username;
        public string password;

        private SendoPurchase __instance;
        private ChromeDriver driver;
        public int hh { get; set; }
        public int mm { get; set; }
        public int ss { get; set; }
        public SendoPurchase()
        {
            hh = mm = ss = -1;
            tabHandler = new List<string>();
            username = password = "";
        }

        public SendoPurchase GetInstance()
        {
            if (__instance == null)
                __instance = new SendoPurchase();
            return __instance;
        }

        public void setDriver(ChromeDriver driver)
        {
            this.driver = driver;
        }

        //Lấy giờ sendo

        public bool __threadingGetSendoClock()
        {
            ChromeOptions opt = new ChromeOptions();
           // opt.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            

            using (ChromeDriver threadDriver = new ChromeDriver(opt))
            {
                Debug.WriteLine(driver.CurrentWindowHandle);
                threadDriver.Navigate().GoToUrl("https://www.sendo.vn/");
                threadDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(20000); 
                var hour = threadDriver.FindElementByClassName("sd-hour");
                var minute = threadDriver.FindElementByClassName("sd-minute");
                var second = threadDriver.FindElementByClassName("sd-second");

                while (!(hh == 0
                            && mm == 0
                            && ss == 0
                            ))
                {
                    Thread.Sleep(5);
                    hh = int.Parse(hour.GetAttribute("innerText"));
                    mm = int.Parse(minute.GetAttribute("innerText"));
                    ss = int.Parse(second.GetAttribute("innerText"));
                    Debug.WriteLine(hh + ":" + mm + ":" + ss + "\n");
                };


                return true;
            }
        }

        //Kịch bản đặt hàng

        public void DoLogin()
        {
            if (isLoggedIn)
                return;
            try
            {
                driver.Navigate().GoToUrl("https://www.sendo.vn/flash-sale/");


                //Lấy vùng input



                var loginBtn = driver.FindElementByXPath("//button[@id='login']");
                Thread.Sleep(2000);
                loginBtn.Click();

                var sendoIDBtn = driver.FindElementByXPath("//div[text()='Đã có SendoID']");
                sendoIDBtn.Click();

                var userNameField = driver.FindElementByXPath("//input[@name='email']");
                var passwordField = driver.FindElementByXPath("//input[@name='password']");
                var submitBtn = driver.FindElementByXPath("//button[text()='Đăng nhập']");

                userNameField.SendKeys(username.Trim());
                passwordField.SendKeys(password.Trim());

                submitBtn.Click();

                isLoggedIn = true;

            }
            catch (ElementClickInterceptedException e)
            {
                 
            }


        }


        public void AddToCart()
        {
            if (!isLoggedIn || isAddedToCart)
                return;
            foreach(Product p in products)
            {
                try
                {
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    driver.Navigate().GoToUrl(p.Name);

                   

                    if (driver.FindElementsByClassName("ProductOptions_1VQA").Count > 1)
                    {
                         
                        Thread.Sleep(10000);
                    }

                    var addToCartBtn = driver.FindElementByXPath("//button[text()='Mua ngay']");
                    addToCartBtn.Click();

                    p.Status = "Added to cart";

                  
                    tabHandler.Add(driver.CurrentWindowHandle);
                   //Open new tab
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                    

                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                isAddedToCart = true;
            }
        }


        public void StartPurchase()
        {
            int pIdx = 0;
            foreach(String t in tabHandler)
            {
                try
                {
                    driver.SwitchTo().Window(t);
                    //Tien hanh purchase
                    var purchaseBtn = driver.FindElementByClassName("button_fKtq");
                    purchaseBtn.Click();

                    //Click lần 2 
                    var purchaseBtn2 = driver.FindElementsByClassName("button_fKtq");
                    if (purchaseBtn2.Count > 0)
                        purchaseBtn2[0].Click();

                    Debug.WriteLine(t);

                    products[pIdx++].Status = "Purchased";
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            isPurchased = true;
        }


        public void ResetState()
        {
            isLoggedIn = false;
            isAddedToCart = false;
            isPurchased = false;
        }
    }
  
}
