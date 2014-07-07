using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PingDomMonitor.Pingdom;

namespace PingDomMonitor
{
    class Program
    {
        const int PINGDOM_API_STATUS_OK = 0;
        const string apik = "120f4dcc809554bbc7b7d35fa1fbdd0f";        
        static void Main(string[] args)
        {

            // See http://www.pingdom.com/services/api-examples/
            Auth_CredentialsData credentialsData= new Auth_CredentialsData();
            credentialsData.username = "jj@gioorgi.com";
            credentialsData.password = "hollyw00d";
            
            Pingdom.PingdomAPISoapPortClient s = new PingdomAPISoapPortClient();
            Auth_LoginResponse r= s.Auth_login(apik, credentialsData);
            if (r.status != PINGDOM_API_STATUS_OK)
            {
                Console.WriteLine("ERR Login FAILED");
            }
            string sid=r.sessionId;
            //Check_GetListResponse lr= s.Check_getList(apik, sid);
            //checkStatus(lr);
            var lastDowns=s.Report_getLastDowns(apik,sid);
            checkStatus(lastDowns);

            
            foreach(var down in lastDowns.lastDowns){
                Console.WriteLine(down.checkName);
                Console.WriteLine(down.lastDown);                
            }

        }

        private static void checkStatus(dynamic lr)
        {
            if (lr.status != PINGDOM_API_STATUS_OK)
            {
                Console.WriteLine("Phase:" + lr.getType().getName() + " FAILED. STATUS=" + lr.status);
            }
        }
    }
}
