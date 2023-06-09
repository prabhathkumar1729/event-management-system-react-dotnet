﻿using BookMyEvent.DLL.Contracts;
using db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyEvent.DLL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        EventManagementSystemTeamZealContext context { get; set; }
        public TransactionRepository(EventManagementSystemTeamZealContext context)
        {
            this.context = context;
        }
        public List<Transaction> GetTransactionsByUserId(Guid UserId)
        {
            return context.Transactions.Where(e => e.UserId.Equals(UserId)).ToList();
        }

        public async Task<Guid> GetUserIdByTransactionId(Guid transactionId)
        {
            try
            {
                Transaction? transaction = await context.Transactions.FindAsync(transactionId);
                if (transaction != null)
                {
                    Guid userId = transaction.UserId;
                    return userId;
                }
                else { return Guid.Empty; }
            }
            catch
            {
                return Guid.Empty; // or any appropriate value indicating an error occurred
            }
        }
        public List<Transaction> GetTransactionsByEventId(Guid EventId)
        {
            return context.Transactions.Where(e => e.EventId.Equals(EventId)).ToList();
        }
        public async Task<Transaction> AddTransaction(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    context.Transactions.Add(transaction);
                    await context.SaveChangesAsync();
                    await context.Entry(transaction).GetDatabaseValuesAsync();
                    return transaction;
                }
                else
                {
                    return new Transaction();
                }
            }
            catch (Exception ex)
            {
                return new Transaction();
            }
        }
        public async Task<Transaction> DeleteTransaction(Guid TransactionId)
        {
            try
            {
                var transaction = await context.Transactions.FirstOrDefaultAsync(e => e.TransactionId.Equals(TransactionId));
                if (transaction != null)
                {
                    context.Transactions.Remove(transaction);
                    context.SaveChanges();
                    return transaction;
                }
                return new Transaction();
            }
            catch (Exception e)
            {
                return new Transaction();
            }
        }
    }
}
