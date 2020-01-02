using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Alonso.Utility
{
    /// <summary>
    /// WhereClausesBuilder(轉換sql where語法)
    /// </summary>
    public class WhereClausesBuilder
    {
        /// <summary>
        /// 轉換where條件 by Nvarchar類型
        /// </summary>
        /// <param name="parameterName">parameter參數名稱</param>
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where 條件</param>
        /// <param name="where">ref sql where字串</param>
        /// <param name="SqlParameters">ref SqlParameters</param>
        public void appendCriteria(string parameterName, string value, string criteria,
            ref string where, ref List<SqlParameter> SqlParameters)
        {
            appendCriteria(parameterName, value, criteria,
                            ref where, ref SqlParameters, SqlDbType.NVarChar);
        }


        /// <summary>
        /// 轉換where條件 by Nvarchar類型
        /// </summary>
        /// <param name="parameterName">parameter參數名稱</param>
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where 條件</param>
        /// <param name="where">ref sql where字串</param>
        /// <param name="SqlParameters">ref SqlParameters</param>
        public void appendCriteriaOR(string parameterName, string value, string criteria,
            ref string where, ref List<SqlParameter> SqlParameters)
        {
            appendCriteriaOR(parameterName, value, criteria,
                            ref where, ref SqlParameters, SqlDbType.NVarChar);
        }

        /// <summary>
        /// 轉換where條件
        /// </summary>
        /// <param name="parameterName">parameter參數名稱</param>
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where條件</param>
        /// <param name="where">ref sql where字串</param>
        /// <param name="SqlParameters">ref SqlParameters </param>
        /// <param name="dbType">資料欄位型別</param>
        public void appendCriteria(string parameterName, string value, string criteria,
            ref string where, ref List<SqlParameter> SqlParameters, SqlDbType dbType)
        {
            if (string.IsNullOrEmpty(value)) return;

            string cond = string.Format(criteria, "@" + parameterName);
            where += string.Concat(" AND ", cond);

            var p = new SqlParameter(parameterName, dbType);
            p.Value = value;
            SqlParameters.Add(p);
        }

        /// <summary>
        /// 轉換where條件 存文字
        /// </summary>        
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where條件</param>
        /// <param name="where">ref sql where字串</param>      
        public void appendCriteriaText(string value, string criteria, ref string where)
        {
            if (string.IsNullOrEmpty(value)) return;

            string cond = string.Format(criteria, value);
            where += string.Concat(" AND ", cond);
        }


        /// <summary>
        /// 轉換where條件 存文字
        /// </summary>        
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where條件</param>
        /// <param name="where">ref sql where字串</param>      
        public void appendCriteriaTextOR(string value, string criteria, ref string where)
        {
            if (string.IsNullOrEmpty(value)) return;

            string cond = string.Format(criteria, value);
            where += string.Concat(" or  ", cond);
        }


        /// <summary>
        /// 轉換where條件
        /// </summary>
        /// <param name="parameterName">parameter參數名稱</param>
        /// <param name="value">值</param>
        /// <param name="criteria">自訂的where條件</param>
        /// <param name="where">ref sql where字串</param>
        /// <param name="SqlParameters">ref SqlParameters </param>
        /// <param name="dbType">資料欄位型別</param>
        public void appendCriteriaOR(string parameterName, string value, string criteria,
            ref string where, ref List<SqlParameter> SqlParameters, SqlDbType dbType)
        {
            if (string.IsNullOrEmpty(value)) return;

            string cond = string.Format(criteria, "@" + parameterName);
            where += string.Concat(" OR ", cond);

            var p = new SqlParameter(parameterName, dbType);
            p.Value = value;
            SqlParameters.Add(p);
        }

        /// <summary>
        /// 轉換where條件 by 西元年日期
        /// </summary>
        /// <param name="dtStart">開始日期</param>
        /// <param name="dtEnd">結束日期</param>
        /// <param name="columnName">欄位行稱</param>
        /// <param name="where">ref SqlParameters</param>
        public void appendCriteriaDate(string dtStart, string dtEnd, string columnName, ref string where)
        {

            if (string.IsNullOrEmpty(dtStart) && string.IsNullOrEmpty(dtEnd)) return;

            DateTime StarDate;
            DateTime EndDate;
            DateTime tempDate;

            //判斷是否為日期格式
            DateTime.TryParse(dtStart, out StarDate);
            DateTime.TryParse(dtEnd, out EndDate);

            if (!string.IsNullOrEmpty(dtStart) && !string.IsNullOrEmpty(dtEnd))
            {
                if (StarDate.CompareTo(EndDate) == 1)  //前後日期大小轉換
                {
                    tempDate = StarDate;
                    StarDate = EndDate;
                    EndDate = tempDate;
                }

                where += string.Format(" AND {0} between '{1}' and '{2}'", columnName, StarDate.ToShortDateString(), EndDate.ToShortDateString());
            }
            else if (string.IsNullOrEmpty(dtEnd))
            {
                where += string.Format(" AND  {0} >= '{1}'", columnName, StarDate.ToShortDateString());
            }
            else if (string.IsNullOrEmpty(dtStart))
            {
                where += string.Format(" AND {0} <= '{1}'", columnName, EndDate.ToShortDateString());
            }
        }


        /// <summary>
        /// 轉換where條件 by 西元年日期
        /// </summary>
        /// <param name="dtStart">開始日期</param>
        /// <param name="dtEnd">結束日期</param>
        /// <param name="columnName">欄位行稱</param>
        /// <param name="formatDate">格式化日期  傳入 yyyy/MM/dd 這種</param>
        /// <param name="where">ref SqlParameters</param>
        public void appendCriteriaDate(string dtStart, string dtEnd, string columnName, string formatDate, ref string where)
        {

            if (string.IsNullOrEmpty(dtStart) && string.IsNullOrEmpty(dtEnd)) return;

            DateTime StarDate;
            DateTime EndDate;
            DateTime tempDate;

            //判斷是否為日期格式
            DateTime.TryParse(dtStart, out StarDate);
            DateTime.TryParse(dtEnd, out EndDate);

            if (!string.IsNullOrEmpty(dtStart) && !string.IsNullOrEmpty(dtEnd))
            {
                if (StarDate.CompareTo(EndDate) == 1)  //前後日期大小轉換
                {
                    tempDate = StarDate;
                    StarDate = EndDate;
                    EndDate = tempDate;
                }

                where += string.Format(" AND {0} between '{1}' and '{2}'", columnName, StarDate.ToString(formatDate), EndDate.ToString(formatDate));
            }
            else if (string.IsNullOrEmpty(dtEnd))
            {
                where += string.Format(" AND  {0} >= '{1}'", columnName, StarDate.ToString(formatDate));
            }
            else if (string.IsNullOrEmpty(dtStart))
            {
                where += string.Format(" AND {0} <= '{1}'", columnName, EndDate.ToString(formatDate));
            }
        }


        /// <summary>
        /// 轉換where條件 by 西元年日期 字串
        /// </summary>
        /// <param name="Start">開始日期</param>
        /// <param name="End">結束日期</param>
        /// <param name="columnName">欄位行稱</param>
        /// <param name="where">ref SqlParameters</param>
        public void appendCriteriaDateText(string Start, string End, string columnName, ref string where)
        {
            if (string.IsNullOrEmpty(Start) && string.IsNullOrEmpty(End)) return;

            string tempDate = string.Empty;

            if (!string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End))
            {
                if (Start.CompareTo(End) == 1)  //前後日期大小轉換
                {
                    tempDate = Start;
                    Start = End;
                    End = tempDate;
                }

                Start += " 00:00:00";
                End += " 23:59:59";

                where += string.Format(" AND {0} between '{1}' and '{2}'", columnName, Start, End);
            }
            else if (string.IsNullOrEmpty(End))
            {
                Start += " 00:00:00";
                where += string.Format(" AND  {0} >= '{1}'", columnName, Start);
            }
            else if (string.IsNullOrEmpty(Start))
            {
                End += " 23:59:59";
                where += string.Format(" AND {0} <= '{1}'", columnName, End);
            }
        }



        /// <summary>
        /// 轉換where條件 by 民國日期
        /// </summary>
        /// <param name="Start">開始日期</param>
        /// <param name="End">結束日期</param>
        /// <param name="columnName">欄位行稱</param>
        /// <param name="where">ref SqlParameters</param>
        public void appendCriteriaChineseDay(string Start, string End, string columnName, ref string where)
        {
            if (string.IsNullOrEmpty(Start) && string.IsNullOrEmpty(End)) return;

            string tempDate = string.Empty;

            if (!string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End))
            {

                if (Start.CompareTo(End) == 1)  //前後日期大小轉換
                {
                    tempDate = Start;
                    Start = End;
                    End = tempDate;
                }

                where += string.Format(" AND {0} between '{1}' and '{2}'", columnName, Start, End);
            }
            else if (string.IsNullOrEmpty(End))
            {
                where += string.Format(" AND  {0} >= '{1}'", columnName, Start);
            }
            else if (string.IsNullOrEmpty(Start))
            {
                where += string.Format(" AND {0} <= '{1}'", columnName, End);
            }
        }

        public void appendCriteriaWhereInString(string[] values, string criteria, ref string where)
        {
            if (values.Length < 1)
            {
                return;
            }
            string result = string.Concat("'", string.Join("','", values), "'");
            where += string.Format(" and  " + criteria, result);
        }
    }
}
