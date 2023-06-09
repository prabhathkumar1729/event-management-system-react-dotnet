﻿using BookMyEvent.BLL.Contracts;
using BookMyEvent.BLL.Models;
using BookMyEvent.DLL.Contracts;
using db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyEvent.BLL.Services
{
    public class TicketServices:ITicketServices
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserInputFormRepository _userInputFormRepository;
        private readonly IUserInputFormFieldsRepository _userInputFormFieldsRepository;
        public TicketServices(ITicketRepository ticketRepository, IUserInputFormRepository userInputFormRepository, IUserInputFormFieldsRepository userInputFormFieldsRepository)
        {
            _ticketRepository = ticketRepository;
            _userInputFormRepository = userInputFormRepository;
            _userInputFormFieldsRepository = userInputFormFieldsRepository;
        }

        public async Task<BLTicket?> AddTickect(BLTicket ticket)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                Ticket ticketDL = mapper.Map<Ticket>(ticket);
                Ticket newTicketDL = await _ticketRepository.AddTicket(ticketDL);
                if (newTicketDL == null) return null;
                BLTicket newTicketBL = mapper.Map<BLTicket>(newTicketDL);
                return newTicketBL;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<(BLTicket ticket, List<BLUserInputFormField> userDetails)>> GetAllTicketsByTransactionId(Guid transactionId)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                List<(BLTicket ticket, List<BLUserInputFormField> userDetails)> AllTickets = new List<(BLTicket ticket, List<BLUserInputFormField> userDetails)>();
                List<Ticket> tickets = await _ticketRepository.GetTicketByTransactionId(transactionId);
                foreach (var ticket in tickets)
                {
                    Guid userInputFormId = ticket.UserInputFormId;
                    List<UserInputFormField> userInputFormFields = await _userInputFormFieldsRepository.GetAllFieldsByFormId(userInputFormId);
                    List<BLUserInputFormField> userInputFormFieldsBL = new List<BLUserInputFormField>();
                    foreach (var field in userInputFormFields)
                    {
                        BLUserInputFormField BLField = mapper.Map<BLUserInputFormField>(field);
                        userInputFormFieldsBL.Add(BLField);
                    }
                    BLTicket ticketBL = mapper.Map<BLTicket>(ticket);
                    (BLTicket ticket, List<BLUserInputFormField> userDetails) ticketWithDetails;
                    ticketWithDetails.ticket = ticketBL;
                    ticketWithDetails.userDetails = userInputFormFieldsBL;
                    AllTickets.Add(ticketWithDetails);
                }
                return AllTickets;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<(BLTicket ticket, List<BLUserInputFormField> userDetails)>?> GetUserEventTickets(Guid userId, Guid eventId)
        {
            try
            {
                var mapper = Automapper.InitializeAutomapper();
                List<(BLTicket ticket, List<BLUserInputFormField> userDetails)> AllTickets = new List<(BLTicket ticket, List<BLUserInputFormField> userDetails)>();
                List<Guid>? formIds = await _userInputFormRepository.GetInputFormIdByUserIdAndEventId(userId, eventId);
                foreach(var formId in formIds)
                {
                    Ticket DLticket = await _ticketRepository.GetTicketByFormId(formId);
                    List<UserInputFormField> userInputFormFields = await _userInputFormFieldsRepository.GetAllFieldsByFormId(formId);
                    List<BLUserInputFormField> userInputFormFieldsBL = new List<BLUserInputFormField>();
                    foreach (var field in userInputFormFields)
                    {
                        BLUserInputFormField BLField = mapper.Map<BLUserInputFormField>(field);
                        userInputFormFieldsBL.Add(BLField);
                    }
                    BLTicket ticketBL = mapper.Map<BLTicket>(DLticket);
                    (BLTicket ticket, List<BLUserInputFormField> userDetails) ticketWithDetails;
                    ticketWithDetails.ticket = ticketBL;
                    ticketWithDetails.userDetails = userInputFormFieldsBL;
                    AllTickets.Add(ticketWithDetails);
                }
                return AllTickets;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CancelTicket(Guid ticketId)
        {
            try
            {
                Ticket cancelledTicket = await _ticketRepository.UpdateIsCanceled(ticketId);
                if (cancelledTicket != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
