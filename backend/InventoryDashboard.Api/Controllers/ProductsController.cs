using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDashboard.Api.Entities;
using InventoryDashboard.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryDashboard.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
    
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productService.GetAll();
            return Ok(products);
        }

    }
}