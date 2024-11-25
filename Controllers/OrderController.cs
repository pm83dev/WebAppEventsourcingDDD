﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Events;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        //Model schema for API
        public class CreateOrderRequest
        {
            public int OrderQty { get; set; }
        }

        public class ModifyOrder
        {
            public Guid OrderId { get; set; }
            public int NewOrderQty { get; set; }
        }

        public class GetSingleEventsOrder
        {
            public Guid OrderIdFilter { get; set; }
        }
        
        private readonly CommandHandler _commandHandler;
       private readonly IEventListener _eventListener;
        public OrderController(CommandHandler handler, IEventListener eventListener)
        {
            _commandHandler = handler;
            _eventListener = eventListener;
        }
        
        
        
        // API REST
        [HttpGet("GetRabbitMessage")]
        public IActionResult GetRabbitMessage()
        {
            // Ottieni i messaggi dalla coda
            var messages = _eventListener.GetMessages();
    
            if (messages != null && messages.Any())  // Verifica se ci sono messaggi
            {
                return Ok(messages);
            }

            return NoContent();  // Se non ci sono messaggi, restituisce un 204 No Content
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
                await _commandHandler.HandleCreateOrderAsync(orderId, request.OrderQty);
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
            if (request == null || request.OrderId == Guid.Empty || request.NewOrderQty <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _commandHandler.HandleUpdateOrderAsync(request.OrderId, request.NewOrderQty);
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