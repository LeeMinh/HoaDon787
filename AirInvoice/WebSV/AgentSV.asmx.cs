using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AirlineInvoice.Models;
namespace AirlineInvoice.WebSV
{
    /// <summary>
    /// Summary description for AgentSV
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AgentSV : System.Web.Services.WebService
    {

        [WebMethod]
        
        public List<Agents> GetAll(string UserName, string Password)
        {
            if (UserName == "mba" && Password == "123456@68##")
            {
                List<Agents> listAgent = new List<Agents>();
                Agents agent = new Agents();
                listAgent = agent.GetAll();
                return listAgent;
            }
            else
            {
                return new List<Agents>();
            }
        }
    }
}
