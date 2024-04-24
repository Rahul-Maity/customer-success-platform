﻿using Microsoft.AspNetCore.Authorization;
using Promact.CustomerSuccess.Platform.Constants;
using Promact.CustomerSuccess.Platform.Entities;
using Promact.CustomerSuccess.Platform.Services.Dtos.ClientFeedback;
using Promact.CustomerSuccess.Platform.Services.Emailing;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Promact.CustomerSuccess.Platform.Services.ClientFeedbacks
{
    [Authorize]
    public class ClientFeedbackService : CrudAppService<ClientFeedback, ClientFeedbackDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateCLientFeedback, CreateUpdateCLientFeedback>, IClientFeedbackService
    {
        IRepository<ClientFeedback, Guid> _repository;
        IEmailService _emailService;
        public ClientFeedbackService(IRepository<ClientFeedback, Guid> repository, IEmailService emailService) : base(repository)
        {
            _emailService = emailService;
            _repository = repository;
        }

        [Authorize(Policy = PolicyName.ClientFeedbackCreatePolicy)]
        public override async Task<ClientFeedbackDto> CreateAsync(CreateUpdateCLientFeedback input)
        {
            var clientFeedback = await base.CreateAsync(input);

            var projectId = input.ProjectId;

            var projectDetail = new EmailToStakeHolderDto
            {
                Subject = "Approved Team Created alert",
                Body = Template.GenerateClientFeedbackEmailBody(clientFeedback, "Created"),
                ProjectId = projectId,
            };
            await _emailService.SendEmailToStakeHolder(projectDetail);

            return clientFeedback;
        }
        [Authorize(Policy = PolicyName.ClientFeedbackUpdatePolicy)]
        public override async Task<ClientFeedbackDto> UpdateAsync(Guid id, CreateUpdateCLientFeedback input)
        {
            var ClientFeedbackDto = await base.UpdateAsync(id, input);

            var projectId = input.ProjectId;

            var projectDetail = new EmailToStakeHolderDto
            {
                Subject = "Cleint Feedback Updated alert",
                Body = Template.GenerateClientFeedbackEmailBody(ClientFeedbackDto, "Updated"),
                ProjectId = projectId,
            };
           await _emailService.SendEmailToStakeHolder(projectDetail);

            return ClientFeedbackDto;
        }
        [Authorize(Policy = PolicyName.ClientFeedbackDeletePolicy)]
        public override async Task DeleteAsync(Guid id)
        {
            // Retrieve approved team to get details before deletion
            var cleintFeedback = await _repository.GetAsync(id);

            // Perform deletion
            await base.DeleteAsync(id);

            // Send email notification
            var projectId = cleintFeedback.ProjectId;

            var projectDetail = new EmailToStakeHolderDto
            {
                Subject = "Cleint Feedback Deleted alert",
                Body = Template.GetClientFeedbackEmailBody(ObjectMapper.Map<ClientFeedback, ClientFeedbackDto>(cleintFeedback), "Deleted"),
                ProjectId = projectId,
            };
            await _emailService.SendEmailToStakeHolder(projectDetail);
        }


        public async Task<List<ClientFeedbackDto>> GetClientFeedbackByProjectIdAsync(Guid projectId)
        {
            var clientFeedback = await _repository.GetListAsync(t => t.ProjectId == projectId);
            return ObjectMapper.Map<List<ClientFeedback>, List<ClientFeedbackDto>>(clientFeedback);
        }
    }
}
