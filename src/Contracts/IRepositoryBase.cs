using System.Linq.Expressions;

namespace Contracts;

public interface IRepositoryBase<T>
{
	IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
	void Create(T entity);
	IQueryable<T> FindAll(bool trackChanges);
}