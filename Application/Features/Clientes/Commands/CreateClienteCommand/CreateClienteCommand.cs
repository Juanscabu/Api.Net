using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using Application.Wrappers;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Application.Interfaces;
using Domain.Entities;
using AutoMapper;

namespace Application.Features.Clientes.Commands.CreateClienteCommand
{
   public class CreateClienteCommand : IRequest<Response<int>>
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public string Direccion { get; set; }
    }


    public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Cliente> _repositoryAsync;
        private readonly IMapper mapper;

        public CreateClienteCommandHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            this.mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateClienteCommand request, CancellationToken cancellationTaken)
        {
           var newRecord = mapper.Map<Cliente>(request);
           var data = await _repositoryAsync.AddAsync(newRecord);
            return new Response<int>(data.Id);
        }
    }
}
