using JetBrains.ReSharper.Psi.Xaml.Tree;

namespace VSPowerTools.Extensions.Xaml
{
	public static class INamespaceAliasExtension
	{
		public static string GetValue(this INamespaceAlias alias)
		{
			return alias.Value != null && alias.Value.ValueToken != null ? alias.Value.ValueToken.UnquotedValue : null;
		}
	}
}
