using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        public class CreateOrderRequest
        {
            public int OrderQty { get; set; }
        }

        public class ModifyOrder
        {
            public Guid OrderId { get; set; }
            public Guid CustomerId { get; set; }
            public int NewOrderQty { get; set; }
        }

        public class GetSingleEventsOrder
        {
            public Guid OrderIdFilter { get; set; }
        }

        private readonly CommandHandler _commandHandler;

        public OrderController(CommandHandler handler)
        {
            _commandHandler = handler;
        }

        [HttpGet("GetAllOrder")]
        public async Task<IActionResult> IndexOrder()
        {
            // Implementa la logica per ottenere tutti gli ordini
            return Ok();
        }

        [HttpPost("GetSingleOrderEvents")]
        public async Task<IActionResult> GetSingleOrder([FromBody] GetSingleEventsOrder? request)
        {
            if (request == null || request.OrderIdFilter == Guid.Empty)
            {
                return BadRequest("Value not valid.");
            }
            var orderId = request.OrderIdFilter;

            var events = await _commandHandler.HandleGetOrderAsync(orderId);

            return Ok(events);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.OrderQty <= 0)
            {
                return BadRequest("Invalid order quantity.");
            }

            try
            {
                var orderId = Guid.Parse("410efa39-917b-45d4-83ff-f9a618d760a3");
                var customerId = Guid.Parse("410efa39-917b-45d4-83ff-f9a618d760a3");

                await _commandHandler.HandleCreateOrderAsync(orderId, customerId, request.OrderQty);
                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating order: {ex.Message}");
            }
        }

        [HttpPut("ModifyOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody] ModifyOrder request)
        {
            if (request == null || request.OrderId == Guid.Empty || request.CustomerId == Guid.Empty || request.NewOrderQty <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _commandHandler.HandleUpdateOrderAsync(request.OrderId, request.CustomerId, request.NewOrderQty);
                return Ok("Order modified successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error modifying order: {ex.Message}");
            }
        }
        
        [HttpDelete("DeleteOrder/{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                return BadRequest("Invalid order ID.");
            }

            try
            {
                await _commandHandler.HandleDeleteOrderAsync(orderId); 
                return Ok("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting order: {ex.Message}");
            }
        }
    }
}