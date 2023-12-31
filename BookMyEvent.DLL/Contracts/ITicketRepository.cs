﻿using db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyEvent.DLL.Contracts
{
    //It is used to perform Operations on  Ticket Table//
    public interface ITicketRepository
    {
        /// <summary>
        /// This Method is used to add a Ticket to the ticket table
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns>Newly Added Ticket.</returns>
        public Task<Ticket> AddTicket(Ticket ticket);
        /// <summary>
        /// This method is used to add many tickets to the tickets table
        /// </summary>
        /// <param name="ticketList"></param>
        /// <returns>It returns newly added tickets list</returns>
        
        public Task<bool> AddMultipleTickets(List<Ticket> ticketList);
        /// <summary>
        /// it id used to get all the tickets associated with transaction Id
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>It true if tickets were added succesfully and NUll if there's any exception </returns>
        public Task<List<Ticket>> GetTicketByTransactionId(Guid transactionId);

        /// <summary>
        /// This mehod is to get Ticket by UserInputFormId
        /// </summary>
        /// <param name="FormId"></param>
        /// <returns> Ticket </returns>
        public Task<Ticket> GetTicketByFormId(Guid FormId);
        /// <summary>
        /// It is to get a ticket by taking Id as input
        /// </summary>
        /// <param name="TicketId"></param>
        /// <returns>Ticket with ticketId</returns>
       
        public Task<Ticket> GetTicketByTicketId(Guid TicketId);
        /// <summary>
        /// to get all the tickets of particular form IDs
        /// </summary>
        /// <param name="FormIds"></param>
        /// <returns></returns>
        public Task<List<Ticket>> GetTicketsByFormids(List<Guid> FormIds);

        /// <summary>
        /// This method is to get all the tickets of an Event
        /// </summary>
        /// <param name="EventId"> Event Id </param>
        /// <returns> List of Tickets of an Event </returns>
        public Task<List<Ticket>?> GetTicketByEventId(Guid EventId);
        /// <summary>
        /// It id for Cancelling the ticket by taking the ticket Id as Parameter
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns>Cancelled Ticket</returns>
        public Task<Ticket> UpdateIsCanceled(Guid ticketId);
    }
}
