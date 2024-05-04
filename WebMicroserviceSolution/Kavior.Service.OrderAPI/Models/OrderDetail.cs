using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kavior.Service.OrderAPI.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        // cái lí do mà để lưu cả tên và giá ở dây là
        // giả sử sau này, nếu giá của một sản phẩm bị thay đổi, chúng tôi muốn hiển thị cái giá của sản phẩm lúc đặt hàng
        public string ProductName { get; set; }
        public double Price { get; set; }


    }
}
