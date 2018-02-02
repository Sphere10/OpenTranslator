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
using OpenTranslator.Utils;

using Ionic.Zip;

namespace OpenTranslator.Controllers.Awesome
{
    public class PascalTranslationController : Controller
    {
        #region private properties

        public StringBuilder sbPo = new StringBuilder();
        private String errorMessage = String.Empty;

		private ITranslation ITranslation;
		private ILanguages ILanguages;
		private ITranslationLog ITranslation_Log;

        #endregion

        #region Constructor
        public PascalTranslationController()
		{
			ITranslation = new TranslationRepository();
			ILanguages = new LanguageRepository();
			ITranslation_Log = new TranslationLogRepository();
		}
        #endregion

        # region Public methods
        
        // GET: PascalTranslation
        public ActionResult Index()
        {
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index","User");
			}

			ViewBag.pageName="Index";
			return View("Index","_AdminLayout");
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
            errorMessage = string.Empty;

			if (postedFile != null)
            {
				string fileextension = Path.GetExtension(postedFile.FileName);

                //Check file extension
                if (fileextension != ".po")
				{
                    errorMessage = "File format is not .po";
                    return ViewToReturn(pagename);
                }

                string path = Path.Combine(Server.MapPath("~/Content/"));
				path = path + "1" + ".po";

                path.DeleteFile();

				postedFile.SaveAs(path);

                //Read the file content as string
				var input = System.IO.File.ReadAllText(path);

				using (var sr = new StreamReader(path, Encoding.UTF8))
				LoadFromReader(sr,"Import");

                path.DeleteFile();

            }

            return ViewToReturn(pagename);

        }

		[HttpPost]
		public ActionResult DownloadPoFile(HttpPostedFileBase postedFile, string pagename)
        {
            errorMessage = "";

            if (postedFile != null)
			{
				string fileextension = System.IO.Path.GetExtension(postedFile.FileName);

				if (fileextension != ".po")
				{
                    errorMessage = "File format is not .po";					
				}
				else
				{
					string path = Path.Combine(Server.MapPath("~/Content/Export/"));

                    Directory.CreateDirectory(path);

                    string filepath = path + postedFile.FileName;
					var filename = Path.GetFileNameWithoutExtension(filepath);

                    filepath.DeleteFile();

                    postedFile.SaveAs(filepath);

                    List<Language> list = new List<Language>(ILanguages.GetAll().ToList());

                    for (int i = 0; i < list.Count; i++)
					{
						var Languagecode = list[i].LanguageCode;
						var input = System.IO.File.ReadAllText(filepath);

                        sbPo = new StringBuilder();

                        using (var sr = new StreamReader(filepath, Encoding.UTF8))
							LoadFromReader(sr, "Export", filename);
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

					Directory.Delete(path, true);

                    errorMessage = "File downloaded successfully.";
				}
			}
			else
			{
                errorMessage = "Please Select File";
			}

            return ViewToReturn(pagename);
			
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

        /// <summary>
        /// Checks the page name and return its corresponding View.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        /// //TODO: IMPROVE THIS WHEN HAVE TIME!
        public ViewResult ViewToReturn(string pageName)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                ViewBag.errormsg = errorMessage;
            }

            if (pageName == "Index")
            {
                ViewBag.pageName = "Index";
                return View("Index", "_AdminLayout");
            }
            ViewBag.pageName = "embeded";
            return View("Index", "_LayoutEmbedAdmin");
        }


        #endregion

        #region Private Methods

        enum BlockType
        {
            None,
            Context,
            Id,
            IdPlural,
            Str,
        }


