using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.BasketModule;
using ServiceAbstraction;
using Shared.DataTransferObjects.BasketModuleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BasketService(IBasketRepository _basketRepository ,IMapper _mapper) : IBasketService
    {
        public async Task<BasketDto> CreateOrUpdateBasketAsync(BasketDto basket)
        {
            var CustomerBasket = _mapper.Map<BasketDto,CustomerBasket>(basket);
            var CreatedOrUpdated = await _basketRepository.CreateOrUpdateAsync(CustomerBasket);
            if (CreatedOrUpdated is not null)
                return await GetBasketAsync(basket.Id);
            else
                throw new Exception("Can Not Create Or Update Basket Now Try Again Later");
        }

        public async Task<bool> DeleteBasketAsync(string Key) => await _basketRepository.DeleteBasketAsync(Key);

        public async Task<BasketDto> GetBasketAsync(string Key)
        {
            var Basket = await _basketRepository.GetBasketAsync(Key);
            if(Basket is not null)
                return _mapper.Map<CustomerBasket,BasketDto>(Basket);
            else
                throw new BasketNotFoundException(Key);
        }
    }
}
