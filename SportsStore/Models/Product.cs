using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    public class Product
    {
        public long ProductId { get; set; }

        [Required(ErrorMessage = "����������, ������� ������������ ������")]
        public string Name { get; set; }

        [Required(ErrorMessage = "����������, ������� ��������")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        [Required(ErrorMessage = "����������, ������� ������������� ����")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "����������, ������� ���������")]
        public string Category { get; set; }
    }
}