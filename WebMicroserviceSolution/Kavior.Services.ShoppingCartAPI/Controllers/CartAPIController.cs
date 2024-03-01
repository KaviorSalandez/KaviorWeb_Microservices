using AutoMapper;
using Kavior.Services.ShoppingCartAPI.Data;
using Kavior.Services.ShoppingCartAPI.Models;
using Kavior.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Kavior.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        public CartAPIController(AppDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _response = new ResponseDto();
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart( string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(x=>x.UserId== userId))  
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(x=>x.CartHeaderId == cart.CartHeader.Id));

                foreach (var item in cart.CartDetails)
                {
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
        [HttpPost("CartUpSert")]
        public async Task<ResponseDto> CartUpSert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDto.CartHeader.UserId);
                if(cartHeaderFromDb == null)
                {
                    // create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _context.CartHeaders.Add(cartHeader);
                    await _context.SaveChangesAsync();

                    cartDto.CartDetails.First().Id = cartHeader.Id;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();  
                }
                else
                {
                    // if header is not null
                    // check if details has same product
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking()
                                            .FirstOrDefaultAsync(x=>x.ProductId == cartDto.CartDetails.First().ProductId && x.CartHeaderId== cartHeaderFromDb.Id);
                    if(cartDetailsFromDb == null)
                    {
                        // create cartdetails

                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.Id;
                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().Id= cartDetailsFromDb.Id;
                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }




        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _context.CartDetails.First(x=>x.Id== cartDetailsId);

                int totalCountOfCartItem = _context.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                //trước khi xóa cartheader cần xóa cartdetail
                _context.CartDetails.Remove(cartDetails);
                
                if (totalCountOfCartItem == 1)
                {
                    // có nghĩa đây là item cuối cùng mà ng dùng xóa khỏi giỏ hàng -> xóa luôn cartHeader
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(x => x.Id == cartDetails.CartHeaderId);
                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();
              
                _response.Result = true;

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }





    }
}
