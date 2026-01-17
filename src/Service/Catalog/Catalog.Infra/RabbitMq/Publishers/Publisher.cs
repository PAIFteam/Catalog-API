using Catalog.Core.Domain.Interfaces;
using MassTransit;


namespace Catalog.Infra.RabbitMq.Publishers
{
    public class Publisher : IPublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public Publisher(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task Publish<T>(T content, Uri queueAddress)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(queueAddress);
            
            await sendEndpoint.Send(content);
        }
    }
}
