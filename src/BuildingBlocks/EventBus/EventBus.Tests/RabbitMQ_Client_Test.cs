using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using System;
using System.Text;

namespace eShopWithoutContainers.BuildingBlocks.EventBus.Tests
{
    [TestClass]
    public class RabbitMQ_Client_Test
    {
        [TestMethod]
        public void RabbitMQ_Client_Connection_Test()
        {
            var factory = new ConnectionFactory();

            factory.HostName = "localhost";
            //factory.UserName = "guest";
            //factory.Password = "guest";
            //factory.VirtualHost = "/";
            //factory.Port = 5672;

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("UnitTestQueue", false, false, false, null);

                    var properties = channel.CreateBasicProperties();

                    properties.DeliveryMode = 1;

                    string message = "I am message from unit test";

                    channel.BasicPublish("", "UnitTestQueue", properties, Encoding.UTF8.GetBytes(message));

                    Console.WriteLine($"Send:{message}");
                }
            }
        }
    }
}
