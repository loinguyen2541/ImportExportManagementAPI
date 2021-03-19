﻿using ImportExportManagementAPI.Models;
using ImportExportManagementAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Controllers
{
    [Route("api/timetemplateitems")]
    [ApiController]
    public class TimeTemplateItemsController : ControllerBase
    {
        private readonly TimeTemplateItemRepository _timeTemplateItemRepository;
        public TimeTemplateItemsController()
        {
            _timeTemplateItemRepository = new TimeTemplateItemRepository();
        }
        [HttpGet("current")]
        public async Task<ActionResult<List<TimeTemplateItem>>> GetCurrent()
        {
            List<TimeTemplateItem> timeTemplateItems = await _timeTemplateItemRepository.GetAppliedItem();
            return Ok(timeTemplateItems);
        }
    }
}