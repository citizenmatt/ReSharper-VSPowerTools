using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xml.Tree;

namespace VSPowerTools.Extensions.Xaml
{
	public static class ITreeNodeExtensions
	{
		public static bool IsChildOfPropertyAttributeValue(this ITreeNode node, out IXmlToken result)
		{
			if(node != null && node.Parent != null)
			{
				IPropertyAttributeValue casted = node.Parent as IPropertyAttributeValue;
				if (casted != null)
				{
					result = casted.GetTextToken();
					return true;
				}
				else
				{
					result = null;
					return false;
				}
			}
			result = null;
			return false;
		}

		public static void After([NotNull] this ITreeNode node, ITreeNode append)
		{
			ModificationUtil.AddChildAfter(node, append);
		}
	}
}
