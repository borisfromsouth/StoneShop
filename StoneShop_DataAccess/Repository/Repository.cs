using Microsoft.EntityFrameworkCore;
using StoneShop_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StoneShop_DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dataBase;
        internal DbSet<T> dataBaseSet; 

        public Repository(ApplicationDbContext dataBase)
        {
            _dataBase = dataBase;
            this.dataBaseSet = _dataBase.Set<T>();
        }

        public void Add(T entity)
        {
            this.dataBaseSet.Add(entity);
        }

        public T Find(int id)
        {
            return this.dataBaseSet.Find(id);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = dataBaseSet;  // запрос
            if (filter != null)
            {
                query = query.Where(filter);  // добавляем filter к запросу (в какой таблице искать)
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))  // свойства представлены строками, разделенными запятыми
                {
                    query = query.Include(includeProp);  // добавляем свойства к запросу
                }
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();  // запрос не будет отслеживаться (запрос выполняется единожды; ускорение работы приложения) 
            }
            return query.FirstOrDefault();  // возвращаем результаты запроса
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, bool isTracking = true)
        {
            IQueryable<T> query = dataBaseSet;  // результаты запроса в БД по таблице типа T
            if (filter != null)
            {
                query = query.Where(filter);  // добавляем filter к запросу
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))  // свойства представлены строками, разделенными запятыми
                {
                    query = query.Include(includeProp);  // добавляем свойства к запросу
                }
            }
            if (orderBy != null)
            {
                query = orderBy(query);  // сортировка по порядку
            }
            if (!isTracking)
            {
                query = query.AsNoTracking();  // запрос не будет отслеживаться (запрос выполняется единожды; ускорение работы приложения) 
            }
            return query.ToList();  // возвращаем результаты запроса
        }

        public void Remove(T entity)
        {
            this.dataBaseSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            this.dataBaseSet.RemoveRange(entity);
        }

        public void Save()
        {
            _dataBase.SaveChanges();
        }
    }
}
