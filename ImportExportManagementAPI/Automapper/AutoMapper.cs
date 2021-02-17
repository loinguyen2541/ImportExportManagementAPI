using AutoMapper;
using ImportExportManagement_API.DTO;
using ImportExportManagement_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagement_API.Automapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            //Partner
            // config chuyển đổi từ Partner => PartnerDTO và ngược lại
            CreateMap<Partner, PartnerDTO>();
            CreateMap<PartnerDTO, Partner>();
        }
        
    }
}
