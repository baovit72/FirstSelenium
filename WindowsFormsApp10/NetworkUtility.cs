using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using OpenQA.Selenium.Support.UI;



namespace WindowsFormsApp10
{
    class NetworkUtility
    {
        

        static public void Reset_Wifi(ChromeDriver driver)
        {
          
                 
                //Vô 192.168.1.1
                driver.Navigate().GoToUrl("http://192.168.1.1/");
                //Lấy thông tin input
                var userNameField = driver.FindElementById("username");
                var pswField = driver.FindElementById("pcPassword");
                var loginButton = driver.FindElementByClassName("loginBtn");

                //Gửi thông tin đăng nhập

                userNameField.SendKeys("admin");
                driver.ExecuteScript("document.getElementById(\"pcPassword\").setAttribute(\"maxlength\",20);");
                pswField.SendKeys("Honguyenbao2212!");
                loginButton.Click();

                //Tien hanh reset
                driver.Navigate().GoToUrl("http://192.168.1.1/resetrouter.html");

                var rebootBtn = driver.FindElementByXPath("//input[@value='Reboot']");
                
                rebootBtn.Click();
                driver.Close();
            
            
        }
    }
}
