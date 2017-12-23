using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Services.Description;
using System.Data;
using OpenTranslator.Repostitory;
using OpenTranslator.Data;
using OpenTranslator.Models.Input;

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
			this.ITranslation = new TranslationRepository(new StringTranslationEntities());
			this.ILanguages = new LanguageRepository(new StringTranslationEntities());
			this.ITranslation_Log = new TranslationLogRepository(new StringTranslationEntities());
		}
        // GET: PascalTranslation
        public ActionResult Index()
        {
		    return View();
        }

		[HttpPost]
		public ActionResult GetUploadedFile(HttpPostedFileBase postedFile)
        {
			ViewBag.errormsg = "";

			if (postedFile != null)
            {
			
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/"));
                path = path + "1" + ".po";
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                postedFile.SaveAs(path);
				var input = System.IO.File.ReadAllText(path);
				using (var sr = new StreamReader(path, Encoding.UTF8))
				this.LoadFromReader(sr,"Import");
            }
			else
			{
				ViewBag.errormsg = " Please select file";
			}
            return View("Index");
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
		public void LoadFromReader(TextReader reader, string input)
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
								this.InsterRecord(TextId,Text);
							}
							else
							{
								if(!TextId.Equals("") && !Text.Equals(""))
								{
									List<Language> list = new List<Language>();
									list = ILanguages.GetLanguages().ToList();	
									for (int i = 0; i < list.Count; i++)
									{
										var Languagecode = list[i].LanguageCode;
										
										this.GetTranslationRecord(TextId,Text);
									}
								}
									
							}
						break;
					}
				}
				
			}
			System.IO.File.WriteAllText("D:\\Demo.en.po", sbPo.ToString());
		}
		public void InsterRecord(string TextId, string Text)
		{
			
			AdminInput input = new AdminInput();
			input.TextId = TextId;
			input.Text = Text;
			input.LanguageCode = "en";
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

		public void GetTranslationRecord(string TextId, string Text)
		{
				var Translations = ITranslation.GetTranslation().Where(x => x.TextId == TextId && x.Translated_Text == Text && x.OfficialBoolean == true).FirstOrDefault();
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
					ViewBag.errormsg = "Records already exist.\n";
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
					ViewBag.errormsg = "Records already exist.\n";
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		[HttpPost]
		public ActionResult DownloadPoFile(HttpPostedFileBase postedFile)
        {
			

			if (postedFile != null)
            {
			
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/"));
                path = path + "1" + ".po";
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                postedFile.SaveAs(path);
				var input = System.IO.File.ReadAllText(path);
 
				using (var sr = new StreamReader(path, Encoding.UTF8))
				this.LoadFromReader(sr,"Export");
            }
            return View("Index");
        }

		public ActionResult Edit(string TextId, string code)
        {
            AdminController adminController = new AdminController();
            return adminController.Edit(TextId,code,"User");
        }
		[HttpPost]
		public ActionResult Edit(TranslationInput input)
		{
			AdminController adminController = new AdminController();
            return adminController.Edit(input);
		}
    }
}