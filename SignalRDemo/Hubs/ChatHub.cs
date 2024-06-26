﻿using Microsoft.AspNetCore.SignalR;
using SignalRDemo.Contexts;
using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    public class ChatHub:Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly ChatDbContext _context;

        public ChatHub(ILogger<ChatHub> logger, ChatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Send(string user,string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);

            Message msg = new Message()
            {
                UserName = user,
                MessageText = message
            };

            _context.Messages.Add(msg);

            await _context.SaveChangesAsync();
        }

        public async Task JoinGroup(string groupName, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.OthersInGroup(groupName).SendAsync("NewMemberJoin", userName, groupName);

            _logger.LogInformation(Context.ConnectionId);
        }



        public async Task SendMessageToGroup(string groupName, string sender, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessageFromGroup", sender, message);

            Message msg = new Message()
            {
                MessageText = message,
                UserName = sender
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();
        }
    }
}
