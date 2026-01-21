using Microsoft.Extensions.Logging;
using Catalog.Core.Domain.Entities.Base;
using Catalog.Core.Domain.Entities.RabbitMQ;
using Catalog.Core.Domain.Interfaces;

namespace Catalog.Core.Application.UseCases.GameUser.PutGameUser
{
    public class PutGameUserUseCase
    {
        //private readonly ILogger<PutGameUserUseCase> _logger;
        private readonly IPublisher _publisher;
        private readonly RabbitMqConfigurationSettings _rabbitMqConfigurationSettings;
        private readonly ILogger<PutGameUserUseCase> _logger;

        public PutGameUserUseCase(
            //IPutGameUserRepository putUserRepository,
            RabbitMqConfigurationSettings rabbitMqConfigurationSettings,
            IPublisher publisher,
            ILogger<PutGameUserUseCase> logger
        )
        {
            //_putGameUserRepository = putUserRepository;
            _logger = logger;
            _rabbitMqConfigurationSettings = rabbitMqConfigurationSettings;
            _publisher = publisher;
        }

        public async Task<PutGameUserOutput> ExecuteAsync(PutGameUserInput input)
        {
            _logger.LogInformation("Starting PutGamerUserCase.ExecuteAsync");
            _logger.LogInformation($"Recebendo requisição para Efetiva compra de Jogo Usuario: {input.IdUser}");

            try
            {                
                OutPutBase outPutBase = ValidateInput(input);

                if (!outPutBase.Result)
                {
                    return new PutGameUserOutput
                    {
                        Result = false,
                        Message = outPutBase.Message,
                        Exception = outPutBase.Exception
                    };
                }

                //Envia mensagem para fila RabbitMQ no Evento OrderPlacedEvent
                var message = new OrderPlacedMessage(input.IdUser, input.IdGame, input.Price);

                _logger.LogInformation("Invoca o Publish para a fila OrderPlacedMessage");
                await _publisher.Publish(message, _rabbitMqConfigurationSettings.GetQueueAdress());

                _logger.LogInformation($"Publish para a fila OrderPlacedMessage ENVIADA p Usuario: {input.IdUser}");

                PutGameUserOutput outPut = new PutGameUserOutput
                {
                    Result = false,
                    Message = "Comando de compra executado, aguardando pagamento!",
                    Exception = null

                };

                return outPut;
            }
            catch (Exception ex)
            {
                return new PutGameUserOutput
                {
                    Result = false,
                    Message = "Ocorreu umm erro de Runtime Interno",
                    Exception = ex
                };
                
            }

        }

        private OutPutBase ValidateInput(PutGameUserInput input)
        {
            //Validações de entrada

            OutPutBase outPut = new OutPutBase();
            outPut.Result = true;
            // Implement validation logic here
            return outPut;
        }
    }
}
