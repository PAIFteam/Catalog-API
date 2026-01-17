using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
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

                await _publisher.Publish(message, _rabbitMqConfigurationSettings.GetQueueAdress());

                //int idUser = await _PutGameUserRepository.PutGameUserAsync(input.MapToUser());

                PutGameUserOutput outPut = new PutGameUserOutput
                {
                    Result = false,
                    Message = "Game to User event created successfully",
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
