using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HotSaleSenfoniAppServer
{
    public class MethodResult<T>
    {
        public MethodResult()
        {
            if (typeof(T).IsPrimitive || typeof(T).Equals(typeof(string)) || typeof(T).IsValueType)
            {
                this.Values = default(T);
            }
            else
            {
                this.Values = (T)Activator.CreateInstance(typeof(T));
            }
            this.Message = string.Empty;
            this.Success = false;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Values { get; set; }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Include,
                    DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
                });
        }

        public void SetData(DataTable table)
        {
            try
            {
                if (table != null && table.Rows.Count > 0)
                {
                    Type type = typeof(T).GetGenericArguments()[0];
                    bool simple = IsSimpleType(type);

                    Type list = typeof(List<>).MakeGenericType(type);
                    var newCollection = (System.Collections.IList)Activator.CreateInstance(list);

                    PropertyInfo[] properties = type.GetProperties();

                    for (int loop = 0; loop < table.Rows.Count; loop++)
                    {
                        object obj = null;
                        if (!simple)
                        {
                            obj = Activator.CreateInstance(type);
                        }
                        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                        {
                            if (table.Rows[loop][columnIndex] != null && table.Rows[loop][columnIndex] != DBNull.Value)
                            {
                                PropertyInfo property = properties.Where(q => q.Name.Equals(table.Columns[columnIndex].Caption, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (!object.ReferenceEquals(property, null))
                                {
                                    try
                                    {
                                        if (simple)
                                        {
                                            obj = Convert.ChangeType(table.Rows[loop][columnIndex], type);
                                        }
                                        else
                                        {
                                            property.SetValue(obj, table.Rows[loop][columnIndex]);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        newCollection.Add(obj);
                    }
                    this.Values = (T)newCollection;
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(string.Concat("List doldurulamadı! Message:", exc.Message, ",StackTrace:", exc.StackTrace));
            }

        }

        private bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
                }.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
    }
}