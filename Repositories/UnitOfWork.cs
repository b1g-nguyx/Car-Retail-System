using Car_Rental_System.Data;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using Microsoft.EntityFrameworkCore.Storage;



namespace Car_Rental_System.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;
        public virtual IRepository<Car> _carRepository { get; set;}
        public virtual IRepository<Images> _imageRepository { get; set;}
        public virtual IRepository<Category> _categoryRepository { get; set;}
        public virtual IRepository<Brand> _brandRepository { get; set;}


        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _carRepository = new Repository<Car>(_dbContext);
            _imageRepository = new Repository<Images>(_dbContext);
            _categoryRepository = new Repository<Category>(_dbContext);
            _brandRepository = new Repository<Brand>(_dbContext);
        }

        /// <summary>
        /// Creates a new transaction for this unit of work.
        /// </summary>
        /// <remarks>
        /// This method must be called before any other operations are performed.
        /// </remarks>
        public virtual void CreateTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// Asynchronously creates a new transaction for this unit of work.
        /// </summary>
        /// <remarks>
        /// This method must be called before any other asynchronous operations are performed.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>

        public virtual async Task CreateTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public virtual void Commit()
        {
            ExecuteTransaction(() =>
            {
                SaveChanges();
                _transaction?.Commit();
            });
        }

        /// <summary>
        /// Asynchronously commits the current transaction.
        /// </summary>
        /// <remarks>
        /// This method ensures that all changes are saved and the transaction is committed.
        /// If no transaction exists, no action is taken.
        /// </remarks>
        /// <returns>A task that represents the asynchronous commit operation.</returns>

        public virtual async Task CommitAsync()
        {
            await ExecuteTransactionAsync(async () =>
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            });
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        /// <remarks>
        /// This method ensures that all changes are rolled back and the transaction is disposed.
        /// If no transaction exists, no action is taken.
        /// </remarks>
        public virtual void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        public virtual async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public virtual int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database.</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                    _transaction?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Executes a transactional operation.
        /// If the operation fails, the transaction is rolled back and the exception is re-thrown.
        /// </summary>
        /// <param name="action">The operation to execute.</param>
        public void ExecuteTransaction(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
            }
        }


        /// <summary>
        /// Executes a transactional operation asynchronously.
        /// If the operation fails, the transaction is rolled back and the exception is re-thrown.
        /// </summary>
        /// <param name="action">The asynchronous operation to execute.</param>
        public async Task ExecuteTransactionAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                }
            }
        }
    }
}

