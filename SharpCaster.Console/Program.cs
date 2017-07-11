using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SharpCaster.Controllers;
using SharpCaster.Extensions;
using SharpCaster.Models;
using SharpCaster.Models.ChromecastStatus;
using SharpCaster.Models.MediaStatus;
using SharpCaster.Services;

namespace SharpCaster.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var sender = new ChromecastSender();
            sender.Setup();
            sender.DoStuff();

            var input = System.Console.ReadLine();
        }

       
    }
}
