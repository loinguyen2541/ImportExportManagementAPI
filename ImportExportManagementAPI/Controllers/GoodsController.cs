using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportExportManagement_API;
using ImportExportManagement_API.Models;
using ImportExportManagementAPI.Repositories;

namespace ImportExportManagementAPI
{
    [Route("api/goods")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private GoodsRepository repo;

        public GoodsController()
        {
            repo = new GoodsRepository();
        }

        // GET: api/Goods
        [HttpGet]
        public ActionResult<List<Goods>> GetGoods()
        {
            return repo.GetGoods();
        }

        // GET: api/Goods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Goods>> GetGoods(int id)
        {
            var goods = await repo.GetByIDAsync(id);

            if (goods == null)
            {
                return NotFound();
            }

            return goods;
        }

        // PUT: api/Goods/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoods(int id, Goods goods)
        {
            if (id != goods.GoodsId)
            {
                return BadRequest();
            }
            repo.Update(goods);

            try
            {
                await repo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!repo.Exist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Goods
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Goods>> PostGoods(Goods goods)
        {
            repo.Insert(goods);
            await repo.SaveAsync();

            return CreatedAtAction("GetGoods", new { id = goods.GoodsId }, goods);
        }

        // DELETE: api/Goods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoods(int id)
        {
            var goods = await repo.GetByIDAsync(id);
            if (goods == null)
            {
                return NotFound();
            }

            repo.DeleteGoods(goods);
            await repo.SaveAsync();

            return NoContent();
        }

     
    }
}
