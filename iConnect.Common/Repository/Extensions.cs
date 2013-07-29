/*
 * The code contained in this file is taken from the free utility called LINQKit, 
 * which is a free set of extensions for LINQ to SQL and Entity Framework power users.
 * The source, demo and license information can be found at: http://www.albahari.com/nutshell/linqkit.aspx
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace iConnect.Common
{
   
    public static class Extensions
    {
        public static Expression Expand(this Expression expression)
        {
            return new ExpressionExpander().Visit(expression);
        }
        
    }
}