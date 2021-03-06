﻿using System;
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
    public class PascalTranslationController : BaseController
    {
        #region private properties

        public StringBuilder sbPo = new StringBuilder();
        private String errorMessage = String.Empty;

        private static string _defaultlanguage = "en";
        
        #endregion

        #region Constructor
        public PascalTranslationController(){}
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
                            LoadFromReader(sr, "Export", filename, Languagecode );
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
        /// This method reads each line from current file in order to save its content (import)
        /// or create a line for the file that will be downloaded (export)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="input"></param>
        /// <param name="LanguageCode"></param>
        /// <param name="filename"></param>
        void LoadFromReader(TextReader reader, string input, string filename = "", string languageCode="")
        {
            //clean error message 
            errorMessage = string.Empty;
            var currentLanguage = _defaultlanguage; //English by default

            var regex = new Regex(@"""(.*)""", RegexOptions.Compiled | RegexOptions.Singleline);
            var regex2 = new Regex(@"^msgstr\[([0-9]+)\]", RegexOptions.Compiled);
            var type = BlockType.None;

			sbPo.Append("msgid \"\"\r\nmsgstr \"Content-Type: text/plain; charset=UTF-8\"\r\n");

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

                type = GetBlockType(line);

                var match = regex.Match(line);
                if (match.Success)
                {
                    var val = StringExtension.Unescape(match.Groups[1].Value).Trim();
                    switch (type)
                    {   
                        case BlockType.Id:
                            originalText = val;
                            textId = val.RemoveNonAlphanumerics().ConvertCaseString(StringExtension.Case.CamelCase).Replace(" ", String.Empty);
                            if (String.IsNullOrEmpty(textId) || String.IsNullOrEmpty(originalText))
                            {
                                continue;
                            }
                            if (input.Equals("Import"))
                            {
                                InsterRecord(textId, originalText, originalText, _defaultlanguage);
                            }
                            break;

                        case BlockType.Str:

                            if (input.Equals("Import"))
                            {
                                ImportStrHandler(textId, originalText, val, currentLanguage);
                            }
                            else
                            {
                                ExportStrHandler(textId, originalText, val, languageCode);
                            }
                            break;

                        default: continue;
                    }
                }

            }

            string path = Path.Combine(Server.MapPath("~/Content/Export/"));
            errorMessage = "File imported successfully";
            if (input.Equals("Export"))
            {
                System.IO.File.WriteAllText(path + "\\" + filename + "." + languageCode + ".po", sbPo.ToString());
            }
        }

        /// <summary>
        /// get translation line to be write on the file that will be export
        /// EXPORT functionality
        /// </summary>
        /// <param name="textId"></param>
        /// <param name="originalText"></param>
        /// <param name="translatedText"></param>
        /// <param name="languageCode"></param>
        void ExportStrHandler(string textId, string originalText, string translatedText, string languageCode)
        {
            if (String.IsNullOrEmpty(textId))
            {
                return;
            }

            GetTranslationRecord(textId, originalText, translatedText, languageCode);
        }

        /// <summary>
        /// Insert on db current translated text. 
        /// IMPORT functionality
        /// </summary>
        /// <param name="textId"></param>
        /// <param name="originalText"></param>
        /// <param name="translatedText"></param>
        /// <param name="languageCode"></param>
        void ImportStrHandler(string textId, string originalText, string translatedText, string languageCode)
        {
            if (String.IsNullOrEmpty(translatedText) || String.IsNullOrEmpty(textId))
            {
                return;
            }
            InsterRecord(textId, originalText, translatedText, languageCode);
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
            if (!ITranslation.IsTranslateAlreadyStoraged(input, translatedText))
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
                if (!ITranslation.IsTextIdAlreadyStoraged(input))
                {
                    currentText.TextId = input.TextId;
                    currentText.System = true;

                    //Save all the entities
                    ITranslation.InsertTextTranslation(currentText, translation, translation_log);
                    return;
                }
                else
                {
                    errorMessage = "Some records already exist.";
                }

				//or only save the new translation and new translation log
				ITranslation.Save(translation);
				ITranslationLog.Save(translation_log);

			}

        }

        /// <summary>
        /// Get the blockType accordint to the first word of a file line read
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        BlockType GetBlockType(string line)
        {
            if (line.StartsWith("msgctxt"))
                return BlockType.Context;

            if (line.StartsWith("msgid_plural"))
                return BlockType.IdPlural;

            if (line.StartsWith("msgid"))
                return BlockType.Id;

            if (line.StartsWith("msgstr"))
                return BlockType.Str;

            return BlockType.None;
        }

               
        /// <summary>
        /// Get a translation from db for a current language param and textId param, 
        /// and set it to the global String Builder
        /// </summary>
        /// <param name="pTextId"></param>
        /// <param name="pOriginalText"></param>
        /// <param name="pTranslatedText"></param>
        /// <param name="LanguageCode"></param>
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