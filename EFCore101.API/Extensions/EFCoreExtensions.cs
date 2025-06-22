namespace EFCore101.API.Extensions;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

public static partial class EFCoreExtensions
{
	public static ModelBuilder ApplyGlobalFilters<TEntityType>(this ModelBuilder modelBuilder, Expression<Func<TEntityType, bool>> expression)
		 where TEntityType : class
	{
		var entities = modelBuilder.Model
			.GetEntityTypes()
			.Select(t => t.ClrType)
			.Where(t => t.BaseType == typeof(TEntityType));

		foreach (var entity in entities)
		{
			var newParam = Expression.Parameter(entity);
			var newBody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
			modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newBody, newParam));
		}

		return modelBuilder;
	}
}
