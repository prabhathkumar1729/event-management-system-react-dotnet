﻿using BookMyEvent.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyEvent.BLL.Contracts
{
    public interface ITransactionServices
    {
        Task<BLTransaction> Add(BLTransaction transaction);
        Task<BLTransaction> Update(BLTransaction transaction);
        Task<bool> Delete(Guid TransactionId);
        Task<List<BLTransaction>> GetAll();
    }
}
