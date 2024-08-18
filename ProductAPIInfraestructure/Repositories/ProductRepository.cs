using ecommerceSharedLibrary.Logs;
using ecommerceSharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductAPIApplication.Interfaces;
using ProductAPIDomain.Entities;
using ProductAPIInfraestructure.Data;
using System.Linq.Expressions;

namespace ProductAPIInfraestructure.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context; 
        }

        public async Task<ResponseModel> CreateAsync(Product entity)
        {
            try
            {
                // check if the product already exist
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if(getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new ResponseModel(false, $"{entity.Name} already added");
                }

                var currentEntity = _context.Products.Add(entity).Entity;
                await _context.SaveChangesAsync();
                if(currentEntity is not null && currentEntity.Id > 0)
                {
                    return new ResponseModel(true, $"{entity.Name} added to database successfully");
                }
                else
                {
                    return new ResponseModel(false, $"Error ocurred while adding {entity.Name}");
                }
            }catch(Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                return new ResponseModel(false, "Error ocurred adding new product");
            }
        }

        public async Task<ResponseModel> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product == null)
                {
                    return new ResponseModel(false, $"{entity.Name} not found");
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new ResponseModel(true, $"{entity.Name} is deleted succesfully!");
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                return new ResponseModel(false, "Error ocurred deleting product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                return product != null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                throw new Exception("Error ocurred retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products.AsNoTracking().ToListAsync();
                return products != null ? products : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                throw new InvalidOperationException("Error ocurred retrieving products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await _context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product != null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                throw new InvalidOperationException("Error ocurred retrieving products");
            }
        }

        public async Task<ResponseModel> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                {
                    return new ResponseModel(false, $"{entity.Name} not found!");
                }
                _context.Entry(product).State = EntityState.Detached;
                _context.Products.Update(entity);
                await _context.SaveChangesAsync();
                return new ResponseModel(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
               return new ResponseModel(false, "Error ocurred updating products");
            }
        }
    }
}
