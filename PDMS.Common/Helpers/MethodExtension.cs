/* ---------------------------------------------------------------------    
 * Copyright:
 * Jabil
 * 
 * Class Description:
 * 模型擴展方法
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2014-11-26  shuwei_miao@Jabil.com
 *
 * ------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data;
using System.Configuration;

namespace PDMS.Common.Helpers
{
    public static class MethodExtension
    {
        public delegate void PropertySetter<T>(T value);
        public delegate T PropertyGetter<T>();

        #region 基础方法
        /// <summary>
        /// 基础方法
        /// </summary>
        /// <typeparam name="Tfrom"></typeparam>
        /// <typeparam name="Tto"></typeparam>
        /// <param name="fromEntity"></param>
        /// <param name="toEntity"></param>
        /// <param name="parms"></param>
        private static void CopyEntityHelper<Tfrom, Tto>(this Tfrom fromEntity, Tto toEntity, params string[] parms)
        {
            foreach (var propertyName in parms)
            {
                Type fromType = fromEntity.GetType();
                Type toType = toEntity.GetType();

                PropertyInfo propertyInfoFrom = fromType.GetProperty(propertyName);
                MethodInfo getterFrom = propertyInfoFrom.GetGetMethod();
                PropertyInfo propertyInfoTo = toType.GetProperty(propertyName);
                MethodInfo setterTo = propertyInfoTo.GetSetMethod();
                var isReadOnly = propertyInfoTo.GetCustomAttributes(true).Count(item => (item is ReadOnlyAttribute)
                   && (item as ReadOnlyAttribute).IsReadOnly) > 0;
                if (getterFrom != null && setterTo != null && propertyInfoFrom.CanRead && propertyInfoTo.CanWrite && !isReadOnly)
                {
                    var type = propertyInfoFrom.PropertyType;
                    //Int
                    if (type.Equals(typeof(Int32)))
                    {
                        var PropGet = (PropertyGetter<int>)Delegate.CreateDelegate(typeof(PropertyGetter<int>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<int>)Delegate.CreateDelegate(typeof(PropertySetter<int>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    else if (type.Equals(typeof(Int32?)))
                    {
                        var PropGet = (PropertyGetter<int?>)Delegate.CreateDelegate(typeof(PropertyGetter<int?>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<int?>)Delegate.CreateDelegate(typeof(PropertySetter<int?>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    //Decimal
                    else if (type.Equals(typeof(decimal)))
                    {
                        var PropGet = (PropertyGetter<decimal>)Delegate.CreateDelegate(typeof(PropertyGetter<decimal>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<decimal>)Delegate.CreateDelegate(typeof(PropertySetter<decimal>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    else if (type.Equals(typeof(decimal?)))
                    {
                        var PropGet = (PropertyGetter<decimal?>)Delegate.CreateDelegate(typeof(PropertyGetter<decimal?>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<decimal?>)Delegate.CreateDelegate(typeof(PropertySetter<decimal?>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    //Bool
                    else if (type.Equals(typeof(bool)))
                    {
                        var PropGet = (PropertyGetter<bool>)Delegate.CreateDelegate(typeof(PropertyGetter<bool>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<bool>)Delegate.CreateDelegate(typeof(PropertySetter<bool>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    else if (type.Equals(typeof(bool?)))
                    {
                        var PropGet = (PropertyGetter<bool?>)Delegate.CreateDelegate(typeof(PropertyGetter<bool?>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<bool?>)Delegate.CreateDelegate(typeof(PropertySetter<bool?>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    //String
                    else if (type.Equals(typeof(String)))
                    {
                        var PropGet = (PropertyGetter<string>)Delegate.CreateDelegate(typeof(PropertyGetter<string>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<string>)Delegate.CreateDelegate(typeof(PropertySetter<string>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    //DateTime
                    else if (type.Equals(typeof(DateTime)))
                    {
                        var PropGet = (PropertyGetter<DateTime>)Delegate.CreateDelegate(typeof(PropertyGetter<DateTime>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<DateTime>)Delegate.CreateDelegate(typeof(PropertySetter<DateTime>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    else if (type.Equals(typeof(DateTime?)))
                    {
                        var PropGet = (PropertyGetter<DateTime?>)Delegate.CreateDelegate(typeof(PropertyGetter<DateTime?>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<DateTime?>)Delegate.CreateDelegate(typeof(PropertySetter<DateTime?>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    //byte[]
                    else if (type.Equals(typeof(byte[])))
                    {
                        var PropGet = (PropertyGetter<byte[]>)Delegate.CreateDelegate(typeof(PropertyGetter<byte[]>), fromEntity, getterFrom);
                        var PropSet = (PropertySetter<byte[]>)Delegate.CreateDelegate(typeof(PropertySetter<byte[]>), toEntity, setterTo);
                        PropSet(PropGet());
                    }
                    else
                    {
                        //无法识别的属性，不能使用泛型委托
                        //propertyInfoTo.SetValue(toEntity, propertyInfoFrom.GetValue(fromEntity, null), null);
                    }
                    ////object
                    //else
                    //{
                    //    var PropGet = (PropertyGetter<object>)Delegate.CreateDelegate(typeof(PropertyGetter<object>), fromEntity, getterFrom);
                    //    var PropSet = (PropertySetter<object>)Delegate.CreateDelegate(typeof(PropertySetter<object>), toEntity, setterTo);
                    //    PropSet(PropGet());
                    //}
                }
            }
        }
        #endregion

        #region 复制整个实体
        /// <summary>
        /// 复制整个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromEntity"></param>
        /// <returns></returns>
        public static void CopyEntity<T>(this T fromEntity, T toEntity) where T : new()
        {

            var copyProperties = (from Property in fromEntity.GetType().GetProperties()
                                  where Property.CanRead && Property.CanWrite
                                  select Property.Name
                                      ).ToArray<string>();
            fromEntity.CopyEntityHelper<T, T>(toEntity, copyProperties);

        }
        #endregion

        #region 动态Linq方式实现行转列
        /// <summary>
        /// 动态Linq方式实现行转列
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="DimensionList">维度列</param>
        /// <param name="DynamicColumn">动态列</param>
        /// <returns>行转列后数据</returns>
        public static List<dynamic> DynamicLinq<T>(List<T> list, List<string> DimensionList, string DynamicColumn, out List<string> AllDynamicColumn) where T : class
        {
            //获取所有动态列
            var columnGroup = list.GroupBy(DynamicColumn, "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
            List<string> AllColumnList = new List<string>();
            foreach (var item in columnGroup)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    AllColumnList.Add(item.Key);
                }
            }
            AllDynamicColumn = AllColumnList;
            var dictFunc = new Dictionary<string, Func<T, bool>>();
            foreach (var column in AllColumnList)
            {
                var func = DynamicExpression.ParseLambda<T, bool>(string.Format("{0}==\"{1}\"", DynamicColumn, column)).Compile();
                dictFunc[column] = func;
            }

            //获取实体所有属性
            Dictionary<string, PropertyInfo> PropertyInfoDict = new Dictionary<string, PropertyInfo>();
            Type type = typeof(T);
            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            //数值列
            List<string> AllNumberField = new List<string>();
            foreach (var item in propertyInfos)
            {
                PropertyInfoDict[item.Name] = item;
                if (item.PropertyType == typeof(int) || item.PropertyType == typeof(double) || item.PropertyType == typeof(float))
                {
                    AllNumberField.Add(item.Name);
                }
            }

            //分组
            var dataGroup = list.GroupBy(string.Format("new ({0})", string.Join(",", DimensionList)), "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
            List<dynamic> listResult = new List<dynamic>();
            IDictionary<string, object> itemObj = null;
            T vm2 = default(T);
            foreach (var group in dataGroup)
            {
                itemObj = new ExpandoObject();
                var listVm = group.Select(e => e.Vm as T).ToList();
                //维度列赋值
                vm2 = listVm.FirstOrDefault();
                foreach (var key in DimensionList)
                {
                    itemObj[key] = PropertyInfoDict[key].GetValue(vm2);
                }

                foreach (var column in AllColumnList)
                {
                    vm2 = listVm.FirstOrDefault(dictFunc[column]);
                    if (vm2 != null)
                    {
                        foreach (string name in AllNumberField)
                        {
                            itemObj[name + column] = PropertyInfoDict[name].GetValue(vm2);
                        }
                    }
                }
                listResult.Add(itemObj);
            }
            return listResult;
        }

        #endregion

        public static string GetConnectionStr()
        {
            var replaceStartStr = "data source";
            var replaceEndStr = "MultipleActiveResultSets";
            var conStr = ConfigurationManager.ConnectionStrings["SPPContext"].ConnectionString;
            var startIndex = conStr.IndexOf(replaceStartStr);
            var endIndex = conStr.IndexOf(replaceEndStr);
            conStr = conStr.Substring(startIndex, endIndex - startIndex);
            conStr = string.Format("{0}Persist Security Info=true;Connection Timeout=300", conStr);
            return conStr;

        }

    }
}
