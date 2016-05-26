using AutoMapper;
using HomeCinema.Entities;
using HomeCinema.Web.Models;
using System.Linq;

namespace HomeCinema.Web.Infrastructure.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            CreateMap<Movie, MovieViewModel>()
                .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
                .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.ID))
                .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.IsAvailable)));

            CreateMap<Genre, GenreViewModel>()
                .ForMember(vm => vm.NumberOfMovies, map => map.MapFrom(g => g.Movies.Count()));

            CreateMap<Customer, CustomerViewModel>();

            CreateMap<Stock, StockViewModel>();

            CreateMap<Rental, RentalViewModel>();

            /*
            Mapper.CreateMap<Movie, MovieViewModel>()
                .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
                .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.ID))
                .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.IsAvailable)));

            Mapper.CreateMap<Genre, GenreViewModel>()
                .ForMember(vm => vm.NumberOfMovies, map => map.MapFrom(g => g.Movies.Count()));
            */
        }

        //protected override void Configure()
        //{
        //    new MapperConfiguration(cfg => {
        //        cfg.CreateMap<Movie, MovieViewModel>()
        //            .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
        //            .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.ID))
        //            .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.IsAvailable)));

        //        cfg.CreateMap<Genre, GenreViewModel>()
        //            .ForMember(vm => vm.NumberOfMovies, map => map.MapFrom(g => g.Movies.Count()));
        //    });
        //}
    }
}