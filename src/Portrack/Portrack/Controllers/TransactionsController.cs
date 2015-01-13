﻿using Microsoft.AspNet.Identity.Owin;
using Portrack.Models.Application;
using Portrack.Repositories.AspAuth;
using Portrack.Repositories.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Portrack.Controllers
{
    [Authorize]
    [RoutePrefix("transactions")]
    public class TransactionsController : ApiController
    {
        private AspAuthUserManager _userManager;
        private readonly IServicesRepository _repository;

        public AspAuthUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AspAuthUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public TransactionsController(IServicesRepository repository)
        {
            _repository = repository;
        }


        [Route("{portfolioName?}/{tickers?}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string portfolioName = null, string tickers = null)
        {
            if (string.IsNullOrWhiteSpace(portfolioName))
            {
                ICollection<Transaction> transactions = await _repository.GetTransactionsAsync(User.Identity.Name);
                return Ok(transactions);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(tickers))
                {
                    ICollection<Transaction> transactions = await _repository.GetTransactionsAsync(User.Identity.Name, portfolioName);
                    return Ok(transactions);
                }
                else
                {
                    IEnumerable<string> tickerEnum = tickers.Split(',').Select(s => s.Trim());
                    ICollection<Transaction> transactions = await _repository.GetTransactionsAsync(User.Identity.Name, portfolioName, tickerEnum);
                    return Ok(transactions);
                }
            }
        }


        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (transaction == null) return BadRequest("Error while deserializing the transaction");

            Portfolio portfolio = await _repository.GetPortfolioAsync(User.Identity.Name, transaction.PortfolioName);
            if (portfolio == null)
            {
                Ok(TransactionResult.Failed("The portfolio designated by this transaction doesn't exist."));
            }

            Position position = await _repository.GetPositionAsync(User.Identity.Name, transaction.PortfolioName, transaction.Ticker);
            if (position == null)
            {
                if (transaction.ShareAmount <= 0)
                {
                    return Ok(TransactionResult.Failed("Cannot add a null or negative transaction to a non-existing position."));
                }
                else
                {
                    Instrument instrument = await _repository.GetInstrumentAsync(transaction.Ticker);
                    if (instrument == null)
                    {
                        return Ok(TransactionResult.Failed("This ticker doens't exist."));
                    }
                    else
                    {
                        position = _repository.AddPosition(new Position(portfolio, instrument, transaction.ShareAmount));
                        Transaction createdTransaction = _repository.AddTransaction(transaction);

                        if (await _repository.SaveAsync() > 0)
                            return Ok(TransactionResult.Success);
                        else
                            return Ok();
                    }
                }
            }
            else
            {
                if (position.ShareAmount + transaction.ShareAmount < 0)
                {
                    return Ok(TransactionResult.Failed("Not enough shares for this ticker."));
                }
                else if (position.ShareAmount + transaction.ShareAmount == 0)
                {
                    await _repository.DeletePositionAsync(User.Identity.Name, transaction.PortfolioName, transaction.Ticker);                  
                    Transaction createdTransaction = _repository.AddTransaction(transaction);

                    if (await _repository.SaveAsync() > 0)
                        return Ok(TransactionResult.Success);
                    else
                        return Ok();
                }
                else
                {
                    position.ShareAmount += transaction.ShareAmount;                   
                    Transaction createdTransaction = _repository.AddTransaction(transaction);

                    if (await _repository.SaveAsync() > 0)
                        return Ok(TransactionResult.Success);
                    else
                        return Ok();
                }
            }
        }
    }
}