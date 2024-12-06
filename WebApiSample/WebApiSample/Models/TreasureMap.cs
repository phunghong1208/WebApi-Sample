using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiSample.Models
{
    public class TreasureMap
    {
        [Key]
        public int Id { get; set; } // Khóa chính của bản đồ kho báu
        public int N { get; set; } // Số hàng của bản đồ
        public int M { get; set; } // Số cột của bản đồ
        public int P { get; set; } // Số loại rương (kho báu)
        public string Grid { get; set; } // Chuỗi JSON đại diện cho ma trận
        public DateTime CreatedAt { get; set; } // Thời gian tạo bản đồ 
    }
}
