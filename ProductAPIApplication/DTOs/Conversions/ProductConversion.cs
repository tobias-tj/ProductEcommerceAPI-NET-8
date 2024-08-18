using ProductAPIDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPIApplication.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO productDTO) => new()
        {
            Id = productDTO.Id,
            Name = productDTO.Name,
            Quantity = productDTO.Quantity,
            Price = productDTO.Price
        };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            // return single
            if(product is not null || products is null)
            {
                var singleProduct = new ProductDTO
                    (
                        product!.Id,
                        product.Name!,
                        product.Quantity,
                        product.Price
                    );
                return (singleProduct, null);
            }

            //return list
            if(products is not null || product is null)
            {
                var _products = products!.Select(p => 
                    new ProductDTO(p.Id, p.Name!, p.Quantity, p.Price)).ToList();

                return(null, _products);
            }

            return(null, null);
        }

    }
}
