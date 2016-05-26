using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Extensions;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Web.Infrastructure.Core;
using HomeCinema.Web.Infrastructure.Extensions;
using HomeCinema.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HomeCinema.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Customer> _customersRepository;

        public CustomersController(IEntityBaseRepository<Customer> customersRepository,
                                   IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork) : base(errorsRepository, unitOfWork)
        {
            _customersRepository = customersRepository;
        }

        public HttpResponseMessage Get(HttpRequestMessage request, string filter)
        {
            filter = filter.ToLower().Trim();

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var customers = _customersRepository.GetAll()
                    .Where(c => c.Email.ToLower().Contains(filter) ||
                                c.FirstName.ToLower().Contains(filter) ||
                                c.LastName.ToLower().Contains(filter))
                    .ToList();

                var customersVm = Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerViewModel>>(customers);

                response = request.CreateResponse<IEnumerable<CustomerViewModel>>(HttpStatusCode.OK, customersVm);

                return response;
            });
        }

        [HttpGet]
        [Route("search/{page:int=0}/{pageSize=6}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IQueryable<Customer> customerQuery = null;

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    customerQuery = _customersRepository.GetAll()
                        .OrderBy(c => c.ID)
                        .Where(c => c.LastName.ToLower().Contains(filter) ||
                                    c.IdentityCard.ToLower().Contains(filter) ||
                                    c.FirstName.ToLower().Contains(filter));
                }
                else
                {
                    customerQuery = _customersRepository.GetAll().OrderBy(c => c.ID);
                }

                int totalCustomers = customerQuery.Count();
                var customers = customerQuery.Skip(currentPage * currentPageSize)
                                     .Take(currentPageSize)
                                     .ToList();

                IEnumerable<CustomerViewModel> customersVM = Mapper.Map<IEnumerable<Customer>,IEnumerable<CustomerViewModel>>(customers);

                PaginationSet<CustomerViewModel> pagedSet = new PaginationSet<CustomerViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalCustomers,
                    TotalPages = (int)Math.Ceiling((decimal)totalCustomers / currentPageSize),
                    Items = customersVM
                };

                response = request.CreateResponse<PaginationSet<CustomerViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, CustomerViewModel customer) {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Customer _customer = _customersRepository.GetSingle(customer.ID);
                    _customer.UpdateCustomer(customer);
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(HttpRequestMessage request, CustomerViewModel customer)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                                                                                .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    if (_customersRepository.UserExists(customer.Email, customer.IdentityCard))
                    {
                        ModelState.AddModelError("Invalid user", "Email or Identity Card number already exists");
                        response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                                                                                    .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        Customer newCustomer = new Customer();

                        newCustomer.UpdateCustomer(customer);
                        _customersRepository.Add(newCustomer);

                        _unitOfWork.Commit();
                        
                        // Update view model
                        customer = Mapper.Map<Customer, CustomerViewModel>(newCustomer);
                        response = request.CreateResponse<CustomerViewModel>(HttpStatusCode.Created, customer);
                    }
                }
                
                return response;
            });
        }
    }
}
