using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace iConnect.Common
{
    public sealed class Repository<TEntity, TContext> : IRepository<TEntity>
        where TContext : ObjectContext
        where TEntity : EntityObject
    {
        // create a private Context variable to hold the current data access context
        private readonly TContext _context;

        public TContext Context { get { return _context; } }

        public Repository()
        {
            
        } 

        public Repository(string connectionString)
        {
            Type type = typeof(TContext);
            _context = (TContext)Activator.CreateInstance(type,new[]{connectionString});
            _context.CommandTimeout = 300;
        }

        /// <summary>
        /// Public constructor which creates an instance and holds the Context object
        /// </summary>
        /// <param name="context">Actual Object Context for the Repository</param>
        public Repository(TContext context)
        {
            this._context = context;
        }


        /// <summary>
        /// Define Dispose method of IDisposable Interface
        /// </summary>
        public void Dispose()
        {
            if (null != _context)
            {
                _context.Dispose();
            }
        }



        /// <summary>
        /// This method adds the entity object into the current Context
        /// </summary>
        /// <param name="entity">The object to be added</param>
        /// <returns></returns>
        public int AddEntity(TEntity entity)
        {
            _context.AddObject(entity.GetType().Name, entity);
            return Save();
        }

        /// <summary>
        /// This method Deletes the entity object into the current Context
        /// </summary>
        /// <param name="entity">The object to be deleted</param>
        /// <returns></returns>
        public int DeleteEntity(TEntity entity)
        {
            _context.DeleteObject(entity);
            return Save();
        }

        /// <summary>
        /// This method gets the entity object from the current Context based on the entity key passed to it
        /// </summary>
        /// <param name="key">Entity Key by which the object is retrieved</param>
        /// <returns></returns>
        public TEntity SelectEntityByKey(EntityKey key)
        {
            return (TEntity)_context.GetObjectByKey(key);
        }

        /// <summary>
        /// This method just saves the changes incorporated
        /// </summary>
        /// <param name="entity">The object to be updated</param>
        /// <returns></returns>
        public int UpdateEntity(TEntity entity)
        {
            try
            {
                return Save();
            }
            catch (OptimisticConcurrencyException)
            {
                
                // Resolve the concurrency conflict by refreshing the 
                // object context before re-saving changes. 
                _context.Refresh(RefreshMode.ClientWins, entity);

                // Save changes.
                int result = Save();
                
                return result;
            }
        }

        public void AttachEntity(TEntity entity)
        {
            _context.Attach(entity);
        }

        /// <summary>
        /// This method just saves the changes incorporated
        /// </summary>
        /// <param name="entities">Collection of objects to be updated</param>
        /// <returns></returns>
        public int UpdateEntities(IList<TEntity> entities)
        {
            try
            {
                return Save();
            }
            catch (OptimisticConcurrencyException)
            {
                
                // Resolve the concurrency conflict by refreshing the 
                // object context before re-saving changes. 
                _context.Refresh(RefreshMode.ClientWins, entities);

                // Save changes.
                int result = Save();
                
                return result;
            }
        }

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(string entitySetName)
        {
            return _context.CreateQuery<TEntity>("[" + entitySetName + "]").ToList();
        }

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name and with the specified specification
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="where">Predicate</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(string entitySetName, ISpecification<TEntity> where)
        {
            return DoQuery(entitySetName, where).ToList();
        }

        /// <summary>
        /// This method gets all the objects available when the Dataset name is unknown and with the specified specification
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(ISpecification<TEntity> where)
        {
            return DoQuery(where).ToList();
        }

        /// <summary>
        /// This method gets all the objects available based on the sort expression and maximum rows and start index. This method focuses on the search criteria
        /// </summary>
        /// <param name="sortExpression">Sort Expresiion</param>
        /// <param name="maximumRows">Maximum number of records to be retrieved</param>
        /// <param name="startRowIndex">Start index of the Record</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(Expression<Func<TEntity, object>> sortExpression, int maximumRows,
                                        int startRowIndex)
        {
            if (sortExpression == null)
            {
                return DoQuery(maximumRows, startRowIndex).ToList();
            }
            return DoQuery(sortExpression, maximumRows, startRowIndex).ToList();
        }

        /// <summary>
        /// get the total number of records available in the table
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]").Count();
        }

        /// <summary>
        /// get the total number of records available in the table with the Specification applied
        /// </summary>
        /// <param name="selectSpec">Specification</param>
        /// <returns></returns>
        public int GetCount(ISpecification<TEntity> selectSpec)
        {
            if (selectSpec == null)
            {
                selectSpec = new Specification<TEntity>();
            }
            return _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]").Where(selectSpec.Predicate).Count();
        }

        /// <summary>
        /// Save the Changes into the Context
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Begin Transaction
        /// </summary>
        /// <returns>Transaction object</returns>
        public DbTransaction BeginTransaction()
        {
            if (_context.Connection.State != ConnectionState.Open)
            {
                _context.Connection.Open();
            }
            return _context.Connection.BeginTransaction();
        }

        /// <summary>
        /// Commit Transaction
        /// </summary>
        /// <param name="tran">Transaction object</param>
        public void CommitTransaction(DbTransaction tran)
        {
            if (tran != null)
            {
                tran.Commit();
            }
            if (_context.Connection.State != ConnectionState.Closed)
            {
                _context.Connection.Close();
            }
        }

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        /// <param name="tran">Transaction object</param>
        public void RollbackTransaction(DbTransaction tran)
        {
            if (tran != null)
            {
                tran.Rollback();
            }
            if (_context.Connection.State != ConnectionState.Closed)
            {
                _context.Connection.Close();
            }
        }



        /// <summary>
        /// This method adds the entity object into the current Context
        /// </summary>
        /// <param name="entities">List of Objects to be added</param>
        /// <returns></returns>
        public int AddEntity(IList<TEntity> entities)
        {
            if (entities == null)
            {
                return 0;
            }
            foreach (TEntity entity in entities)
            {
                _context.AddObject(entity.GetType().Name, entity);
            }

            return Save();
        }

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="includeParameters">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(string entitySetName, string[] includeParameters)
        {
            ObjectQuery<TEntity> query = _context.CreateQuery<TEntity>("[" + entitySetName + "]");
            if (includeParameters != null)
            {
                foreach (string innerTable in includeParameters)
                {
                    query = query.Include(innerTable);
                }
            }
            return query.ToList();
        }

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="includeParameter">The other dependant object, that has to be fetched along with the entity object</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(string entitySetName, string includeParameter)
        {
            return _context.CreateQuery<TEntity>("[" + entitySetName + "]").Include(includeParameter).ToList();
        }

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name and with the specified specification applied in Where Condition
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="where">Predicate</param>
        /// <returns></returns>
        private ObjectQuery<TEntity> DoQuery(string entitySetName, ISpecification<TEntity> where)
        {
            return (ObjectQuery<TEntity>)_context.CreateQuery<TEntity>("[" + entitySetName + "]").Where(where.Predicate);
        }

        /// <summary>
        /// This method gets all the objects available when the Dataset name is unknown and with the specified specification
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="includeParameters">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters)
        {
            return DoQuery(where, includeParameters).ToList();
        }

        public IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, NamedDictionary inParameters)
        {
            if (!(inParameters != null && inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0))
            {
                return SelectAll(where, includeParameters).ToList();
            }
            Collection<TEntity> mainCollection = DoQuery(inParameters, includeParameters, where);
            return new Collection<TEntity>(
                mainCollection.AsQueryable().ToList());

            


        }

        public IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, NamedDictionary inParameters, string sortBy,
                                        string sortOrder, int maximumRows, int startRowIndex)
        {
            IList<TEntity> mainCollection = new List<TEntity>();
            if (!(inParameters != null && inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0))
            {
                mainCollection = SelectAll(where, includeParameters).ToList();
            }
            else
            {
                mainCollection = DoQuery(inParameters, includeParameters, where).ToList();
            }
            
            return new Collection<TEntity>(
                CommonUtility.Sort(mainCollection.AsQueryable(),
                                 sortBy + " " + (sortOrder??"ASC").ToUpper(CultureInfo.CurrentCulture)).Skip(
                                startRowIndex).Take(maximumRows).ToList());
            




        }

        /// <summary>
        /// This method gets all the objects available when the Dataset name is unknown and with the specified specification
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <returns></returns>
        private ObjectQuery<TEntity> DoQuery(ISpecification<TEntity> where)
        {
            return DoQuery(where, null);
        }

        /// <summary>
        /// This method gets all the objects available based on the sort expression and maximum rows and start index. This method focuses on the search criteria
        /// </summary>
        /// <param name="sortExpression">Sort Expression</param>
        /// <param name="maximumRows">maximum number of records to be retrieved</param>
        /// <param name="startRowIndex">starting number of records</param>
        /// <returns></returns>
        private ObjectQuery<TEntity> DoQuery(Expression<Func<TEntity, object>> sortExpression, int maximumRows,
                                             int startRowIndex)
        {
            if (sortExpression == null)
            {
                return DoQuery(maximumRows, startRowIndex);
            }
            return (ObjectQuery<TEntity>)DoQuery(sortExpression).Skip(startRowIndex).Take(maximumRows);
        }

        /// <summary>
        /// This method gets all the objects available based on the sort expression and maximum rows and start index. This method focuses on the search criteria
        /// </summary>
        /// <param name="maximumRows">Maximum number of records to be retrieved</param>
        /// <param name="startRowIndex">Start index of the record</param>
        /// <returns></returns>
        private ObjectQuery<TEntity> DoQuery(int maximumRows, int startRowIndex)
        {
            return
                (ObjectQuery<TEntity>)
                _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]").Skip(startRowIndex).Take(maximumRows);
        }
        /// <summary>
        /// This method gets all the records based on the sort expression
        /// </summary>
        /// <param name="sortExpression">Sort Expression</param>
        /// <returns>Collection of Objects</returns>
        private ObjectQuery<TEntity> DoQuery(Expression<Func<TEntity, object>> sortExpression)
        {
            if (null == sortExpression)
            {
                return DoQuery();
            }
            return (ObjectQuery<TEntity>)DoQuery().OrderBy(sortExpression);
        }

        /// <summary>
        /// This method gets all the records that matches the criteria and also gets the dependant objects along with the result
        /// </summary>
        /// <param name="where">Presdicate</param>
        /// <param name="includeParams">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <returns>Collection of Objects</returns>
        private ObjectQuery<TEntity> DoQuery(ISpecification<TEntity> where, string[] includeParams)

        {
            ObjectQuery<TEntity> query = _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
            if (includeParams != null)
            {
                for (int paramIndex = 0; paramIndex < includeParams.Length; paramIndex++)
                {
                    query = query.Include(includeParams[paramIndex]);
                }
            }
            if (where != null && where.Predicate != null)
            {
                return (ObjectQuery<TEntity>)query.Where(where.Predicate);
            }
            return query;

        }
        /// <summary>
        /// This method gets all the records that matches search Criteria, sort expression(string) and with paging criteria
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="includeParameters">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <param name="sortBy">The propert name on which the sorting should occur</param>
        /// <param name="sortOrder">The order of sorting</param>
        /// <param name="maximumRows">Maximum number of rows that should be returned</param>
        /// <param name="startRowIndex">Start index of the record</param>
        /// <returns></returns>
        public IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, string sortBy,
                                        string sortOrder, int maximumRows, int startRowIndex)
        {
            ObjectQuery<TEntity> query = DoQuery(includeParameters);
            sortOrder = (sortOrder ?? "ASC");
            try
            {
                if (maximumRows > 0)
                {
                    where = where ?? new Specification<TEntity>();
                        return
                            CommonUtility.Sort(where.Predicate != null ? query.Where(where.Predicate) : query,
                                 sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).Skip(
                                startRowIndex).Take(maximumRows).ToList();
                    
                }
            }
            
            catch (EntityCommandExecutionException tooManyTableNamesException)
            {
                if (tooManyTableNamesException.InnerException != null && tooManyTableNamesException.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException) && ((System.Data.SqlClient.SqlException)(tooManyTableNamesException.InnerException)).Number == 106)
                {
                    throw ; 
                }
               
            }
             return CommonUtility.Sort((where != null && where.Predicate != null) ? query.Where(where.Predicate) : query, sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).ToList();
            
        }

        private ObjectQuery<TEntity> DoQuery(string[] includeParameters)
        {
            ObjectQuery<TEntity> query = _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
            if (includeParameters != null)
            {
                for (int paramIndex = 0; paramIndex < includeParameters.Length; paramIndex++)
                {
                    query = query.Include(includeParameters[paramIndex]);
                }
            }
            return query;
        }

        public IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, string sortBy,
                                        string sortOrder, int maximumRows, int startRowIndex, NamedDictionary inParameters)
        {

            sortOrder = (sortOrder ?? "ASC");
            if (inParameters != null && inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0)
            {
                Collection<TEntity> mainCollection = DoQuery(inParameters, includeParameters, where);
                return new Collection<TEntity>(
                    CommonUtility.Sort(mainCollection.AsQueryable(),
                                         sortBy + " " +
                                         (sortOrder).ToUpper(
                                             CultureInfo.CurrentCulture)).ToList()).Skip(
                    startRowIndex).Take(maximumRows).ToList();
            }


            ObjectQuery<TEntity> query = _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
            query = DoQuery(includeParameters, query, inParameters);
            if (maximumRows > 0 && where != null)
            {
                
                if (where.Predicate != null)
                {
                    return
                        CommonUtility.Sort(query.Where(where.Predicate),
                             sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).Skip(
                            startRowIndex).Take(maximumRows).ToList();
                }
                if (where.Predicate == null)
                {
                    return
                    CommonUtility.Sort(query, sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).Skip(startRowIndex).Take(maximumRows)
                        .ToList();
                }

            }
            if (where != null && where.Predicate != null)
            {
                return CommonUtility.Sort(query.Where(where.Predicate), sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).ToList();
            }
            return CommonUtility.Sort(query, sortBy + " " + sortOrder.ToUpper(CultureInfo.CurrentCulture)).ToList();
        }

        private Collection<TEntity> DoQuery(NamedDictionary inParameters, string[] includeParameters, ISpecification<TEntity> where)
        {
            var mainCollection = new Collection<TEntity>();
            ObjectQuery<TEntity> subQuery = _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
            

            NamedDictionary result;
            subQuery = DoQuery(includeParameters,subQuery, inParameters);
            bool firstSearch = true;
            foreach (var pair in inParameters)
            {
                var pairResultCollection = new Collection<TEntity>();
                for (int k = 0; k < pair.Value.Count; k = k + 100)
                {
                    var output = new StringCollection();
                    for (int i = k; i < k + 100 && i < pair.Value.Count; i++)
                    {
                        var ids = new Collection<string>();
                        ids.Add(pair.Value[i]);
                        output.AddRange(ids);

                    }
                    result = new NamedDictionary();
                    result.Add(pair.Key, output);
                    subQuery = DoQuery(includeParameters, subQuery, result);
                    IList<TEntity> subCollection;
                    if (where != null && where.Predicate != null)
                    {
                        subCollection = subQuery.Where(where.Predicate.Compile()).ToList();
                    }
                    else
                    {
                        subCollection = subQuery.ToList();
                    }
                    pairResultCollection.AddRange(subCollection);
                }
                if (firstSearch)
                {
                    mainCollection.AddRange(pairResultCollection);
                    firstSearch = false;
                }
                else
                {
                    var sqlResult = from first in mainCollection
                                    from second in pairResultCollection
                                    where first.Equals(second)
                                    select first;
                    mainCollection = new Collection<TEntity>();
                    mainCollection.AddRange(sqlResult.ToList());

                }
            }
            return mainCollection;
        }

        private ObjectQuery<TEntity> DoQuery(string[] includeParameters, ObjectQuery<TEntity> query, NamedDictionary inParameters)
        {
            if (inParameters != null && inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0)
            {
                query = DoQuery(query, inParameters);
            }
            if (includeParameters != null)
            {
                for (int paramIndex = 0; paramIndex < includeParameters.Length; paramIndex++)
                {
                    query = query.Include(includeParameters[paramIndex]);
                }
            }

            return query;
        }

        private ObjectQuery<TEntity> DoQuery(ObjectQuery<TEntity> query, NamedDictionary inParameters)
        {
            string sql = @" Select value obj from " + typeof(TEntity).Name + " as obj where";
            string columnName = null;
            foreach (var entity in inParameters)
            {
                if ((entity.Value).Count == 0)
                {
                    continue;
                }
                sql = GetSql(entity, columnName, sql);
            }
            
            if (inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0)
            {
                query = _context.CreateQuery<TEntity>(sql);
            }
            
            return query;
        }

        private static string GetSql(KeyValuePair<string, StringCollection> entity, string columnName, string sql)
        {
            String[] orderByChildProperties = entity.Key.Split('.');
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty(orderByChildProperties[0]);
            for (int i = 1; i < orderByChildProperties.Length; i++)
            {
                Type t = propertyInfo.PropertyType;
                if (!t.IsGenericType)
                {
                    propertyInfo = t.GetProperty(orderByChildProperties[i]);
                }
                else
                {
                    propertyInfo = t.GetGenericArguments().First().GetProperty(orderByChildProperties[i]);
                }
            }
            columnName = columnName == null ? "obj." + entity.Key : (entity.Key.IndexOf("and", StringComparison.OrdinalIgnoreCase) != -1 || entity.Key.IndexOf("or", StringComparison.OrdinalIgnoreCase) != -1 ? entity.Key : "and obj." + entity.Key);
            sql = GetSql(entity, propertyInfo, columnName, sql);
            return sql;
        }

        private static string GetSql(KeyValuePair<string, StringCollection> entity, PropertyInfo propertyInfo, string columnName, string sql)
        {
            if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
            {

                sql += " (" + columnName + " in {" +
                       String.Join(",",
                                   ((entity.Value)).Select(c => "GUID\'" + c + "\'").
                                       ToArray()) + "} or " + columnName + " is null )";

            }
            else if (propertyInfo.PropertyType == typeof(string))
            {

                sql += " " + columnName + " in {" + String.Join(",", ((entity.Value)).Select(c => "'" + c + "'").ToArray()) + "} ";
            }
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
            {

                sql += " " + columnName + " in {" + String.Join(",", ((entity.Value)).Select(c => c).ToArray()) + "} ";
            }
            return sql;
        }

        /// <summary>
        /// To get the total number of records
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <returns></returns>
        public int SelectTotalRecordCount(ISpecification<TEntity> where)
        {
            ObjectQuery<TEntity> query = _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
            if (where != null && where.Predicate != null)
            {
                return query.Where(where.Predicate).ToList().Count;
            }
            return query.ToList().Count;
        }

        /// <summary>
        /// To get the total number of records
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="inParameters">Input parameters</param>
        /// <returns>The count of records</returns>
        public int SelectTotalRecordCount(ISpecification<TEntity> where, NamedDictionary inParameters)
        {
            Collection<TEntity> result;
            if (inParameters != null && inParameters.Count > 0 && inParameters[inParameters.First().Key].Count > 0)
            {
                result = DoQuery(inParameters, null, where);
            }
            else
            {
                return DoQuery(where).Count();
            }

            return result.Count;

        }

        /// <summary>
        /// Create a query to get  the records
        /// </summary>
        /// <returns>Query</returns>
        private ObjectQuery<TEntity> DoQuery()
        {
            return _context.CreateQuery<TEntity>("[" + typeof(TEntity).Name + "]");
        }

        

        
    }
    public sealed class Repository<TContext> : IRepository
         where TContext : ObjectContext
    {
        // create a private Context variable to hold the current data access context
        private TContext _context;
        private string _connectionString;

        private Repository()
        {

        }

        public Repository(string connectionString)
        {
            Type type = typeof(TContext);
            _connectionString = connectionString;
            _context = (TContext)Activator.CreateInstance(type, new[] { connectionString });
            this._context.CommandTimeout = 3600;
        }

        /// <summary>
        /// Public constructor which creates an instance and holds the Context object
        /// </summary>
        /// <param name="context">Actual Object Context for the Repository</param>
        public Repository(TContext context)
        {
            //context.CommandTimeout = 600;
            this._context = context;
            this._context.CommandTimeout = 3600;
        }


        /// <summary>
        /// Define Dispose method of IDisposable Interface
        /// </summary>
        public void Dispose()
        {
            if (null != _context)
            {
                _context.Connection.Dispose();
                _context.Dispose();
            }
        }

        public void Open()
        {
            //if(!_context.DatabaseExists())
            //{
            //    Type type = typeof(TContext);
            //    _context = (TContext)Activator.CreateInstance(type, new[] { _connectionString });
            //}
            if (_context.Connection.State != ConnectionState.Open)
            {
                _context.Connection.Open();
                
                
            }
        }

        public void ExecuteFunction<TType>(String functionName, IDictionary<string, object> parameters,out IList<TType> output)
        {
            List<ObjectParameter> parameterCollection = null;
            if (parameters != null)
            {
                parameterCollection = new List<ObjectParameter>();
                foreach (var parameter in parameters)
                {
                    if (parameter.Value as Type == null)
                    {
                        parameterCollection.Add(new ObjectParameter(parameter.Key, parameter.Value));
                    }
                    else
                    {
                        parameterCollection.Add(new ObjectParameter(parameter.Key, (Type)parameter.Value));
                    }
                    
                }
            }
            if (parameterCollection != null)
            {
                output =  _context.ExecuteFunction<TType>(functionName, parameterCollection.ToArray()).ToList();
            }
            else
            {
                output = _context.ExecuteFunction<TType>(functionName).ToList();
            }         


        }

        public void Close()
        {
            if (_context.Connection.State == ConnectionState.Open)
            {
                _context.Connection.Close();
            }
            
        }


    }
}