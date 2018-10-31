//using System;
//using EasyNetQ;
//using Microsoft.Extensions.Logging;

//namespace Robin.Lottery.WebApp.MQ
//{
//    public interface IBusBuilder
//    {
//        IBus CreateMessageBus();
//    }

//    public class BusBuilder : IBusBuilder
//    {
//        private readonly ILogger<BusBuilder> _logger;
//        private IBus _bus;

//        public BusBuilder(ILogger<BusBuilder> logger)
//        {
//            _logger = logger;
//        }

//        public IBus CreateMessageBus()
//        {
//            if (_bus != null) return _bus;

//            try
//            {
//                _bus = RabbitHutch.CreateBus("host=localhost");
//            }
//            catch (Exception e)
//            {
//                _logger.LogWarning("连接 RabbitMQ 并创建 IBus 实例失败. 异常详情: " + Environment.NewLine + e);
//            }

//            return _bus;
//        }
//    }
//}