        /// <summary>
        /// This method reads each line from current file in order to save its content
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="input"></param>
        /// <param name="LanguageCode"></param>
        /// <param name="filename"></param>
        void LoadFromReader(TextReader reader, string input, string filename = "")
        {
            //clean error message 
            errorMessage = string.Empty;
            var currentLanguage = "en"; //English by default

            var regex = new Regex(@"""(.*)""", RegexOptions.Compiled | RegexOptions.Singleline);
            var regex2 = new Regex(@"^msgstr\[([0-9]+)\]", RegexOptions.Compiled);
            var type = BlockType.None;

            var textId = string.Empty;
            var originalText = string.Empty;
            var translatedText = string.Empty;

            string line = String.Empty;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                String language = String.Empty;

                if (line.StartsWith("#"))
                {
                    textId = line.Substring(line.IndexOf(":") + 1);
                    continue;
                }

                if (line.StartsWith("\"Language:"))
                {
                    var elarray =  line.Split(new string[] { "Language:" }, StringSplitOptions.None);
                    currentLanguage = elarray[1].Trim().Substring(0, 2);
                }

                //TODO: improve this!
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
                    var val = StringExtension.Unescape(match.Groups[1].Value);
                    switch (type)
                    {   
                        case BlockType.Id:
                            originalText = val;
                            textId = val.RemoveNonAlphanumerics().ConvertCaseString(StringExtension.Case.CamelCase).Replace(" ", String.Empty);
                            break;

                        case BlockType.Str:

                            if (String.IsNullOrEmpty(textId) && String.IsNullOrEmpty(originalText))
                            {
                                continue;
                            }
                            translatedText = val;
                            if (input.Equals("Import"))
                            {
                                InsterRecord(textId, originalText, translatedText, currentLanguage);
                            }

                            GetTranslationRecord(textId, originalText, translatedText, currentLanguage);
                            break;

                        default: continue;
                    }
                }

            }

            string path = Path.Combine(Server.MapPath("~/Content/Export/"));
            errorMessage = "File imported successfully";
            if (input.Equals("Export"))
            {
                sbPo.Append("msgid \"\"\r\nmsgstr \"Content-Type: text/plain; charset=UTF-8\"\r\n");
                System.IO.File.WriteAllText(path + "\\" + filename + "." + currentLanguage + ".po", sbPo.ToString());
            }
        }

       

        /// <summary>
        /// Insert new entities in DB
        /// </summary>
        /// <param name="textId"></param>
        /// <param name="originalText"></param>
        /// <param name="translatedText"></param>
        /// <param name="languageCode"></param>
        void InsterRecord(string textId, string originalText, string translatedText, string languageCode)
        {
            AdminInput input = new AdminInput
            {
                TextId = textId,
                Text = originalText,
                LanguageCode = languageCode
            };

            Translation translation = new Translation();
            TranslationLog translation_log = new TranslationLog();
            Text currentText = new Text();

            //In case the translated text doesn't exists, we proceed to save the objects
            if (!TranslateAlreadyExist(input, translatedText))
            {

                translation.LanguageCode = input.LanguageCode;
                translation.TextId = input.TextId;
                translation.Translated_Text = translatedText;
                translation.OfficialBoolean = true;
                translation.Votes = 0;
                
                translation_log.TextId = input.TextId;
                translation_log.System_Date = DateTime.Now;
                translation_log.LanguageCode = input.LanguageCode;
                translation_log.Translated_Text = translatedText;

                //We should know if this is a translation for a new text (Text entity doesn't exist yet) 
                // or it is a translation text for an already Text entity in db
                if (!TextIdExist(input))
                {
                    currentText.TextId = input.TextId;
                    currentText.OriginalText = originalText;
                    currentText.System = true;

                    //Save all the entities
                    ITranslation.InsertTextTranslation(currentText, translation, translation_log);
                    return;
                }

                //or only save the new translation and new translation log
                ITranslation.Save(translation);
                ITranslation_Log.Save(translation_log);

            }

        }

        /// <summary>
        /// Check if Text entity is already storaged in DB
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool TextIdExist(AdminInput input)
        {
            errorMessage = string.Empty;
            var textQuery = ITranslation.GetText().Where(x => x.TextId == input.TextId);

            if (input.Id != 0)
            {
                textQuery = textQuery.Where(y => y.Id != Convert.ToDecimal(input.Id));
            }

            var text = textQuery.FirstOrDefault();

            if (text != null )
            {
                errorMessage = "Some records already exist.";
            }

            return text != null;
        }

        /// <summary>
        /// Check if Translation entity already exists in storage DB
        /// </summary>
        /// <param name="input"></param>
        /// <param name="translatedText"></param>
        /// <returns></returns>
        bool TranslateAlreadyExist(AdminInput input, string translatedText)
        {
            var textQuery = ITranslation.GetAll().Where(x => x.TextId == input.TextId && x.LanguageCode == input.LanguageCode && x.Translated_Text.Equals(translatedText));

            return textQuery.FirstOrDefault() != null;

        }


        void GetTranslationRecord(string pTextId, string pOriginalText, string pTranslatedText, string LanguageCode)
        {
            var Translations = ITranslation.GetAll().Where(x => x.TextId == pTextId && x.OfficialBoolean == true && x.LanguageCode == LanguageCode).FirstOrDefault();
            
            if (Translations == null)
                sbPo.Append("\r\n#: " + pTextId + "\r\nmsgid \"" + pOriginalText + "\"\r\nmsgstr \"" + "\"\r\n");
            else
                sbPo.Append("\r\n#: " + pTextId + "\r\nmsgid \"" + pOriginalText + "\"\r\nmsgstr \"" + Translations.Translated_Text + "\"\r\n");

        }

        #endregion
    }
}