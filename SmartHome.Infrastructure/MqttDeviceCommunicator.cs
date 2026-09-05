//using DeviceCommunicator;
//using MQTTnet;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace SmartHome.Infrastructure.mqtt
//{
//    public class MqttDeviceCommunicator : IDeviceCommunicator
//    {

//        private readonly IMqttClient _client;

//        private const string BrokerAddress = "localhost";
//        private const int BrokerPort = 1883;

//        private const string CommandTopic = "home/command";
//        private const string StatusTopic = "home/status";
//        private const string TelemetryTopic = "home/telemetry";
//        private const string LwtTopic = "home/lwt";
//        private const string feedback = "home/feedback";


//        // =========================
//        // آخرین وضعیت در حافظه
//        // =========================
//        public bool IsOnline { get; private set; } = false;
//        //public bool[] Relays { get; private set; } = new bool[4];
//        //public Dictionary<string, double> Sensors { get; private set; } = new();
//        //public DateTime? LastStatusTime { get; private set; }
//        //public DateTime? LastTelemetryTime { get; private set; }

//        public MqttDeviceCommunicator(MqttClientFactory factory)
//        {
//            _client = factory.CreateMqttClient();
//            _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
//        }

//        public async Task ConnectAsync()
//        {
//            if (_client.IsConnected)
//                return;

//            var options = new MqttClientOptionsBuilder()
//                .WithTcpServer(BrokerAddress, BrokerPort)
//                .WithClientId("SmartHome-Backend")
//                .WithCleanSession()
//                .Build();

//            await _client.ConnectAsync(options);
//            Console.WriteLine("Backend connected to MQTT!");

//            // Subscribe به همه topicهای مهم
//            await _client.SubscribeAsync(StatusTopic);
//            await _client.SubscribeAsync(TelemetryTopic);
//            await _client.SubscribeAsync(LwtTopic);

//            Console.WriteLine($"Subscribed to: {StatusTopic}, {TelemetryTopic}, {LwtTopic}");
//        }



//        public async Task PublishCommandAsync(string message)
//        {
//            if (!_client.IsConnected)
//            {
//                Console.WriteLine("MQTT is not connected. Cannot publish.");
//                return;
//            }

//            var mqttMessage = new MqttApplicationMessageBuilder()
//                .WithTopic(CommandTopic)
//                .WithPayload(message)
//                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
//                .Build();

//            await _client.PublishAsync(mqttMessage);

//            Console.WriteLine();
//            Console.WriteLine("Command sent:");
//            Console.WriteLine(message);
//        }


//        private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
//        {
//            var topic = e.ApplicationMessage.Topic;
//            var message = e.ApplicationMessage.ConvertPayloadToString();

//            Console.WriteLine();
//            Console.WriteLine("========== MQTT MESSAGE ==========");
//            Console.WriteLine($"Topic  : {topic}");
//            Console.WriteLine($"Message: {message}");
//            Console.WriteLine("==================================");

//            try
//            {
//                using var doc = JsonDocument.Parse(message);
//                var root = doc.RootElement;










//                if (topic == LwtTopic)
//                {
//                    //انلاین بودن esp گرفته میشه
//                }




//                if (topic == feedback)
//                {
//                    //انجام شدن هر دستور تایید میشه

//                }








//                else if (topic == StatusTopic)
//                {

//                    //گرفتن مقادیر سنسور های دیچیتال
//                }









//                else if (topic == TelemetryTopic)
//                {
//                    //گرفتن مقادیر سنسور های انالوگ

//                }



//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error parsing message: {ex.Message}");
//            }

//            return Task.CompletedTask;
//        }






//        //**********************************************************************



//        public Task Esp_get_sensor_status(string ExternalId)
//        {
//            //درخواست ارسال وضعیت سنسور

//            return PublishCommandAsync($"Esp_get_sensor_status:{ExternalId}");


//        }

//        public Task Esp_get_sensor_value(string ExternalId)
//        {
//            //درخواست ارسال مقدار سنسور
//            //ExternalId can be I2C address
//            //ExternalId can be SPI ss
//            return PublishCommandAsync($"Esp_get_sensor_value:{ExternalId}");

//        }

//        public Task Esp_Turn_Off(string ExternalId)
//        {
//            //دستور روشن کردن و انتظار فیدبک
//            return PublishCommandAsync($"Esp_Turn_off:{ExternalId}");
//        }

//        public Task Esp_Turn_On(string ExternalId)
//        {
//            //دستور خاموش کردن و انتظار فیدبک

//            return PublishCommandAsync($"Esp_Turn_On:{ExternalId}");

//        }


//    }
//}
