using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMLib.Collections;
using AMLib.VisualStudio;

namespace VSPowerTools.ToolWindows.LanguageMassEditor.BusinessObjects
{
    public class Ressource : ChangeCollection<ResxNode>, IEquatable<Ressource>
	{
		public static string[] GetRessourceName(string path)
		{
			var result = new string[2];
			result[0] = Path.GetDirectoryName(path);
			result[1] = "";

			var tmp = Path.GetFileNameWithoutExtension(path);
			if (tmp != null)
			{
				var tmpSplit = tmp.Split('.').ToList();
				if (tmpSplit.Count >= 1)
				{
					result[1] = string.Join("", tmpSplit.Take(1));
					return result;
				}
			}
			
			return result;
		}

	    public Project Project
	    {
		    get
		    {
			    if (Files.Count > 0)
			    {
				    return Files.First().Project;
			    }

			    return null;
		    }
	    }

	    protected override void RemoveItem(int index)
	    {
			ResxNode item = Items[index];

	        foreach (var nodeFile in Files)
	        {
	        	var node = nodeFile.Nodes.FirstOrDefault(d => d.Tag == item.Tag);
				if(node != null)
				{
					nodeFile.Nodes.Remove(node);
				}
	        }
            base.RemoveItem(index);
	    }

	    public Ressource(string ressourceName)
		{
			RessourceName = ressourceName;
		}

		private readonly List<ResxFile> _files = new List<ResxFile>();
		public List<ResxFile> Files
	    {
	        get
	        {
	            return _files;
	        }
	    }

	    private string _ressourceName;
		public string RessourceName
		{
			get { return _ressourceName; }
			set
			{
				_ressourceName = value;
			}
		}

		public void AddFile(ResxFile file)
		{
            if (File.Exists(file.ResourceFilePath) && !Files.Any(f => f.ResourceFilePath == file.ResourceFilePath))
			{
				Files.Add(file);
			}
		}

		public bool Save(out Exception exception)
		{
		    exception = null;
            foreach (var nodeFile in Files)
			{
				if(!nodeFile.Save(out exception))
				{
				    return false;
				}
			}

		    return true;
		}

		public IEnumerable<string> AvailableTags
		{
			get { return Items.Select(t => t.Tag); }
		}

		public void ClearFiles()
		{
            Files.Clear();
		}

	    public bool Equals(Ressource other)
	    {
            if (other == null)
                return false;
	        return this.RessourceName == other.RessourceName;
	    }
	}
}
