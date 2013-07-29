using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Linq.Expressions;

namespace iConnect.Common
{
    public interface IRepository<TEntity> : IDisposable 
    {

        /// <summary>
        /// This method adds the entity object into the current Context
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>Added count of entities</returns>
        int AddEntity(TEntity entity);

        /// <summary>
        /// This method Deletes the entity object into the current Context
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        /// <returns>The number of entities deleted</returns>
        int DeleteEntity(TEntity entity);

        /// <summary>
        /// This method gets the entity object from the current Context based on the entity key passed to it
        /// </summary>
        /// <param name="key">The key entity to select the entity</param>
        /// <returns>The selected entity</returns>
        TEntity SelectEntityByKey(EntityKey key);

        /// <summary>
        /// This method just saves the changes incorporated
        /// </summary>
        /// <param name="entity">The entity to be saved</param>
        /// <returns>The number of entitites updated</returns>
        int UpdateEntity(TEntity entity);

        /// <summary>
        /// This method just saves the changes incorporated
        /// </summary>
        /// <param name="entity">The entity to be saved</param>
        /// <returns>The number of entitites updated</returns>
        void AttachEntity(TEntity entity);

        /// <summary>
        /// This method just saves the changes incorporated
        /// </summary>
        /// <param name="entities">The entities to be updated</param>
        /// <returns>The number of entities updated</returns>
        int UpdateEntities(IList<TEntity> entities);


        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">The entity data set</param>
        /// <returns>The list of entities returned</returns>
        IList<TEntity> SelectAll(string entitySetName);

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name and with the specified specification
        /// </summary>
        /// <param name="entitySetName">The entity data set</param>
        /// <param name="where">The specfication to be used to select objects</param>
        /// <returns>The selected objects</returns>
        IList<TEntity> SelectAll(string entitySetName, ISpecification<TEntity> where);

        /// <summary>
        /// This method gets all the objects available when the Dataset name is unknown and with the specified specification
        /// </summary>
        /// <param name="where">The specification to select entities</param>
        /// <returns>The selected entities</returns>
        IList<TEntity> SelectAll(ISpecification<TEntity> where);

        /// <summary>
        /// This method gets all the objects available based on the sort expression and maximum rows and start index. This method focuses on the search criteria
        /// </summary>
        /// <param name="sortExpression">The expression to be used to sort the returned elements</param>
        /// <param name="maximumRows">The maximum number of rows to be returned</param>
        /// <param name="startRowIndex">The start index of the selection</param>
        /// <returns>The elements selected</returns>
        IList<TEntity> SelectAll(Expression<Func<TEntity, object>> sortExpression,
                                 int maximumRows, int startRowIndex);

        /// <summary>
        /// get the total number of records available in the table
        /// </summary>
        /// <returns>The count of elements</returns>
        int GetCount();

        /// <summary>
        /// get the total number of records available in the table with the Specification applied
        /// </summary>
        /// <param name="selectSpec">The specification to get the count</param>
        /// <returns>The count of records</returns>
        int GetCount(ISpecification<TEntity> selectSpec);

        /// <summary>
        /// Save the Changes into the Context
        /// </summary>
        /// <returns>The number of records affected</returns>
        int Save();

        /// <summary>
        /// Start Transaction
        /// </summary>
        /// <returns>The transaction object that is created</returns>
        DbTransaction BeginTransaction();

        /// <summary>
        /// Commit Transaction
        /// </summary>
        ///<param name="tran">The transaction object</param>
        void CommitTransaction(DbTransaction tran);

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        ///<param name="tran">The transaction object</param>
        void RollbackTransaction(DbTransaction tran);

        /// <summary>
        /// This method adds the entity object into the current Context
        /// </summary>
        /// <param name="entities">List of Objects to be added</param>
        /// <returns>The number of records affected</returns>
        int AddEntity(IList<TEntity> entities);

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="includeParameters">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <returns>The list of entities selected</returns>
        IList<TEntity> SelectAll(string entitySetName, string[] includeParameters);

        /// <summary>
        /// This method gets all the objects available in the EntityDataset Name
        /// </summary>
        /// <param name="entitySetName">Unique dataset name of the object</param>
        /// <param name="includeParameter">The other dependant object, that has to be fetched along with the entity object</param>
        /// <returns>The list of entities selected</returns>
        IList<TEntity> SelectAll(string entitySetName, string includeParameter);

        /// <summary>
        /// This method gets all the objects available when the Dataset name is unknown and with the specified specification
        /// </summary>
        /// <param name="where">Predicate</param>
        /// <param name="includeParameters">The other dependant objects, that has to be fetched along with the entity object</param>
        /// <returns>The list of entities selected</returns>
        IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters);

        /// <summary>
        /// Select all objects that meet the given specification, along with other dependent objects
        /// </summary>
        /// <param name="where">The predicate to select items</param>
        /// <param name="includeParameters">Other dependent objects to be selected</param>
        /// <param name="inParameters">Inward parameters</param>
        /// <returns>The selected objects</returns>
        IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters,
                                        NamedDictionary inParameters);

        IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters,
                                 NamedDictionary inParameters, string sortBy,
                                 string sortOrder, int maximumRows, int startRowIndex);

        IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, string sortBy,
                                 string sortOrder, int maximumRows, int startRowIndex);

        IList<TEntity> SelectAll(ISpecification<TEntity> where, string[] includeParameters, string sortBy,
                                 string sortOrder, int maximumRows, int startRowIndex,
                                 NamedDictionary inParameters);

        int SelectTotalRecordCount(ISpecification<TEntity> where);

        int SelectTotalRecordCount(ISpecification<TEntity> where, NamedDictionary inParameters);

        
    }
    public interface IRepository : IDisposable
    {
        void Open();
        void ExecuteFunction<TType>(String functionName, IDictionary<string, object> parameters, out IList<TType> output);
        void Close();
    }
}