using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Xaml.Impl;
using JetBrains.ReSharper.Psi.Xaml.Tree;

namespace VSPowerTools.Extensions.Generic
{
	public static class IFileExtensions
	{
		public static bool AddNamespaceOnce(this IFile file, string aliasName, string clrNamespace, string assemblyName, bool applyFormatting)
		{
			ICSharpFile cSharp = file as ICSharpFile;
			IXamlFile xaml = file as IXamlFile;

			if(cSharp != null)
			{
				List<IUsingNamespaceDirective> namespaces = new List<IUsingNamespaceDirective>();
				var usingLists = file.Children<IUsingList>();
				var firstList = usingLists.FirstOrDefault();
				if(firstList != null)
				{
					namespaces = firstList.Children<IUsingNamespaceDirective>().ToList();
				}
				
				var lastNamespace = namespaces.LastOrDefault();

				if (lastNamespace != null)
				{
					if(namespaces.ToLookup(d => d.ImportedSymbolName.QualifiedName).Contains(clrNamespace))
						return false;
					
					var factory = CSharpElementFactory.GetInstance(file.GetPsiModule(), true, false);

					var created = factory.CreateUsingDirective(clrNamespace);

					ModificationUtil.AddChildAfter(lastNamespace, created);

					// todo am fix
//					file.GetPsiServices().MarkAsDirty(file.GetSourceFile());
				}

				return true;
			}

			#region Xaml
			
			if (xaml != null)
			{
				List<INamespaceAlias> namespaces = new List<INamespaceAlias>();

				file.ProcessChildren(delegate(ITreeNode node)
				{
					var casted = node as INamespaceAlias;
					if (casted != null)
					{
						namespaces.Add(casted);
					}
				});

				//clr-namespace:ResourceFileSolution.Usercontrols

				var lastNamespace = namespaces.LastOrDefault();

				if (lastNamespace != null)
				{
					if (namespaces.Any(d => clrNamespace.Equals(d.DeclaredElement.Value)))
						return false;

					if (namespaces.Any(d =>
					{
						var parts = SplitXamlNamespace(d);
						if (parts.Length == 1)
						{
							if (BuildXamlNamespace(clrNamespace, assemblyName).Equals(BuildXamlNamespace(parts[0], "")))
								return true;
						}
						if (parts.Length == 2)
						{
							if (BuildXamlNamespace(clrNamespace, assemblyName).Equals(BuildXamlNamespace(parts[0], parts[1])))
								return true;
						}

						return false;
					}))
						return false;
					
					// clr-namespace:SDKSample;assembly=SDKSampleLibrary
					var factory = (XamlElementFactory)XamlElementFactory.GetInstance(lastNamespace);
					var created = factory.CreateNamespaceAlias(aliasName, BuildXamlNamespace(clrNamespace, assemblyName));

					ModificationUtil.AddChildAfter(lastNamespace, created);
				}

				return true;
			}

			#endregion Xaml

			return false;
		}

		private static string BuildXamlNamespace(string @namespace, string assemblyName)
		{
			return string.Format("clr-namespace:{0};assembly={1}", @namespace, assemblyName);
		}

		private static string[] SplitXamlNamespace(INamespaceAlias alias)
		{
			string[] source = alias.DeclaredElement.Value.Split(';');
			if (source.Length == 2)
			{
				source[0] = source[0].Substring("clr-namespace:".Length);
				source[1] = source[1].Substring("assembly=".Length);
			}
			if (source.Length == 1)
			{
				source[0] = source[0].Substring("clr-namespace:".Length);
			}

			return source;
		}
	}
}
