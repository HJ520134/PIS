using PDMS.Data;
using PDMS.Data.Infrastructure;
using PDMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IBaseSevice<T, TDto, TSearch> where T : class where TDto : class where TSearch : class
    {
        IList<T> Query(QueryModel<TSearch> queryModel);
        IList<TDto> QueryDto(QueryModel<TSearch> queryModel);
          
    }
    public abstract class BaseSevice<T, TDto, TSearch> where T : class where TDto : class where TSearch : class
    {
        private readonly IRepository<T> repository;
        public BaseSevice(IRepository<T> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// 查询,返回DTO类型
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public virtual IList<TDto> QueryDto(QueryModel<TSearch> queryModel)
        {
            var resultList = Query(queryModel);
            var dtoResultList = AutoMapper.Mapper.Map<List<TDto>>(resultList);
            return dtoResultList;
        }

        /// <summary>
        /// 查询,返回实体类型
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public virtual IList<T> Query(QueryModel<TSearch> queryModel)
        {
            //Expression<Func<T, bool>> expr = null;
            //ParameterExpression c = Expression.Parameter(typeof(T), "c");
            //Expression condition = Expression.Constant(false);
            var body = default(Expression);
            var parameter = Expression.Parameter(typeof(T), "c");

            //等于
            if (queryModel.Equal != null)
            {
                var model = queryModel.Equal;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    //var x = property.PropertyType.GetGenericTypeDefinition();
                    //bool isNullable = property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                    if (isValueType || property.PropertyType == typeof(string))
                    {
                        //若是基础类型或string
                        var propertyVlaue = property.GetValue(model, null);

                        //defaultValue 为当前类型的默认值
                        //var defaultValue = Activator.CreateInstance(property.PropertyType, null);
                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            //ConstantExpression constantReference = null;
                            //if (isGenericType)
                            //{
                            //    constantReference = Expression.Constant(Convert.ChangeType(propertyVlaue, Nullable.GetUnderlyingType(property.PropertyType)), property.PropertyType);
                            //}
                            //else
                            //{
                            //    constantReference = Expression.Constant(Convert.ChangeType(propertyVlaue, property.PropertyType));
                            //}
                            //string

                            //var stringValue = propertyVlaue as string;
                            //propertyCondition = Expression.Call(Expression.Property(parameter, property.Name), typeof(string).GetMethod("Contains"), Expression.Constant(value));
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);//
                            propertyCondition = Expression.Equal(propertyReference, constantReference);
                            //Expression.Property(parameter,property.Name).Call("Contains", Expression.Constant(stringValue));
                            //ConstantExpression stringExpression = Expression.Constant(stringValue,typeof(string));
                            //propertyCondition = Expression.Call(Expression.Property(parameter, property.Name), typeof(string).GetMethod("Contains"), Expression.Constant(value));

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //不等于
            if (queryModel.NotEqual != null)
            {
                var model = queryModel.NotEqual;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    if (isValueType || property.PropertyType == typeof(string))
                    {
                        var propertyVlaue = property.GetValue(model, null);

                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);
                            propertyCondition = Expression.NotEqual(propertyReference, constantReference);

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //大于
            if (queryModel.GreaterThan != null)
            {
                var model = queryModel.GreaterThan;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    if (isValueType && property.PropertyType != typeof(bool))
                    {
                        var propertyVlaue = property.GetValue(model, null);

                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);
                            propertyCondition = Expression.GreaterThan(propertyReference, constantReference);

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //小于
            if (queryModel.LessThan != null)
            {
                var model = queryModel.LessThan;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    if (isValueType && property.PropertyType != typeof(bool))
                    {
                        var propertyVlaue = property.GetValue(model, null);

                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);
                            propertyCondition = Expression.LessThan(propertyReference, constantReference);

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //大于等于
            if (queryModel.GreaterThanOrEqual != null)
            {
                var model = queryModel.GreaterThanOrEqual;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    if (isValueType && property.PropertyType != typeof(bool))
                    {
                        var propertyVlaue = property.GetValue(model, null);

                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);
                            propertyCondition = Expression.GreaterThanOrEqual(propertyReference, constantReference);

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //小于等于
            if (queryModel.LessThanOrEqual != null)
            {
                var model = queryModel.LessThanOrEqual;
                var propertys = model.GetType().GetProperties();
                //遍历属性
                foreach (var property in propertys)
                {
                    //是否为基础类型
                    bool isValueType = property.PropertyType.IsValueType;
                    if (isValueType && property.PropertyType != typeof(bool))
                    {
                        var propertyVlaue = property.GetValue(model, null);

                        if (propertyVlaue != null)
                        {
                            Expression propertyCondition = null;
                            var propertyReference = Expression.Property(parameter, property.Name);
                            ConstantExpression constantReference = Expression.Constant(propertyVlaue, propertyReference.Type);
                            propertyCondition = Expression.LessThanOrEqual(propertyReference, constantReference);

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            //like仅对string类型过滤
            if (queryModel.Like != null)
            {
                var model = queryModel.Like;
                var propertys = model.GetType().GetProperties();
                foreach (var property in propertys)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var value = property.GetValue(model, null);
                        if (value != null)
                        {
                            Expression propertyCondition = null;
                            //string
                            if (property.PropertyType == typeof(string) && !string.IsNullOrEmpty(value as string))
                            {
                                var stringValue = value as string;
                                propertyCondition = Expression.Call(Expression.Property(parameter, property.Name), typeof(string).GetMethod("Contains"), Expression.Constant(value));
                            }

                            if (propertyCondition != null)
                            {
                                body = body != null ? Expression.AndAlso(body, propertyCondition) : propertyCondition;
                            }
                        }
                    }
                    else
                    {
                        //若为复杂类型
                    }
                }
            }

            if (body == null)
            {
                body = Expression.Constant(true);
            }
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            var resultList = repository.GetMany(lambda).ToList();
            return resultList;
        }
     
    }
}
