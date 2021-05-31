using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace BlazingPizza.Server
{
    [Route("orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly PizzaStoreContext _db;
        private readonly IMediator _mediator;

        public OrdersController(PizzaStoreContext db,
            IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
        {
            var model = await _mediator.Send(new Features.Queries.GetOrdersByUserId.Query { UserId = GetUserId() });
            return model.Orders.ToList();

        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
        {
            var model = await _mediator.Send(new Features.Queries.GetOrder.Query { OrderId = orderId, UserId = GetUserId() });

            if (model.Order == null)
            {
                return NotFound();
            }

            return Ok(model.Order);
        }

        [HttpPost]
        public async Task<ActionResult<int>> PlaceOrder(Order order)
        {
            order.CreatedTime = DateTime.Now;
            order.DeliveryLocation = new LatLong(51.5001, -0.1239);
            order.UserId = GetUserId();

            var command = await _mediator.Send(new Features.Commands.PlaceOrder.Command { Order = order });

            if (command.SuccessfullySaved)
            {
                // In the background, send push notifications if possible
                var subscription = await _db.NotificationSubscriptions.Where(e => e.UserId == GetUserId()).SingleOrDefaultAsync();
                if (subscription != null)
                {
                    _ = TrackAndSendNotificationsAsync(command.SavedOrder, subscription);
                }

                return Ok(command.SavedOrder.OrderId);
            }
            else
            {
                // Log error
                return StatusCode(403, command.Message);
            }
            
        }

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private static async Task TrackAndSendNotificationsAsync(Order order, NotificationSubscription subscription)
        {
            // In a realistic case, some other backend process would track
            // order delivery progress and send us notifications when it
            // changes. Since we don't have any such process here, fake it.
            await Task.Delay(OrderWithStatus.PreparationDuration);
            await SendNotificationAsync(order, subscription, "Your order has been dispatched!");

            await Task.Delay(OrderWithStatus.DeliveryDuration);
            await SendNotificationAsync(order, subscription, "Your order is now delivered. Enjoy!");
        }

        private static async Task SendNotificationAsync(Order order, NotificationSubscription subscription, string message)
        {
            // For a real application, generate your own
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"myorders/{order.OrderId}",
                });
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error sending push notification: " + ex.Message);
            }
        }
    }
}
