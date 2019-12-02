// <copyright file="HtmlHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// Helper class for preventing error in cshtml file.
    /// </summary>
    public static class HtmlHelper
    {
        /// <summary>
        /// Get property name from a lamda expression.
        /// </summary>
        /// <typeparam name="TModel">A model.</typeparam>
        /// <typeparam name="TProperty">A property.</typeparam>
        /// <param name="expression">An expression which return a property from a model.</param>
        /// <returns>Property name.</returns>
        public static string GetPropertyName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }

        /// <summary>
        /// Get SelectList from list of items to prevent name typo.
        /// </summary>
        /// <typeparam name="TModel">Model of list items.</typeparam>
        /// <typeparam name="TProperty1">Property of data field.</typeparam>
        /// <typeparam name="TProperty2">Property of text field.</typeparam>
        /// <param name="items">List of items.</param>
        /// <param name="dataValueField">Similar to TProperty1.</param>
        /// <param name="dataTextField">Similar to TProperty2.</param>
        /// <param name="selectedValue">Select value object.</param>
        /// <returns>A SelectList.</returns>
        public static SelectList GetSelectList<TModel, TProperty1, TProperty2>(this IEnumerable<TModel> items, Expression<Func<TModel, TProperty1>> dataValueField, Expression<Func<TModel, TProperty2>> dataTextField, object selectedValue)
        {
            return new SelectList(items, dataValueField.GetPropertyName(), dataTextField.GetPropertyName(), selectedValue);
        }

        /// <summary>
        /// Uses UrlHelper.Action to generate url action link.
        /// </summary>
        /// <typeparam name="TController">Name of controller to call.</typeparam>
        /// <param name="urlHelper">Invokes from UrlHelper.</param>
        /// <param name="actionName">Action name for request.</param>
        /// <param name="routeValues">Route values if having.</param>
        /// <returns>Url String.</returns>
        public static string Action<TController>(this UrlHelper urlHelper, Expression<Func<TController, string>> actionName, object routeValues = null)
            where TController : Controller
        {
            var controllerName = typeof(TController).Name;
            controllerName = controllerName.Substring(0, controllerName.LastIndexOf("Controller"));
            return urlHelper.Action(actionName.Compile()(null), controllerName, routeValues);
        }

        /// <summary>
        /// Get property name of a model.
        /// </summary>
        /// <typeparam name="TModel">Type of model.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="model">The model to get property.</param>
        /// <param name="property">Property of model.</param>
        /// <returns>Name of property.</returns>
        public static string NameOf<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> property)
        {
            return property.GetPropertyName();
        }
    }
}
