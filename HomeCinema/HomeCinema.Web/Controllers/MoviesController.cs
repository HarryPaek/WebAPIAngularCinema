using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Web.Infrastructure.Core;
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
    [RoutePrefix("api/movies")]
    public class MoviesController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Movie> _moviesRepository;
        private readonly IEntityBaseRepository<Rental> _rentalsRepository;
        private readonly IEntityBaseRepository<Stock> _stocksRepository;
        private readonly IEntityBaseRepository<Customer> _customersRepository;

        public MoviesController(IEntityBaseRepository<Movie> moviesRepository, IEntityBaseRepository<Rental> rentalsRepository, IEntityBaseRepository<Stock> stocksRepository, IEntityBaseRepository<Customer> customersRepository,
                                IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork) : base(errorsRepository, unitOfWork)
        {
            _moviesRepository = moviesRepository;
            _rentalsRepository = rentalsRepository;
            _stocksRepository = stocksRepository;
            _customersRepository = customersRepository;
        }

        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movies = _moviesRepository.GetAll().OrderByDescending(m => m.ReleaseDate).Take(6).ToList();

                IEnumerable<MovieViewModel> moviesVM = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);
                response = request.CreateResponse<IEnumerable<MovieViewModel>>(HttpStatusCode.OK, moviesVM);

                return response;
            });
        }

        [AllowAnonymous]
        [Route("{page:int=0}/{pageSize=6}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IQueryable<Movie> movieQuery = null;

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    movieQuery = _moviesRepository.GetAll()
                        .OrderBy(m => m.ID)
                        .Where(m => m.Title.ToLower().Contains(filter.ToLower().Trim()));
                }
                else
                {
                    movieQuery = _moviesRepository.GetAll().OrderBy(m => m.ID);
                }

                int totalMovies = movieQuery.Count();
                var movies = movieQuery.Skip(currentPage * currentPageSize)
                                       .Take(currentPageSize)
                                       .ToList();

                IEnumerable<MovieViewModel> moviesVM = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

                PaginationSet<MovieViewModel> pagedSet = new PaginationSet<MovieViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                    Items = moviesVM
                };

                response = request.CreateResponse<PaginationSet<MovieViewModel>>(HttpStatusCode.OK, pagedSet);

                return response;
            });
        }

        [Route("details/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movie = _moviesRepository.GetSingle(id);
                
                MovieViewModel movieVM = Mapper.Map<Movie, MovieViewModel>(movie);
                response = request.CreateResponse<MovieViewModel>(HttpStatusCode.OK, movieVM);
                
                return response;
            });
        }
    }
}
