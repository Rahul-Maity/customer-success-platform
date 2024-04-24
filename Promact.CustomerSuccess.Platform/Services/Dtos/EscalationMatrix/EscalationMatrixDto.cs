﻿using Promact.CustomerSuccess.Platform.Entities.Constants;
using Promact.CustomerSuccess.Platform.Services.Dtos.User;
using Volo.Abp.Application.Dtos;

namespace Promact.CustomerSuccess.Platform.Services.Dtos.EscalationMatrix
{
    public class EscalationMatrixDto : IEntityDto<Guid>
    {
        public Guid Id { get; set; }
        public EscalationMatrixLevels Level { get; set; }
        public EscalationType EscalationType { get; set; }
        public UserDto ResponsiblePerson { get; set; }
        public Guid ProjectId { get; set; }
    }
}