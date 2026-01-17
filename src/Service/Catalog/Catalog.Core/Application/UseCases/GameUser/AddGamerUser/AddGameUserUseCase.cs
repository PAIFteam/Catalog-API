using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Catalog.Core.Domain.Entities.Base;
using Catalog.Core.Domain.Entities.RabbitMQ;
using Catalog.Core.Domain.Interfaces;

namespace Catalog.Core.Application.UseCases.GameUser.AddGameUser
{
    public class AddGameUserUseCase
    {
        private readonly IAddGameUserRepository _addGameUserRepository;
        private readonly ILogger<AddGameUserUseCase> _logger;
        private readonly IPublisher _publisher;
        private readonly RabbitMqConfigurationSettings _rabbitMqConfigurationSettings;
        public AddGameUserUseCase(
            IAddGameUserRepository addUserRepository,
            ILogger<AddGameUserUseCase> logger
        )
        {
            _addGameUserRepository = addUserRepository;
            _logger = logger;
        }

        public async Task<AddGameUserOutput> ExecuteAsync(AddGameUserInput input)
        {

            _logger.LogInformation("Starting AddGamerUserCase.ExecuteAsync");

            try
            {
                
                OutPutBase outPutBase = ValidateInput(input);

                if (!outPutBase.Result)
                {
                    return new AddGameUserOutput
                    {
                        Result = false,
                        Message = outPutBase.Message,
                        Exception = outPutBase.Exception
                    };
                }

                //Envia mensagem para fila RabbitMQ no Evento OrderPlacedEvent
                var message = new OrderPlacedMessage(input.IdUser, input.IdGame, input.Price);

                await _publisher.Publish(message, _rabbitMqConfigurationSettings.GetQueueAdress());

                //int idUser = await _AddGameUserRepository.AddGameUserAsync(input.MapToUser());

                AddGameUserOutput outPut = new AddGameUserOutput
                {
                    Result = false,
                    Message = "Game to User event created successfully",
                    Exception = null

                };

                return outPut;
            }
            catch (Exception ex)
            {
                return new AddGameUserOutput
                {
                    Result = false,
                    Message = "Ocorreu umm erro de Runtime Interno",
                    Exception = ex
                };
                
            }

        }

        private OutPutBase ValidateInput(AddGameUserInput input)
        {
            //Validações de entrada

            OutPutBase outPut = new OutPutBase();
            outPut.Result = true;
            // Implement validation logic here
            return outPut;
        }
    }
}
