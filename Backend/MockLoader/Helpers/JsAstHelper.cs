using System.Globalization;
using Esprima.Ast;
using MockLoader.Models;

namespace MockLoader.Helpers
{
    /// <summary>
    /// Helper to parse JavaScript code and extract objects from it
    /// </summary>
    internal class JsAstHelper
    {
        /// <summary>
        /// Extract top level arrays from the JS script
        /// </summary>
        /// <param name="dataSheet">JS file with data</param>
        /// <exception cref="ArgumentNullException">Data can;t be null</exception>
        public TopLevelArraysResult BuildTopLevelArraysMap(Script dataSheet)
        {
            if (dataSheet is null)
                throw new ArgumentNullException(nameof(dataSheet));

            var result = new TopLevelArraysResult();

            foreach (var statement in dataSheet.Body)
            {
                if (statement is not VariableDeclaration variableDeclaration)
                    continue;

                foreach (var declarator in variableDeclaration.Declarations)
                {
                    if (declarator.Id is not Identifier identifier)
                        continue;

                    if (string.IsNullOrWhiteSpace(identifier.Name))
                        continue;

                    if (declarator.Init is not ArrayExpression arrayExpression)
                        continue;

                    if (!result.Arrays.TryAdd(identifier.Name, arrayExpression))
                    {
                        result.Issues.Add(
                            $"Duplicate top-level array declaration '{identifier.Name}' was found.");
                    }
                }
            }

            return result;
        }

        public bool TryGetObject(ObjectExpression obj, string propertyName, out ObjectExpression? result)
        {
            var value = GetPropertyValue(obj, propertyName);

            if (value is ObjectExpression objectExpression)
            {
                result = objectExpression;
                return true;
            }

            result = null;
            return false;
        }

        public bool TryGetString(ObjectExpression obj, string propertyName, out string result)
        {
            var value = GetPropertyValue(obj, propertyName);

            if (value is Literal literal && literal.Value is string s)
            {
                result = s;
                return true;
            }

            result = string.Empty;
            return false;
        }

        public bool TryGetBool(ObjectExpression obj, string propertyName, out bool result)
        {
            var value = GetPropertyValue(obj, propertyName);

            if (value is Literal { Value: bool b })
            {
                result = b;
                return true;
            }

            result = false;
            return false;
        }

        public bool TryGetDecimal(ObjectExpression obj, string propertyName, out decimal? result)
        {
            var value = GetPropertyValue(obj, propertyName);

            if (value is Literal literal)
            {
                switch (literal.Value)
                {
                    case decimal d:
                        result = d;
                        return true;

                    case double d:
                        result = (decimal)d;
                        return true;

                    case float f:
                        result = (decimal)f;
                        return true;

                    case int i:
                        result = i;
                        return true;

                    case long l:
                        result = l;
                        return true;

                    case string s when decimal.TryParse(s, out var parsed):
                        result = parsed;
                        return true;
                }
            }

            result = 0;
            return false;
        }

        public bool TryGetInt(ObjectExpression obj, string propertyName, out int? result)
        {
            result = null;

            var value = GetPropertyValue(obj, propertyName);

            if (value is Literal { Value: var v })
            {
                result = v switch
                {
                    null => null,
                    int i => i,
                    long l => (int)l,
                    double d => (int)d,
                    _ => null
                };

                return v is null or int or long or double;
            }

            return false;
        }

        public bool TryGetNestedString(ObjectExpression obj, string objectName, string propertyName, out string result)
        {
            result = string.Empty;

            if (!TryGetObject(obj, objectName, out var nested) || nested == null)
                return false;

            return TryGetString(nested, propertyName, out result);
        }

        public bool TryGetNestedBool(ObjectExpression obj, string objectName, string propertyName, out bool result)
        {
            result = false;

            if (!TryGetObject(obj, objectName, out var nested) || nested == null)
                return false;

            return TryGetBool(nested, propertyName, out result);
        }

        public bool TryGetNestedInt(ObjectExpression obj, string objectName, string propertyName, out int? result)
        {
            result = null;

            if (!TryGetObject(obj, objectName, out var nested) || nested == null)
                return false;

            var value = GetPropertyValue(nested, propertyName);

            if (value is Literal { Value: var v })
            {
                result = v switch
                {
                    null => null,
                    int i => i,
                    long l => (int)l,
                    double d => (int)d,
                    _ => null
                };

                return v is null or int or long or double;
            }

            return false;
        }

        public bool TryGetDateTime(ObjectExpression obj, string propertyName, out DateTime result)
        {
            result = default;

            if (!TryGetString(obj, propertyName, out var raw))
                return false;

            return DateTime.TryParse(
                raw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out result);
        }

        public bool TryGetNestedDateTime(ObjectExpression obj, string objectName, string propertyName, out DateTime? result)
        {
            result = null;

            if (!TryGetObject(obj, objectName, out var nested) || nested == null)
                return false;

            var value = GetPropertyValue(nested, propertyName);

            if (value is Literal literal)
            {
                if (literal.Value is null)
                {
                    result = null;
                    return true;
                }

                if (literal.Value is string s &&
                    DateTime.TryParse(
                        s,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                        out var dt))
                {
                    result = dt;
                    return true;
                }
            }

            return false;
        }

        private Node? GetPropertyValue(ObjectExpression obj, string propertyName)
        {
            foreach (var propertyNode in obj.Properties)
            {
                if (propertyNode is not Property property)
                    continue;

                var keyName = property.Key switch
                {
                    Identifier identifier => identifier.Name,
                    Literal { Value: string s } => s,
                    _ => null
                };

                if (string.Equals(keyName, propertyName, StringComparison.Ordinal))
                    return property.Value;
            }

            return null;
        }
    }
}
