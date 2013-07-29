
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace iConnect.Common
{
    
    public sealed class ExpandableCollection<T> : IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _inner;
        private readonly ExpandableQueryProvider<T> _provider;

        // Original query, that we're wrapping

        internal ExpandableCollection(IQueryable<T> inner)
        {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        internal IQueryable<T> InnerQuery
        {
            get { return _inner; }
        }


        Expression IQueryable.Expression
        {
            get { return _inner.Expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof (T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public override string ToString()
        {
            return _inner.ToString();
        }
    }

    internal class ExpandableQueryProvider<T> : IQueryProvider
    {
        private readonly ExpandableCollection<T> _query;

        internal ExpandableQueryProvider(ExpandableCollection<T> query)
        {
            _query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

       

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new ExpandableCollection<TElement>(_query.InnerQuery.Provider.CreateQuery<TElement>(expression.Expand()));
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.Expand());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute<TResult>(expression.Expand());
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute(expression.Expand());
        }

      
    }
}