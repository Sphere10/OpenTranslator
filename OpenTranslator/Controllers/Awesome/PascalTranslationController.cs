using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Data;

using OpenTranslator.Repostitory;
using OpenTranslator.Data;
using OpenTranslator.Models.Input;

using Ionic.Zip;

namespace OpenTranslator.Controllers.Awesome
{
    public class PascalTranslationController : Controller
    {
		public StringBuilder sbPo = new StringBuilder();
		private ITranslation ITranslation;
		private ILanguages ILanguages;
		private ITranslationLog ITranslation_Log;

		public PascalTranslationController()
		{
			this.ITranslation = new TranslationRepository();
			this.ILanguages = new LanguageRepository();
			this.ITranslation_Log = new TranslationLogRepository(new StringTranslationEntities());
		}
        // GET: PascalTranslation
        public ActionResult Index()
        {
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index","User");
			}
			else
			{
				ViewBag.pageName="Index";
			  return View("Index","_AdminLayout");
			}
        }
		public ActionResult embeded()
		{
			ViewBag.pageName="embeded";
			return View("Index","_LayoutEmbedAdmin");
		}

		[HttpPost]
		public ActionResult GetUploadedFile(HttpPostedFileBase postedFile, string pagename)
        {
			string path1 = Request.Url.AbsolutePath;
			ViewBag.errormsg = "";
			
			if (postedFile != null)
            {
				string fileextension = System.IO.Path.GetExtension(postedFile.FileName);
				if (fileextension != ".po")
				{
					ViewBag.errormsg = "File format is not .po";
					//return View("Index");
				}
				else
				{
					string path = System.IO.Path.Combine(Server.MapPath("~/Content/"));
					path = path + "1" + ".po";
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
					postedFile.SaveAs(path);
					var input = System.IO.File.ReadAllText(path);
					var LanguageCode = "en";
					using (var sr = new StreamReader(path, Encoding.UTF8))
					this.LoadFromReader(sr,"Import",LanguageCode);
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
				}
                
            }
			else
			{
				ViewBag.errormsg = " Please select file";
				
			}
			if(pagename == "Index")
			{
				ViewBag.pageName="Index";
				 return View("Index","_AdminLayout");
			}	
			else
			{
				ViewBag.pageName="embeded";
				return View("Index","_LayoutEmbedAdmin");
			}
        }
		private enum BlockType
		{
			None,
			Context,
			Id,
			IdPlural,
			Str,
		}
		private static string Unescape(string str)
		{
			str = Regex.Replace(str, @"(^|[^\\])\\r", "$1\r");
			str = Regex.Replace(str, @"(^|[^\\])\\n", "$1\n");
			str = Regex.Replace(str, @"(^|[^\\])\\t", "$1\t");
			str = str.Replace("\\\"", "\"");

			return str;
		}
		public void LoadFromReader(TextReader reader, string input, string LanguageCode, string filename="")
		{
			
			var regex = new Regex(@"""(.*)""", RegexOptions.Compiled | RegexOptions.Singleline);
			var regex2 = new Regex(@"^msgstr\[([0-9]+)\]", RegexOptions.Compiled);
			sbPo.Append("msgid \"\"\r\nmsgstr \"Content-Type: text/plain; charset=UTF-8\"\r\n");
			var type = BlockType.None;
			var TextId ="";
			var Text ="";
			string line = null;
			while ((line = reader.ReadLine()) != null)
			{
				line = line.Trim();
				if (line.StartsWith("#"))
				{
					TextId = line.Substring(line.IndexOf(":")+1);
					continue;
				}
				if (string.IsNullOrEmpty(line))
					continue;
			
				if (line.StartsWith("msgctxt"))
					type = BlockType.Context;
				else if (line.StartsWith("msgid_plural"))
					type = BlockType.IdPlural;
				else if (line.StartsWith("msgid"))
					type = BlockType.Id;
				else if (line.StartsWith("msgstr"))
					type = BlockType.Str;
				else if (!line.StartsWith("\""))
					type = BlockType.None;

				var match = regex.Match(line);
				if (match.Success)
				{
					var val = Unescape(match.Groups[1].Value);
					switch (type)
					{
						case BlockType.Context:
							var context = val;
						break;
						case BlockType.Id:
							 Text = val;
						break;
						case BlockType.IdPlural:
							var IdPlural = val;
						break;
						case BlockType.Str:
							var index = 0;
							match = regex2.Match(line);
							if (match.Success)
								index = Convert.ToInt32(match.Groups[1].Value);
							var g = val;
							if(input.Equals("Import"))
							{
								if(!TextId.Equals("") && !Text.Equals(""))
								    this.InsterRecord(TextId,Text,LanguageCode);
							}
							else
							{
								if(!TextId.Equals("") && !Text.Equals(""))
									this.GetTranslationRecord(TextId,Text,LanguageCode);
									
							}
						break;
					}
				}
				
			}
			string path = System.IO.Path.Combine(Server.MapPath("~/Content/Export/"));
			if(input.Equals("Export"))
				System.IO.File.WriteAllText(path + "\\" + filename + "."+ LanguageCode +".po", sbPo.ToString());
		}
		public void InsterRecord(string TextId, string Text, string LanguageCode)
		{
			
			AdminInput input = new AdminInput();
			input.TextId = TextId;
			input.Text = Text;
			input.LanguageCode = LanguageCode;
			if (this.doesTextIdExist(input) == false)
			{
				input.TextId = input.TextId;
				Text text = new Text();
				text.TextId = input.TextId;
				text.System = true;

				Translation translation = new Translation();
				translation.LanguageCode = input.LanguageCode;
				translation.TextId = text.TextId;
				translation.Translated_Text = input.Text;
				translation.OfficialBoolean = true;
				translation.Votes = 0;

				TranslationLog translation_log = new TranslationLog();
				translation_log.TextId = input.TextId;
				translation_log.System_Date = DateTime.Now;
				translation_log.LanguageCode = input.LanguageCode;
				translation_log.Translated_Text = input.Text;

				ITranslation.InsertTextTranslation(text, translation, translation_log);
			}
		}

