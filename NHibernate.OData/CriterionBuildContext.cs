using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NHibernate.SqlCommand;

namespace NHibernate.OData
{
    internal class CriterionBuildContext
    {
        public ODataSessionFactoryContext SessionFactoryContext { get; private set; }
        public IDictionary<string, Alias> AliasesByName { get; private set; }
        public bool CaseSensitiveResolve { get; private set; }
        public bool CaseSensitiveLike { get; private set; }
        public NameResolver NameResolver { get; private set; }
        public JoinType JoinType { get; set; }

        public int ExpressionLevel
        {
            get { return _lambdaContextStack.Count; }
        }

        private int _aliasCounter;

        private readonly Stack<LambdaExpressionContext> _lambdaContextStack = new Stack<LambdaExpressionContext>();

        public CriterionBuildContext(ODataSessionFactoryContext sessionFactoryContext, bool caseSensitiveResolve, bool caseSensitiveLike, NameResolver nameResolver, JoinType joinType = JoinType.InnerJoin)
        {
            Require.NotNull(sessionFactoryContext, "sessionFactoryContext");
            Require.NotNull(nameResolver, "nameResolver");

            SessionFactoryContext = sessionFactoryContext;
            CaseSensitiveResolve = caseSensitiveResolve;
            CaseSensitiveLike = caseSensitiveLike;
            NameResolver = nameResolver;
            JoinType = joinType;

            AliasesByName = new Dictionary<string, Alias>(StringComparer.Ordinal);
        }

        public void AddAliases(IEnumerable<Alias> aliasesToAdd)
        {
            Require.NotNull(aliasesToAdd, "aliasesToAdd");

            foreach (var alias in aliasesToAdd)
                AddAlias(alias);
        }

        public void AddAlias(Alias alias)
        {
            Require.NotNull(alias, "alias");

            AliasesByName.Add(alias.Name, alias);
        }

        public string CreateUniqueAliasName()
        {
            return "t" + (++_aliasCounter).ToString(CultureInfo.InvariantCulture);
        }

        public void PushLambdaContext(string parameterName, System.Type parameterType, string parameterAlias)
        {
            if (_lambdaContextStack.Any(x => x.ParameterName.Equals(parameterName, StringComparison.Ordinal)))
                throw new ODataException(string.Format(ErrorMessages.Expression_LambdaParameterIsAlreadyDefined, parameterName));

            _lambdaContextStack.Push(new LambdaExpressionContext(parameterName, parameterType, parameterAlias));
        }

        public void PopLambdaContext()
        {
            _lambdaContextStack.Pop();
        }

        public LambdaExpressionContext FindLambdaContext(string parameterName)
        {
            Require.NotNull(parameterName, "parameterName");

            return _lambdaContextStack.FirstOrDefault(x => x.ParameterName.Equals(parameterName, StringComparison.Ordinal));
        }
    }
}