		public void GetTranslationRecord(string TextId, string Text, string LanguageCode)
		{
				var Translations = ITranslation.GetTranslation().Where(x => x.TextId == TextId && x.OfficialBoolean == true && x.LanguageCode == LanguageCode).FirstOrDefault();
			    if(Translations == null)
					sbPo.Append("\r\n#: " + TextId + "\r\nmsgid \"" + Text + "\"\r\nmsgstr \"" + "\"\r\n");
				else
					sbPo.Append("\r\n#: " + TextId + "\r\nmsgid \"" + Text + "\"\r\nmsgstr \"" + Translations.Translated_Text + "\"\r\n");
				
		}

		public bool doesTextIdExist(AdminInput input)
		{

			var id = Convert.ToDecimal(input.Id);
			if (input.Id == 0)
			{
				var text = ITranslation.GetText().Where(x => x.TextId == input.TextId).FirstOrDefault();
				if (text != null)
				{
					//ViewBag.errormsg += input.TextId  + " TextId already exist.\n";
					ViewBag.errormsg = "Records already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var text = ITranslation.GetText().Where(x => x.TextId == input.TextId && x.Id != id).FirstOrDefault();
				if (text != null)
				{
					//ViewBag.errormsg += input.TextId + "TextId already exist.";
					ViewBag.errormsg = "Records already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		[HttpPost]
		public ActionResult DownloadPoFile(HttpPostedFileBase postedFile, string pagename)
        {
			ViewBag.errormsgexport = "";
			if (postedFile != null)
			{
				string fileextension = System.IO.Path.GetExtension(postedFile.FileName);
				if (fileextension != ".po")
				{
					ViewBag.errormsgexport = "File format is not .po";
					
				}
				else
				{
					string path = System.IO.Path.Combine(Server.MapPath("~/Content/Export/"));
					System.IO.Directory.CreateDirectory(path);
					string filepath = path + postedFile.FileName;
					var filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
					if (System.IO.File.Exists(filepath))
						System.IO.File.Delete(filepath);
					postedFile.SaveAs(filepath);
					List<Language> list = new List<Language>();
					list = ILanguages.GetLanguages().ToList();
					for (int i = 0; i < list.Count; i++)
					{
						var Languagecode = list[i].LanguageCode;

						var input = System.IO.File.ReadAllText(filepath);
						sbPo = new StringBuilder();
						using (var sr = new StreamReader(filepath, Encoding.UTF8))
							this.LoadFromReader(sr, "Export", Languagecode, filename);
					}
					string SourceFolderPath = System.IO.Path.Combine(path, "Initial");

					Response.Clear();
					Response.ContentType = "application/zip";
					Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}", "Languages" + ".zip"));

					bool recurseDirectories = true;
					using (ZipFile zip = new ZipFile())
					{
						zip.AddSelectedFiles("*", path, string.Empty, recurseDirectories);
						zip.Save(Response.OutputStream);
					}
					Response.End();

					System.IO.Directory.Delete(path, true);
					ViewBag.errormsgexport = "File downloaded successfully.";
				}
			}
			else
			{
				ViewBag.errormsgexport = "Please Select File";
			}

			if(pagename == "Index")
			{
				ViewBag.pageName="Index";
				 return View("Index","_AdminLayout");
			}	
			else
			{
				ViewBag.pageName="embeded";
				return View("Index","_LayoutEmbedAdmin");
			}
		}

		public ActionResult Edit(string TextId, string code)
        {
            AdminController adminController = new AdminController();
            return adminController.Edit(TextId,code,"User");
        }
		[HttpPost]
		public ActionResult Edit(TranslationInput input)
		{
			if (!ModelState.IsValid)
            {
                return PartialView("../User/EditTranslation", input);
            }
			AdminController adminController = new AdminController();
            return adminController.Edit(input);
		}
    }
}